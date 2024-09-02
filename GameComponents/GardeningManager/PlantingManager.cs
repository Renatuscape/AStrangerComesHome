using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//HANDLES PLANTING SEEDS
public class PlantingManager : MonoBehaviour
{
    public GardenManager gardenManager;
    public DataManagerScript dataManager;
    public PageinatedContainer seedContainer;
    public PlanterData activePlanter;

    public GameObject planterFrame;
    public GameObject planterA;
    public Image planterIconA;
    public GameObject planterB;
    public Image planterIconB;
    public GameObject planterC;
    public Image planterIconC;
    public Sprite planterIconFree;

    public bool readyToPlant;
    public int unlockedPlanters;
    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        planterFrame.SetActive(false);

        planterIconA = planterA.GetComponent<Image>();
        planterIconB = planterB.GetComponent<Image>();
        planterIconC = planterC.GetComponent<Image>();
    }

    private void OnEnable()
    {
        unlockedPlanters = Player.GetCount(StaticTags.UnlockedPlanters, name);

        planterFrame.SetActive(false);
        readyToPlant = false;

        if (unlockedPlanters == 1)
        {
            planterA.gameObject.SetActive(true);
            planterB.gameObject.SetActive(false);
            planterC.gameObject.SetActive(false);
        }
        else if (unlockedPlanters == 2)
        {
            planterA.gameObject.SetActive(true);
            planterB.gameObject.SetActive(true);
            planterC.gameObject.SetActive(false);
        }
        else if (unlockedPlanters >= 3)
        {
            planterA.gameObject.SetActive(true);
            planterB.gameObject.SetActive(true);
            planterC.gameObject.SetActive(true);
        }
        else
        {
            planterA.gameObject.SetActive(false);
            planterB.gameObject.SetActive(false);
            planterC.gameObject.SetActive(false);
        }

        PrintSeeds();

        DynamicPlanterSelection();

        if (TransientDataScript.IsTimeFlowing())
        {
            UpdatePlanterIcons();
        }
    }


    private void UpdatePlanterIcons()
    {
        unlockedPlanters = Player.GetCount(StaticTags.UnlockedPlanters, name);

        if (unlockedPlanters > 0)
        {
            var planterData = dataManager.planters.FirstOrDefault(p => p.planterID == "planterA");

            if (planterData.isActive)
            {
                planterIconA.sprite = SpriteFactory.GetItemSprite(planterData.seed.objectID);
            }
            else
            {
                planterIconA.sprite = planterIconFree;
            }
        }
        if (unlockedPlanters > 1)
        {
            var planterData = dataManager.planters.FirstOrDefault(p => p.planterID == "planterB");

            if (planterData.isActive)
            {
                planterIconB.sprite = SpriteFactory.GetItemSprite(planterData.seed.objectID);
            }
            else
            {
                planterIconB.sprite = planterIconFree;
            }
        }
        if (unlockedPlanters > 2)
        {
            var planterData = dataManager.planters.FirstOrDefault(p => p.planterID == "planterC");

            if (planterData.isActive)
            {
                planterIconC.sprite = SpriteFactory.GetItemSprite(planterData.seed.objectID);
            }
            else
            {
                planterIconC.sprite = planterIconFree;
            }
        }

        TransientDataScript.gameManager.coachPlanters.CheckUnlockedPlanters();
    }

    private void PrintSeeds()
    {
        List<Item> seedStock = new();

        foreach (Item item in Items.all)
        {
            if (item.type == ItemType.Seed)
            {

                if (Player.GetCount(item.objectID, name) > 0 && item.GetOutput() != null)
                {
                    seedStock.Add(item);
                }
            }
        }

        var seeds = seedContainer.Initialise(seedStock, true, true, true);

        foreach (var item in seeds)
        {
            var uiData = item.GetComponent<ItemIconData>();
            uiData.printRarity = true;
            uiData.printSeedData = true;
        }
    }

    private void RemoveSeeds()
    {
        planterFrame.SetActive(false);
        planterFrame.transform.SetParent(planterA.transform, false);

        seedContainer.ClearPrefabs();
        readyToPlant = false;
    }

    private void DynamicPlanterSelection()
    {
        Debug.Log("Attempting to dynamically select next planter");
        int plantersUnlocked = Player.GetCount(StaticTags.UnlockedPlanters, name);
        int occupiedPlanters = 0;
        foreach (var planter in dataManager.planters)
        {
            if (planter.planterID == "planterA" && plantersUnlocked > 0)
            {
                if (!planter.isActive)
                {
                    MouseDownSelectPlanterA();
                    break;
                }
                else
                {
                    occupiedPlanters++;
                }
            }
            else if (planter.planterID == "planterB" && plantersUnlocked > 0)
            {
                if (!planter.isActive)
                {
                    MouseDownSelectPlanterB();
                    break;
                }
                else
                {
                    occupiedPlanters++;
                }
            }
            else if (planter.planterID == "planterC" && plantersUnlocked > 0)
            {
                if (!planter.isActive)
                {
                    MouseDownSelectPlanterC();
                    break;
                }
                else
                {
                    occupiedPlanters++;
                }
            }
            else
            {
                Debug.Log("Planter did not match any accepted IDs: " + planter.planterID);
            }

            if (occupiedPlanters == unlockedPlanters)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        readyToPlant = false;

        planterFrame?.SetActive(false);
        planterFrame?.transform.SetParent(planterA.transform, false);
        planterFrame?.SetActive(false);
        planterFrame?.transform.SetParent(planterA.transform, false);

        //delete prefabs here
        RemoveSeeds();
    }
    void Update()
    {
        if (TransientDataScript.CameraView != CameraView.Garden)
            gameObject.SetActive(false);

        if (seedContainer.selectedItem != null && planterFrame.activeInHierarchy == true)
            readyToPlant = true;

        //planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

    }

    public void SelectSeed(GameObject seedObject) //must pass the spawned prefab. Might have to be done in a script triggered by event
    {
        if (seedContainer.selectedItem.GetOutput() != null)
        {
            planterFrame.transform.SetParent(seedObject.transform);
            planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

            planterFrame.SetActive(true);
        }
        else
        {
            planterFrame.transform.SetParent(seedObject.transform);
            planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

            planterFrame.SetActive(true);
        }

    }

    //FOR SELECTING PLANTER IN THE MENU. CONSOLIDATE INTO ONE METHOD LATER, BUT IT WORKS FOR NOW
    public void MouseDownSelectPlanterA()
    {
        Debug.Log("MouseDown on planterA");
        activePlanter = dataManager.planters.FirstOrDefault(p => p.planterID == "planterA");
        planterFrame.transform.SetParent(planterA.transform, false);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }
    public void MouseDownSelectPlanterB()
    {
        Debug.Log("MouseDown on planterB");
        activePlanter = dataManager.planters.FirstOrDefault(p => p.planterID == "planterB");
        planterFrame.transform.SetParent(planterB.transform, false);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }
    public void MouseDownSelectPlanterC()
    {
        Debug.Log("MouseDown on planterC");
        activePlanter = dataManager.planters.FirstOrDefault(p => p.planterID == "planterC");
        planterFrame.transform.SetParent(planterC.transform, false);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }

    public void PlantThatSeed()
    {
        if (readyToPlant && seedContainer.selectedItem != null) //both seed and planter has been selected
        {
            if (Player.GetCount(seedContainer.selectedItem.objectID, name) > 0)
            {
                if (activePlanter != null)
                {
                    if (!activePlanter.isActive && !string.IsNullOrEmpty(seedContainer.selectedItem.objectID))
                    {
                        StorePlanterData(activePlanter);
                    }
                    else
                    {
                        LogAlert.QueueTextAlert("This planter is occupied.");
                    }
                }
            }
            else
            {
                Debug.Log("I am all out of this type of seeds.");
            }
        }
        else
        {
            LogAlert.QueueTextAlert("I must select a seed and an empty planter.");
        }
    }

    public void StorePlanterData(PlanterData planterData)
    {
        if (seedContainer.selectedItem != null)
        {
            Player.Remove(seedContainer.selectedItem.objectID);

            planterData.seed = Items.FindByID(seedContainer.selectedItem.objectID);

            Debug.Log($"Saving seed {planterData.seed.objectID}. Health registered as " + planterData.seed.health);

            planterData.seedHealth = planterData.seed.health;
            planterData.isActive = true;

            bool isAnyPlanterFree = false;

            foreach (var planter in dataManager.planters)
            {
                if (!planter.isActive)
                {
                    isAnyPlanterFree = true;
                    break;
                }
            }

            if (!isAnyPlanterFree)
            {
                gameObject.SetActive(false);
            }

            DynamicPlanterSelection();
            UpdatePlanterIcons();
        }

        TransientDataScript.gameManager.coachPlanters.Setup();
    }
}
