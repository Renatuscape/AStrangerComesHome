using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterNode : MonoBehaviour
{
    public string characterID;
    public bool allowOverride = false; // replaces node with speaker from a viable dialogue where location is explicitly set.
    public bool updateAtMidnight = false;
    public string alternateFloatText; // Display this text instead of character name
    public string alternativeSpriteID;
    public string activeDialogueID; // Character only appears if all dialogue requirements are met
    public int randomSpawnChance; // Leave at 0 to always spawn
    public bool continuouslyCheckRequirements; // If false, checks will not be reevaluated until destroyed
    public RequirementPackage customRequirements;
    public Character character;
    public bool isDormant = false; // allows other classes know not to enable the node
    public string textToDisplay;
    public string generatedNodeID;
    public CharacterNodeBobber bobber;

    SpriteRenderer rend;
    CapsuleCollider2D col;
    bool failedRNG = false;

    void Start()
    {
        generatedNodeID = (string.IsNullOrEmpty(characterID) ? "overrideNode" : characterID) + Random.Range(100000, 999999);
        WorldNodeTracker.AddCharacterNode(this);

        rend = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();

        if (randomSpawnChance == 0 || Random.Range(0, 100) <= randomSpawnChance)
        {
            AttemptSpawn();
        }
        else
        {
            failedRNG = true;
            HideNode();
        }
    }

    void AttemptSpawn()
    {
        if (allowOverride)
        {
            FindAnyViableSpeaker();
        }

        if ((character == null || string.IsNullOrEmpty(character.objectID)) && !string.IsNullOrEmpty(characterID))
        {
            character = Characters.FindByID(characterID);

            if (allowOverride)
            {
                generatedNodeID = character.objectID + "_" + generatedNodeID;
            }
        }

        if (character != null && !string.IsNullOrEmpty(character.objectID))
        {
            if (AttemptChecks())
            {
                EnableCharacter();
            }
            else
            {
                Debug.Log("CNode: Checks failed. Attempting to hide node " + character.objectID);
                HideNode();
            }

            if (continuouslyCheckRequirements)
            {
                StartCoroutine(ContinuouslyCheckRequirements());
            }
        }
        else
        {
            HideNode();
        }
    }

    public void RefreshNode()
    {
        if (character == null)
        {
            AttemptSpawn();
        }

        if (!failedRNG && AttemptChecks())
        {
            Debug.Log("CNode: Checks succeeded. Attempting setup if alpha is 0 for " + generatedNodeID);
            if (isDormant)
            {
                isDormant = false;
                ConfigureDisplayText();
                FindSprite();
                StartCoroutine(FadeInAndEnable());
            }
        }
        else
        {
            Debug.Log("CNode: Checks failed. Attempting fade and hide if not already dormant " + generatedNodeID);

            if (!isDormant)
            {
                StartCoroutine(FadeOutNode(true));
            }
        }
    }

    public void FadeInAfterTemporaryDisable()
    {
        if (!isDormant)
        {
            ConfigureDisplayText();
            FindSprite();
            StartCoroutine(FadeInAndEnable());
        }
    }

    public void DisableWithFade()
    {
        //Debug.Log("Disable with fade was called for " + this);
        col.enabled = false;
        StartCoroutine(FadeOutNode(true));
    }

    public void TemporarilyHide()
    {
        StartCoroutine(FadeOutNode(false));
    }

    void HideNode()
    {
        bobber.HideBobber();
        isDormant = true;
        Debug.Log("CNode: Hiding character " + characterID);
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0);
        col.enabled = false;
    }

    bool AttemptChecks()
    {
        bool passedCustomRequirements;
        bool passedDialogueRequirements;
        bool isAlreadySpawned = character != null && WorldNodeTracker.CheckIfCharacterExistsInDifferentNode(this);

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
            if (allowOverride)
            {
                passedDialogueRequirements = HasAnyRelevantDialogue();
            }
            else
            {
                passedDialogueRequirements = true;
            }
        }
        else
        {
            Dialogue dialogue = Dialogues.FindByID(activeDialogueID);
            passedDialogueRequirements = RequirementChecker.CheckDialogueRequirements(dialogue);
            Debug.Log("CNode: PassedDialogueRequirements = " + passedDialogueRequirements);
        }

        //Debug.Log($"Attempted to spawn {characterID}. Dialogue requirements: {passedDialogueRequirements}. Custom requirements passed: {passedCustomRequirements}. Is already spawned: {isAlreadySpawned}");
        return passedCustomRequirements && passedDialogueRequirements && !isAlreadySpawned;
    }

    void EnableCharacter()
    {
        Debug.Log("CNode: Attempting to enable character " + characterID);

        if (character != null)
        {
            isDormant = false;
            ConfigureDisplayText();
            FindSprite();
            StartCoroutine(FadeInAndEnable());
        }
        else
        {
            Debug.LogWarning("CNode: No character found with ID " + characterID + ". Use different class for decorative NPCs.");
            HideNode();
        }
    }

    void FindSprite()
    {
        if (!string.IsNullOrEmpty(alternativeSpriteID))
        {
            rend.sprite = SpriteFactory.GetWorldCharacterSprite(alternativeSpriteID);
        }
        else if (character != null)
        {
            rend.sprite = SpriteFactory.GetWorldCharacterSprite(character.objectID);
        }
    }

    void ConfigureDisplayText()
    {
        if (!string.IsNullOrEmpty(alternateFloatText))
        {
            textToDisplay = alternateFloatText;
        }
        else if (character != null)
        {
            textToDisplay = character.NamePlate();
        }

        //Debug.Log("Display text configured to " + textToDisplay);
    }

    void FindAnyViableSpeaker()
    {
        foreach (Quest quest in Quests.all)
        {
            if (quest.dialogues != null && quest.dialogues.Count > 0)
            {
                int stage = Player.GetCount(quest.objectID, "Character node any speaker");

                Dialogue dialogue = quest.dialogues.FirstOrDefault(d => d.questStage == stage);

                if (dialogue != null &&
                    !dialogue.disableAutoNode &&
                    !string.IsNullOrEmpty(dialogue.checks.locationID) &&
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

                        if (!WorldNodeTracker.CheckIfCharacterExistsInWorld(speaker))
                        {
                            characterID = speaker;
                            Debug.Log("Found viable dialogue and speaker for location.");
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
    }

    void OnMouseOver()
    {
        if (!string.IsNullOrEmpty(textToDisplay) && TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            TransientDataScript.PrintFloatText(textToDisplay);
        }
    }

    void OnMouseExit()
    {
        if (!string.IsNullOrEmpty(textToDisplay))
        {
            TransientDataScript.DisableFloatText();
        }
    }

    void OnMouseDown()
    {
        if (character != null && TransientDataScript.transientData.currentSpeed == 0)
        {
            if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
            {
                // Debug.Log($"Opening interact menu with {character.name}");
                InteractMenu.Open(character);
            }
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        WorldNodeTracker.RemoveNodeFromList(this);
    }

    IEnumerator ContinuouslyCheckRequirements()
    {
        Debug.Log("CNode: Started requirement checker for continuous spawn checks.");
        while (true)
        {
            yield return new WaitForSeconds(5);

            if (continuouslyCheckRequirements && TransientDataScript.GameState == GameState.Overworld)
            {
                RefreshNode();
            }
        }
    }

    IEnumerator FadeOutNode(bool hide)
    {
        //Debug.Log("Fade Node And Hide Called");
        float alpha = rend.color.a;
        float fadeValue = 0.001f;

        while (alpha > 0)
        {
            yield return new WaitForSeconds(0.005f);
            alpha -= fadeValue;
            fadeValue += 0.0005f;

            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
        }

        if (hide)
        {
            HideNode();
        }
    }

    IEnumerator FadeInAndEnable()
    {
        //Debug.Log("Fade In And Enable Called");
        float alpha = 0;
        float fadeValue = 0.01f;

        bobber.EnableBobber(character, rend);

        while (alpha < 1)
        {
            alpha += fadeValue;
            fadeValue += 0.005f;

            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            col.enabled = true;

            yield return new WaitForSeconds(0.05f);
        }
    }

    bool HasAnyRelevantDialogue()
    {
        if (character.objectID != null)
        {
            var dialogues = Dialogues.GetDialoguesBySpeaker(character.objectID, true);

            foreach ( var dialogue in dialogues)
            {
                var passed = dialogue.CheckRequirements();

                if (passed)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
