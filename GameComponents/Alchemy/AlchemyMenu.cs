using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum SynthesiserType
{
    Stella,
    Capital,
    Magus,
    Home,
    Coach
}
public class AlchemyMenu : MonoBehaviour
{
    public static SynthesiserData synthData;
    public SynthesiserData synthDataDebug;
    public PageinatedContainer pageinatedContainer;

    public DataManagerScript dataManager;
    public AlchemyTracker alchemyTracker;
    public AlchemyAnimator alchemyAnimator;
    public AlchemySelectedIngredients selectedIngredients;
    public AlchemyProgressBar progressBar;
    public AlchemyYieldManager yieldManager;
    public AlchemyButtonManager buttonManager;
    public AlchemyRecipeTin recipeTin;

    public List<AlchemyObject> alchemyObjects = new();

    public GameObject inventoryContainer;
    public GameObject materialContainer;
    public GameObject infusionContainer;
    public GameObject dragParent;

    public GameObject infusionList;
    public GameObject materialList;

    public bool isDebugging = false;
    bool containersEnabled;

    public void Initialise(SynthesiserType synthesiserType)
    {
        if (!containersEnabled)
        {
            dataManager = TransientDataScript.gameManager.dataManager;
            SetUpContainers();
        }

        string synthName = synthesiserType.ToString();

        if (dataManager == null)
        {
            dataManager = TransientDataScript.gameManager.dataManager;
        }

        var foundSynth = dataManager.alchemySynthesisers.FirstOrDefault(s => s.synthesiserID == synthName);

        if (foundSynth == null)
        {
            Debug.Log($"Found synth with id {synthName} was null. Creating new.");

            foundSynth = new() { synthesiserID = synthName };

            if (foundSynth.synthesiserID.ToLower().Contains("coach"))
            {
                synthData.consumesMana = true;
            }

            dataManager.alchemySynthesisers.Add(foundSynth);
        }

        InitialiseBySynthesiser(foundSynth, isDebugging);
    }

    public void InitialiseBySynthesiser(SynthesiserData incomingSynthesiser, bool isDebugging = false)
    {
        if (!containersEnabled)
        {
            dataManager = TransientDataScript.gameManager.dataManager;
            SetUpContainers();
        }

        Debug.Log($"Received {incomingSynthesiser.synthesiserID} at alchemy menu.");

        synthData = incomingSynthesiser;
        synthDataDebug = synthData;

        if (synthData != null)
        {

            if (synthData.synthRecipe != null)
            {
                synthData.synthRecipe.SetWorkload(); // In case the formula has been changed, update workload
            }

            gameObject.SetActive(true);
            alchemyObjects = SetUpAlchemyObjects(isDebugging);
            //inventory.RenderInventory(ItemType.Catalyst, false);

            List<string> availableIngredients = new();

            foreach (var item in Items.all) // exclude seeds, misc, scripts and books, and any unique item
            {
                if (item.type == ItemType.Treasure
                || item.type == ItemType.Plant
                || item.type == ItemType.Trade
                || item.type == ItemType.Catalyst
                || item.type == ItemType.Material)
                {
                    if (item.rarity != ItemRarity.Unique)
                    {
                        int amount = Player.GetCount(item.objectID, name);

                        if (amount > 0)
                        {
                            availableIngredients.Add(item.objectID);
                        }
                    }
                }
            }

            var inventoryItems = pageinatedContainer.Initialise(availableIngredients, true, true, false);

            foreach (var obj in inventoryItems)
            {
                var itemIconScript = obj.GetComponent<ItemIconData>();

                var alcObj = alchemyObjects.FirstOrDefault(o => o.itemEntry.item.objectID == itemIconScript.item.objectID);

                if (alcObj != null)
                {
                    var inventoryScript = obj.AddComponent<AlchemyInventoryItem>();

                    inventoryScript.alchemyObject = alcObj;
                    inventoryScript.itemIconData = itemIconScript;
                }
            }

            TransientDataScript.SetGameState(GameState.AlchemyMenu, name, gameObject);
            progressBar.alchemyMenu = this;
            progressBar.Initialise(synthData);
            yieldManager.Setup(synthData);

        }
        else
        {
            Debug.Log("Synthdata was somehow null when opening alchemy menu.");
        }

        alchemyAnimator.Initialise(synthData);
    }
    void SetUpContainers()
    {
        if (!containersEnabled)
        {
            materialContainer.AddComponent<AlchemyContainer>();
            materialContainer.GetComponent<AlchemyContainer>().itemLimit = 40;
            materialContainer.GetComponent<AlchemyContainer>().reverseAnimation = true;
            materialContainer.AddComponent<RadialLayout>();
            var tableLayout = materialContainer.GetComponent<RadialLayout>();
            tableLayout.MinAngle = 360;
            tableLayout.fDistance = 280;

            infusionContainer.AddComponent<AlchemyContainer>();
            infusionContainer.GetComponent<AlchemyContainer>().itemLimit = 20;
            infusionContainer.AddComponent<RadialLayout>();
            var infusionLayout = infusionContainer.GetComponent<RadialLayout>();
            infusionLayout.MinAngle = 360;
            infusionLayout.fDistance = 110;

            containersEnabled = true;
        }
    }

    List<AlchemyObject> SetUpAlchemyObjects(bool isDebugging = false)
    {
        var alcObjects = new List<AlchemyObject>();

        List<ItemIntPair> availableIngredients = new();

        foreach (var item in Items.all) // exclude seeds, misc, scripts and books, and any unique item
        {
            if (item.type == ItemType.Treasure
            || item.type == ItemType.Plant
            || item.type == ItemType.Trade
            || item.type == ItemType.Catalyst
            || item.type == ItemType.Material)
            {
                if (item.rarity != ItemRarity.Unique)
                {

                    if (isDebugging)
                    {
                        availableIngredients.Add(new() { item = item, amount = 30 });
                    }
                    else
                    {
                        int amount = Player.GetCount(item.objectID, name);

                        if (amount > 0)
                        {
                            availableIngredients.Add(new() { item = item, amount = amount });
                        }
                    }
                }
            }
        }

        foreach (ItemIntPair entry in availableIngredients)
        {
            AlchemyObject newObject = new();
            newObject.Initialise(entry, this);
            alcObjects.Add(newObject);
        }

        return alcObjects;
    }

    public void HandleClaim()
    {
        yieldManager.Clear();
        synthData.synthRecipe.AddYieldToPlayer();

        // Handle recipe learning
        if (!synthData.synthRecipe.notResearchable && Player.GetCount(synthData.synthRecipe.objectID, "AlchemyMenu Claim()") == 0)
        {
            Player.Add(synthData.synthRecipe.objectID);
            string printName;

            if (synthData.synthRecipe.name.Contains(" Recipe"))
            {
                printName = synthData.synthRecipe.name.Replace(" Recipe", "");
            }
            else
            {
                printName = synthData.synthRecipe.name;
            }

            LogAlert.QueueTextAlert("Discovered recipe for\n" + printName);

            recipeTin.RefreshTin();
        }

        synthData.isSynthActive = false;
        synthData.progressSynth = 0;
        synthData.synthRecipe = null;
    }

    public void HandleCreate()
    {
        float animationTimer = 2;

        var foundCatalyst = alchemyObjects.FirstOrDefault(ob => ob.isInfusion && ob.itemEntry.item.type == ItemType.Catalyst);
        var foundPlant = alchemyObjects.FirstOrDefault(ob => ob.isInfusion && ob.itemEntry.item.type == ItemType.Plant);

        if (foundCatalyst == null || foundPlant == null)
        {
            TransientDataScript.PushAlert("I need a catalyst and a type of plant for the infusion.");
        }
        else
        {
            var infusions = alchemyObjects.Where(ob => ob.isInfusion && ob.currentlyOnTable > 0).ToList();
            List<AlchemyDraggableItem> draggablePrefabs = new();

            if (infusions.Count != 2)
            {
                //Debug.Log($"Infusions list count was {infusions.Count}");
                TransientDataScript.PushAlert("The infusion isn't correctly balanced.");
                TransientDataScript.PushAlert("There should be one type of catalyst and one type of plant in the bowl.");
            }
            else
            {
                List<IdIntPair> ingredientList = new();
                foreach (var ob in alchemyObjects)
                {
                    if (ob.currentlyOnTable > 0)
                    {
                        ingredientList.Add(new() { objectID = ob.objectID, amount = ob.currentlyOnTable });
                        draggablePrefabs.AddRange(ob.draggableObjects);
                        ob.UseForCreation(isDebugging);
                    }
                }

                synthData.synthRecipe = Recipes.AttemptAlchemy(ingredientList, out bool isSuccessful, isDebugging);
                synthData.isSynthActive = true;
                synthData.isSynthPaused = false;

                alchemyTracker.gameObject.SetActive(true);

                InitialiseAnimateCreate(draggablePrefabs, animationTimer);
                alchemyAnimator.AnimateCreate();

                if (!isSuccessful)
                {
                    Invoke("CreateFailure", animationTimer);
                }

                Debug.Log($"Began crafting {synthData.synthRecipe.objectID} ({synthData.synthRecipe.name})");
            }
        }
    }

    void InitialiseAnimateCreate(List<AlchemyDraggableItem> draggablePrefabs, float duration)
    {
        for (int i = draggablePrefabs.Count - 1; i >= 0; i--)
        {
            duration += i * 0.1f;
            duration = duration * 0.3f;

            StartCoroutine(AnimateCreate(draggablePrefabs[i].gameObject, duration));
        }
    }

    IEnumerator AnimateCreate(GameObject prefab, float duration)
    {
        //Debug.Log($"Animating {prefab.name}");
        prefab.transform.SetParent(dragParent.transform);
        Vector3 targetLocation = infusionContainer.transform.position;
        Vector3 startPosition = prefab.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            prefab.transform.position = Vector3.Lerp(startPosition, targetLocation, t);
            yield return null;
        }

        // Ensure the object is exactly at the target position
        prefab.transform.position = targetLocation;

        prefab.GetComponent<ItemIconData>().Return("AlchemyMenu on AnimateCreate");
    }

    void CreateFailure()
    {
        TransientDataScript.PushAlert("I think something went wrong...");
    }

    private void OnDisable()
    {
        pageinatedContainer.ClearPrefabs();

        foreach (var entry in alchemyObjects)
        {
            Destroy(entry.selectedEntryPrefab);//entry.selectedEntryPrefab.GetComponent<ItemIconData>().Return();
            // Debug.Log("Implement object pool for ItemRows!");

            if (entry.draggableObjects != null)
            {
                foreach (var obj in entry.draggableObjects)
                {
                    obj.gameObject.GetComponent<ItemIconData>().Return("AlchemyMenu on disable");
                }
            }
        }

        alchemyObjects.Clear();
    }

    public void SynthComplete()
    {
        if (!yieldManager.isYieldPrinted)
        {
            Debug.Log("Progress bar registered that synth was complete.");
            yieldManager.CheckCompletion();
            buttonManager.CheckButtons();
        }

    }
}
