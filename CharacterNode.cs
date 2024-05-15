using System.Collections;
using System.Linq;
using UnityEngine;

public class CharacterNode : MonoBehaviour
{
    public bool allowOverride = false; // replaces node with speaker from a viable dialogue where location is explicitly set.
    public bool ignoreCustomReqOnOverride = false;
    public bool updateAtMidnight = false;
    public Character character;
    public string characterID;
    public string alternateFloatText; // Display this text instead of character name
    public string alternativeSpriteID;
    public string activeDialogueID; // Character only appears if all dialogue requirements are met
    public int randomSpawnChance; // Leave at 0 to always spawn
    public bool ornamentalOnly; // Will not be changed by checks. Prevents any re-enabling. Never applied to overrides.
    public bool disableFloatText; // Will not display float text. Enforced if character is unlinked and there is no alternative.
    public bool doNotLinkCharacterData; // Node is not linked to an actual character object. Interaction disabled
    public bool continuouslyCheckRequirements; // If false, checks will not be reevaluated until destroyed
    public RequirementPackage customRequirements;
    public bool isDormant = true;

    SpriteRenderer sRender;
    CapsuleCollider2D col;
    string textToDisplay;
    bool isReadyToRetest;
    float readyTimer = 0;
    float readyTick = 2;
    bool fadeIn = true;

    void Start()
    {
        CharacterNodeTracker.allExistingNodes.Add(this);
        sRender = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();
        SetupNode();
        fadeIn = true;
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
        if (character != null && TransientDataScript.transientData.currentSpeed == 0)
        {
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                // Debug.Log($"Opening interact menu with {character.name}");
                InteractMenu.Open(character);
            }
        }
    }

    public bool AttemptFadeIn()
    {
        return isDormant;
    }

    public void UpdateAtMidnight()
    {
        SetupNode();
    }

    public void DisableWithFade()
    {
        col.enabled = false;
        StartCoroutine(FadeNodeAndHide());
    }

    IEnumerator FadeNodeAndHide()
    {
        float alpha = sRender.color.a;
        float fadeValue = 0.01f;

        while (alpha > 0)
        {
            yield return new WaitForSeconds(0.05f);
            alpha -= fadeValue;
            fadeValue += 0.005f;

            sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, alpha);
        }

        HideNode();
    }

    IEnumerator FadeInAndEnable()
    {
        float alpha = 0;
        float fadeValue = 0.01f;

        while (alpha < 1)
        {
            alpha += fadeValue;
            fadeValue += 0.005f;

            sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, alpha);

            yield return new WaitForSeconds(0.05f);
        }

        if (!ornamentalOnly)
        {
            col.enabled = true;
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
            // Debug.Log("Attempting to set up character " + characterID);
            bool checksPassed = AttemptChecks();

            if (checksPassed)
            {
                if (randomSpawnChance == 0 || Random.Range(0, 100) <= randomSpawnChance)
                {
                    col.enabled = false;
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
        isDormant = true;
        //Debug.Log("Hiding character " + characterID);
        sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, 0);
        col.enabled = false;
    }

    bool AttemptChecks()
    {
        bool passedCustomRequirements;
        bool passedDialogueRequirements;
        bool isAlreadySpawned = CharacterNodeTracker.CheckIfCharacterExistsInWorld(characterID);

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

        // Debug.Log($"Attempted to spawn {characterID}. Dialogue requirements: {passedDialogueRequirements}. Custom requirements passed: {passedCustomRequirements}. Is already spawned: {isAlreadySpawned}");
        return passedCustomRequirements && passedDialogueRequirements && !isAlreadySpawned;
    }

    void EnableCharacter()
    {
        // Debug.Log("Attempting to enable character " + characterID);

        if (doNotLinkCharacterData)
        {
            NullCharacterConfiguration();
        }
        else
        {
            character = Characters.FindByID(characterID);

            if (character != null)
            {
                if (CharacterNodeTracker.AddCharacterNode(this))
                {
                    isDormant = false;
                    ConfigureDisplayText();
                    FindSprite();
                    
                    if (fadeIn)
                    {
                        // Debug.Log("Fading in node (" + characterID + "). Override is " + allowOverride);
                        StartCoroutine(FadeInAndEnable());
                    }
                    else
                    {
                        sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, 1);

                        if (!ornamentalOnly)
                        {
                            col.enabled = true;
                        }
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
        col.enabled = false;
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

                        if (!CharacterNodeTracker.CheckIfCharacterExistsInWorld(speaker))
                        {
                            characterID = speaker;
                            // Debug.Log("Found viable dialogue and speaker for location.");
                            foundSpeaker = true;
                            break;
                        }
                        else
                        {
                            // Debug.Log("Viable speaker was found, but had already been spawned. Continuing search.");
                        }
                    }
                }
            }
        }

        if (!foundSpeaker)
        {
            // Debug.Log("No viable speaker found for location. Hiding node.");
            HideNode();
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
        CharacterNodeTracker.allExistingNodes.Remove(this);
        CharacterNodeTracker.RemoveWorldCharacterFromList(character.objectID);
    }
}
