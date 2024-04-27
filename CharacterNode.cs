using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterNode : MonoBehaviour
{
    public bool allowOverride = false; // replaces node with speaker from a viable dialogue where location is explicitly set.
    public bool ignoreCustomReqOnOverride = false;
    public bool updateOnNewDay = false;
    public Character character;
    public string characterID;
    public string alternateFloatText; // Display this text instead of character name
    public string alternativeSpriteID;
    public string activeDialogueID; // Character only appears if all dialogue requirements are met
    public int randomSpawnChance; // Leave at 0 to always spawn
    public bool interactionDisabled; // Interact-menu will only be available if this is false. Enforced if character is unlinked.
    public bool disableFloatText; // Will not display float text. Enforced if character is unlinked and there is no alternative.
    public bool doNotLinkCharacterData; // Node is not linked to an actual character object. Interaction disabled
    public bool continuouslyCheckRequirements; // If false, checks will not be reevaluated until 
    public RequirementPackage customRequirements;

    SpriteRenderer sRender;
    CapsuleCollider2D col;
    string textToDisplay;
    bool isReadyToRetest;
    float readyTimer = 0;
    float readyTick = 2;

    public void UpdateOnNewDay()
    {
        SetupNode();
    }

    public void DisableWithFade()
    {
        // Implement enumeration
        HideNode();
    }
    void Start()
    {
        sRender = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();
        SetupNode();
    }

    private void OnMouseOver()
    {
        if (!disableFloatText && TransientDataScript.GameState == GameState.Overworld)
        {
            TransientDataScript.PrintFloatText(textToDisplay);
        }
    }

    private void OnMouseExit()
    {
        if (!disableFloatText)
        {
            TransientDataScript.DisableFloatText();
        }
    }

    private void OnMouseDown()
    {
        if (!interactionDisabled && character != null)
        {
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                Debug.Log($"Opening interact menu with {character.name}");
                InteractMenu.Open(character);
            }
        }
    }

    void SetupNode()
    {
        isReadyToRetest = false;

        if (allowOverride)
        {
            if (ignoreCustomReqOnOverride)
            {
                FindAnyViableSpeaker();
            }
            else if (AttemptChecks())
            {
                FindAnyViableSpeaker();
            }
            else
            {
                HideNode();
            }
        }
        else if (!string.IsNullOrEmpty(characterID))
        {
            Debug.Log("Attempting to set up character " + characterID);
            bool checksPassed = AttemptChecks();

            if (checksPassed)
            {
                if (randomSpawnChance == 0 || Random.Range(0, 100) <= randomSpawnChance)
                {
                    EnableCharacter();
                }
            }
            else
            {
                HideNode();
            }
        }
        else
        {
            HideNode();
        }
    }

    void HideNode()
    {
        Debug.Log("Hiding character " + characterID);
        interactionDisabled = true;
        sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, 0);
        col.enabled = false;
    }

    bool AttemptChecks()
    {
        bool passedCustomRequirements;
        bool passedDialogueRequirements;
        bool isAlreadySpawned = TransientDataScript.CheckIfCharacterExistsInWorld(characterID);

        if (customRequirements == null)
        {
            passedCustomRequirements = true;
        }
        else
        {
            passedCustomRequirements = RequirementChecker.CheckPackage(customRequirements);
        }

        if (string.IsNullOrEmpty(activeDialogueID))
        {
            passedDialogueRequirements = true;
        }
        else
        {
            Dialogue dialogue = Dialogues.FindByID(activeDialogueID);
            passedDialogueRequirements = RequirementChecker.CheckDialogueRequirements(dialogue);
        }

        Debug.Log($"Attempted to spawn {characterID}. Dialogue requirements: {passedDialogueRequirements}. Custom requirements passed: {passedCustomRequirements}. Is already spawned: {isAlreadySpawned}");
        return passedCustomRequirements && passedDialogueRequirements && !isAlreadySpawned;
    }

    void EnableCharacter()
    {
        Debug.Log("Attempting to enable character " + characterID);
        if (doNotLinkCharacterData)
        {
            NullCharacterConfiguration();
        }
        else
        {
            character = Characters.FindByID(characterID);

            if (character != null)
            {
                if (TransientDataScript.AddCharacterNode(this))
                {
                    ConfigureDisplayText();
                    FindSprite();

                    sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, 1);

                    if (!interactionDisabled)
                    {
                        col.enabled = true;
                    }
                }
                else
                {
                    Debug.LogWarning("Attempted to spawn already existing character. Hiding node.");
                    HideNode();
                }
            }
            else
            {
                Debug.LogWarning($"Character with ID {characterID} not found. Node is set to uninteractable.");
                NullCharacterConfiguration();
            }
        }
    }

    void NullCharacterConfiguration()
    {
        interactionDisabled = true;
        ConfigureDisplayText();
        FindSprite();
    }
    void FindSprite()
    {
        if (!string.IsNullOrEmpty(alternativeSpriteID))
        {
            sRender.sprite = SpriteFactory.GetWorldCharacterSprite(alternativeSpriteID);
        }
        else if (character != null)
        {
            sRender.sprite = SpriteFactory.GetWorldCharacterSprite(character.objectID);
        }
    }
    void ConfigureDisplayText()
    {
        if (!disableFloatText)
        {
            if (!string.IsNullOrEmpty(alternateFloatText))
            {
                textToDisplay = alternateFloatText;
            }
            else if (character != null)
            {
                textToDisplay = character.NamePlate();
            }
            else
            {
                disableFloatText = true;
            }
        }
    }

    void FindAnyViableSpeaker()
    {
        bool foundSpeaker = false;
        foreach (Quest quest in Quests.all)
        {
            if (quest.dialogues != null && quest.dialogues.Count > 0)
            {
                int stage = Player.GetCount(quest.objectID, "Character node any speaker");

                Dialogue dialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

                if (dialogue != null &&
                    !dialogue.disableAutoNode &&
                    !string.IsNullOrEmpty(dialogue.locationID) &&
                    dialogue.stageType == StageType.Dialogue)
                {
                    if (RequirementChecker.CheckDialogueRequirements(dialogue))
                    {
                        string speaker;

                        if (string.IsNullOrEmpty(dialogue.speakerID))
                        {
                            speaker = quest.questGiver.objectID;
                        }
                        else
                        {
                            speaker = dialogue.speakerID;
                        }

                        if (!TransientDataScript.CheckIfCharacterExistsInWorld(speaker))
                        {
                            characterID = speaker;
                            Debug.Log("Found viable dialogue and speaker for location.");
                            foundSpeaker = true;
                            break;
                        }
                        else
                        {
                            Debug.Log("Viable speaker was found, but had already been spawned. Continuing search.");
                        }
                    }
                }
            }
        }

        if (!foundSpeaker)
        {
            Debug.Log("No viable speaker found for location. Disabling override and setting up with default ID.");

            allowOverride = false;
            SetupNode();
        }
        else
        {
            EnableCharacter();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (continuouslyCheckRequirements && TransientDataScript.GameState == GameState.Overworld)
        {
            if (isReadyToRetest)
            {
                SetupNode();
            }
            else
            {
                readyTimer += Time.deltaTime;

                if (readyTimer >= readyTick)
                {
                    Debug.Log("Retried character node check");
                    readyTimer = 0;
                    isReadyToRetest = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        TransientDataScript.RemoveWorldCharacterFromList(character.objectID);
    }
}
