using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum SynthesiserType
{
    Stella,
    Capital,
    Magus,
    Home,
    Coach,
    Coach2,
    Coach3
}
public class AlchemyMenu : MonoBehaviour
{
    public DataManagerScript dataManager;
    public AlchemyTracker alchemyTracker;
    public SynthesiserData synthData;
    public AlchemySelectedIngredients selectedIngredients;
    public AlchemyInventory inventory;
    public AlchemyProgressBar progressBar;
    public AlchemyYieldManager yieldManager;
    public AlchemyButtonManager buttonManager;
    public SynthesiserType synthesiserType;

    public List<AlchemyObject> alchemyObjects = new();

    public GameObject inventoryContainer;
    public GameObject materialContainer;
    public GameObject infusionContainer;
    public GameObject dragParent;

    public GameObject infusionList;
    public GameObject materialList;

    public bool isDebugging = true;
    bool containersEnabled;

    private void Start()
    {
        dataManager = TransientDataCalls.gameManager.dataManager;
        SetUpContainers();
    }

    private void OnEnable()
    {
        if (isDebugging)
        {
            Initialise(SynthesiserType.Stella);
        }
    }

    public void Initialise(SynthesiserType synthesiserType)
    {
        this.synthesiserType = synthesiserType;

        string name = synthesiserType.ToString();

        if (dataManager == null)
        {
            dataManager = TransientDataCalls.gameManager.dataManager;
        }

        synthData = dataManager.alchemySynthesisers.FirstOrDefault(s => s.synthesiserID == name);

        Initialise(synthData);
    }

    public void Initialise(SynthesiserData synthData)
    {
        if (synthData == null)
        {
            synthData = new() { synthesiserID = name };
            if (synthData.synthesiserID.ToLower().Contains("coach"))
            {
                synthData.consumesMana = true;
            }
            dataManager.alchemySynthesisers.Add(synthData);
        }

        if (synthData != null && synthData.synthRecipe != null)
        {
            synthData.synthRecipe.SetWorkload(); // In case the formula has been changed, update workload

            alchemyObjects = SetUpAlchemyObjects(isDebugging);
            inventory.RenderInventory(ItemType.Catalyst, false);
            gameObject.SetActive(true);
            TransientDataCalls.SetGameState(GameState.AlchemyMenu, name, gameObject);
            progressBar.alchemyMenu = this;
            progressBar.Initialise(synthData);
            yieldManager.Setup(synthData);
        }
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
                        int amount = item.GetCountPlayer();

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
            newObject.AddToInventory(entry, this);
            alcObjects.Add(newObject);
        }

        return alcObjects;
    }

    public void HandleClaim()
    {
        yieldManager.Clear();
        synthData.synthRecipe.AddYieldToPlayer();
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
            TransientDataCalls.PushAlert("I need a catalyst and a type of plant for the infusion.");
        }
        else
        {
            var infusions = alchemyObjects.Where(ob => ob.isInfusion && ob.currentlyOnTable > 0).ToList();
            List<AlchemyDraggableItem> draggablePrefabs = new();

            if (infusions.Count != 2)
            {
                //Debug.Log($"Infusions list count was {infusions.Count}");
                TransientDataCalls.PushAlert("The infusion isn't correctly balanced.");
                TransientDataCalls.PushAlert("There should be one type of catalyst and one type of plant in the bowl.");
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

        Destroy(prefab);
    }

    void CreateFailure()
    {
        TransientDataCalls.PushAlert("I think something went wrong...");
    }

    private void OnDisable()
    {
        foreach (var entry in alchemyObjects)
        {
            Destroy(entry.inventoryClass.gameObject);
            Destroy(entry.selectedEntryPrefab);

            if (entry.draggableObjects != null)
            {
                foreach (var obj in entry.draggableObjects)
                {
                    Destroy(obj.gameObject);
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
