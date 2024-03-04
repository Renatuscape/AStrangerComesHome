using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractNode : MonoBehaviour
{
    public IdIntPair memoryNode;
    public string walkingNpcId;
    public bool ignoreWalkingConditions;
    public string repeatingItemQuest;
    public int itemSpawnChance = 100;

    public BoxCollider2D boxCollider;
    public SpriteRenderer nodeSprite;
    public Dialogue memory;
    public Dialogue itemNode;
    public Character character;
    public MemoryMenu memoryMenu;

    public AnimatedSprite placeholderNpc; //Retrieve information from AnimationLibrary
    public List<Sprite> itemCrate;
    public AnimatedSprite memoryShard; //Retrieve information from AnimationLibrary

    public float animationFrameRate;
    public float animationTimer;
    public int animationFrameIndex;
    public bool playAnimation;

    public bool isSpawningMemory = false;
    public bool isSpawningNpc = false;
    public bool isSpawningItem = false;
    public bool isClickable = true;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        SetUpNode();
    }

    private void Update()
    {
        if (playAnimation)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationFrameRate)
            {
                Animate();
                animationTimer = 0;
            }
        }

        if (TransientDataScript.GameState == GameState.Overworld && isClickable)
        {
            boxCollider.enabled = true;
        }
        else
        {
            boxCollider.enabled = false;
        }
    }

    private void OnMouseOver()
    {
        if (isSpawningNpc && character != null)
        {
            TransientDataCalls.PrintFloatText(character.NamePlate());
        }
        else if (isSpawningMemory)
        {
            TransientDataCalls.PrintFloatText("A Memory");
        }
    }

    private void OnMouseExit()
    {
        TransientDataCalls.DisableFloatText();
    }

    private void OnMouseDown()
    {
        if (isSpawningNpc)
        {
            HandleNpcClick();
        }
        else if (isSpawningMemory)
        {
            HandleMemoryClick();
        }
        else if (isSpawningItem)
        {
            HandleItemClick();
        }
    }
    private void Animate()
    {
        animationFrameIndex++;

        if (isSpawningItem)
        {
            if (animationFrameIndex == itemCrate.Count)
            {
                nodeSprite.color = new Color(1, 1, 1, 0.5f);
            }
            else if (animationFrameIndex > itemCrate.Count)
            {
                nodeSprite.enabled = false;
                playAnimation = false;
            }
            else
            {
                nodeSprite.sprite = itemCrate[animationFrameIndex];
            }
        }
    }

    private void HandleNpcClick()
    {
        Debug.Log($"Opening interact menu with {character.name}");
        InteractMenu.Open(character);
    }
    private void HandleMemoryClick()
    {
        if (memoryMenu == null)
        {
            Debug.Log("Getting memory menu");
            memoryMenu = TransientDataCalls.GetStorySystem().memoryMenu;
        }
        if (memoryMenu != null)
        {
            bool isSuccess = memoryMenu.InitialiseReader(memoryNode.objectID, memoryNode.amount);

            if (!isSuccess)
            {
                Debug.Log("Memory initialisation failed");
            }
            else
            {
                Debug.Log("Memory initialisation succeeded");
            }
        }
    }
    private void HandleItemClick()
    {
        AudioManager.PlayUISound("cloth3");
        animationTimer = 100; //Start animating immediately
        playAnimation = true;
        isClickable = false;

        Choice choice = itemNode.choices[Random.Range(0, itemNode.choices.Count)];
        Player.Set(repeatingItemQuest, itemNode.choices[0].advanceTo);

        var rewards = choice.rewards;

        foreach (var entry in rewards)
        {
            Player.Add(entry.objectID, entry.amount);
        }
    }

    private void SetUpNode()
    {
        if (!string.IsNullOrEmpty(memoryNode.objectID))
        {
            if (Player.GetCount(memoryNode.objectID, name) == memoryNode.amount)
            {
                SetUpMemory();
            }
        }
        if (!isSpawningMemory && !string.IsNullOrEmpty(walkingNpcId))
        {
            Character foundCharacter = Characters.FindByID(walkingNpcId);

            if (foundCharacter == null)
            {
                Debug.LogWarning($"Walking NPC ID {walkingNpcId} was not found in Characters.all.");
            }
            else if (foundCharacter.walkingLocations == null || foundCharacter.walkingLocations.Count == 0)
            {
                Debug.LogWarning($"Attempting to walk NPC without walking locations {walkingNpcId}.");

                if (ignoreWalkingConditions)
                {
                    Debug.LogWarning("Ignoring walking conditions.");
                    SetUpWalkingNpc(foundCharacter);
                }
            }
            else
            {
                bool foundValidWalkingLocation = false;
                foreach (var walkingLocation in foundCharacter.walkingLocations)
                {
                    if (walkingLocation.CheckRequirement())
                    {
                        Debug.Log("Found valid walking location for " + foundCharacter.objectID);
                        foundValidWalkingLocation = true;
                    }
                }
                if (foundValidWalkingLocation)
                {
                    SetUpWalkingNpc(foundCharacter);
                }

            }
        }
        if (!string.IsNullOrEmpty(repeatingItemQuest))
        {
            Quest quest = Quests.FindByID(repeatingItemQuest);
            int stage = Player.GetCount(repeatingItemQuest, "node checker");

            Debug.Log($"Node Spawner looking for {repeatingItemQuest}. Found {quest.name}");

            if (stage < quest.dialogues.Count)
            {
                Dialogue foundDialogue = quest.dialogues[stage];
                Debug.Log($"Dialogue found is {foundDialogue.objectID}. Dialogue count is {quest.dialogues.Count}");

                if (foundDialogue != null)
                {
                    Debug.Log("Found dialogue was not null.");
                    if (foundDialogue.stageType == StageType.Node)
                    {
                        Debug.Log("Stage type was node.");
                        SetUpItem(foundDialogue);
                    }

                }
            }
        }

        if (!isSpawningMemory && !isSpawningNpc && !isSpawningItem)
        {
            nodeSprite.enabled = false;
            boxCollider.enabled = false;
        }
    }

    private void SetUpMemory()
    {
        try
        {
            Dialogue foundMemory = Quests.all.Where(q => (q.objectID == memoryNode.objectID)).FirstOrDefault().dialogues[memoryNode.amount] ?? null;

            if (RequirementChecker.CheckDialogueRequirements(foundMemory))
            {
                isSpawningMemory = true;
                nodeSprite.sprite = memoryShard.still;
            }
        }
        catch (IndexOutOfRangeException)
        {
            Debug.Log($"No valid quest found for {memoryNode.objectID} stage {memoryNode.amount}.");
        }
    }

    private void SetUpWalkingNpc(Character foundCharacter)
    {
        isSpawningNpc = true;
        character = foundCharacter;
        nodeSprite.sprite = placeholderNpc.still;
    }

    private void SetUpItem(Dialogue dialogue)
    {
        Debug.Log("Starting item setup.");
        var random = Random.Range(0, 100);

        if (random < itemSpawnChance)
        {
            Debug.Log("Passed random spawn check!");
            itemNode = dialogue;
            isSpawningItem = true;
            nodeSprite.sprite = itemCrate[0];
        }
        else
        {
            Debug.Log("Failed random spawn check!");
        }
    }
}

public static class RequirementChecker
{

    public static bool CheckDialogueRequirements(Dialogue dialogue)
    {
        bool timeCheck = CheckTime(dialogue.startTime, dialogue.endTime);
        bool locationCheck = CheckLocation(dialogue.location);
        bool requirementCheck = CheckRequirements(dialogue.requirements);
        bool restrictionCheck = CheckRestrictions(dialogue.restrictions);

        if (timeCheck && locationCheck && requirementCheck && restrictionCheck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CheckTime(float startTime, float endTime)
    {
        if (startTime != endTime)
        {
            float currentTime = TransientDataCalls.GetTimeOfDay();

            if (currentTime > endTime)
            {
                return false;
            }
            else if (currentTime < startTime)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }

    }
    public static bool CheckLocation(string objectID)
    {
        Location requiredLocation = Locations.FindByID(objectID);

        if (requiredLocation == null)
        {
            Debug.LogWarning("Location check returned true because required location returned null.");
            return true;
        }
        else
        {
            if (TransientDataCalls.GetCurrentLocation() == requiredLocation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static bool CheckRequirements(List<IdIntPair> requirements)
    {
        if (requirements != null && requirements.Count > 0)
        {
            foreach (IdIntPair requirement in requirements)
            {
                int amount = Player.GetCount(requirement.objectID, "Choice Requirement Check");

                if (amount < requirement.amount)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckRestrictions(List<IdIntPair> restrictions)
    {
        if (restrictions != null && restrictions.Count > 0) //don't run if checks already failed
        {
            foreach (IdIntPair restriction in restrictions)
            {
                int amount = Player.GetCount(restriction.objectID, "Choice Restriction Check");

                if (amount > restriction.amount)
                {
                    return false;
                }
            }
            Debug.Log("Returned true.");
        }

        return true;
    }
}