using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterNode : MonoBehaviour
{
    public Character character;
    public string characterID;
    public string alternateFloatText; // Display this text instead of character name
    public string alternativeSpriteID;
    public string activeDialogueID; // Character only appears if all dialogue requirements are met
    public string activeQuestID; // Character only appears during this quest. Stage must be > 0 and < 100
    public int minQuestStage;
    public int maxQuestStage;
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
            Debug.Log($"Opening interact menu with {character.name}");
            InteractMenu.Open(character);
        }
    }

    void SetupNode()
    {
        isReadyToRetest = false;

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
            sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, 0);
            col.enabled = false;
        }
    }

    bool AttemptChecks()
    {
        bool passedCustomRequirements;
        bool passedQuestRequirements = false;
        bool passedDialogueRequirements;

        if (customRequirements == null)
        {
            passedCustomRequirements = true;
        }
        else
        {
            passedCustomRequirements = RequirementChecker.CheckPackage(customRequirements);
        }

        if (string.IsNullOrEmpty(activeQuestID))
        {
            passedQuestRequirements = true;
        }
        else
        {
            int questStage = Player.GetCount(activeQuestID, name);

            if (questStage < maxQuestStage && questStage > minQuestStage)
            {
                passedQuestRequirements = true;
            }
            else
            {
                passedCustomRequirements = false;
            }
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

        return passedCustomRequirements && passedQuestRequirements && passedDialogueRequirements;
    }

    void EnableCharacter()
    {
        if (doNotLinkCharacterData)
        {
            NullCharacterConfiguration();
        }
        else
        {
            character = Characters.FindByID(characterID);

            if (character != null)
            {
                ConfigureDisplayText();
                FindSprite();
            }
            else
            {
                Debug.LogWarning($"Character with ID {characterID} not found. Node is set to uninteractable.");
                NullCharacterConfiguration();
            }
        }

        sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, 1);

        if (!interactionDisabled)
        {
            col.enabled = true;
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
}
