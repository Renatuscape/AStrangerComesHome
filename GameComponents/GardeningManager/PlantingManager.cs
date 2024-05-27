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

        if (unlockedPlanters >= 3)
        {
            DynamicPlanterSelection();
        }
        else
        {
            MouseDownSelectPlanterA();
        }

        UpdatePlanterIcons();
    }


    private void UpdatePlanterIcons()
    {
        if (gardenManager.planterPackages != null)
        {
            foreach (var planter in gardenManager.planterPackages)
            {
                if (planter.planterData.isActive)
                {
                    if (planter.planterData.planterID == "planterA")
                    {
                        planterIconA.sprite = Items.all.FirstOrDefault(x => x.objectID == planter.planterData.seed.objectID).sprite;
                    }
                    else if (planter.planterData.planterID == "planterB")
                    {
                        planterIconB.sprite = Items.all.FirstOrDefault(x => x.objectID == planter.planterData.seed.objectID).sprite;
                    }
                    else if (planter.planterData.planterID == "planterC")
                    {
                        planterIconC.sprite = Items.all.FirstOrDefault(x => x.objectID == planter.planterData.seed.objectID).sprite;
                    }
                }
                else
                {
                    if (planter.planterData.planterID == "planterA")
                    {
                        planterIconA.sprite = gardenManager.planterSprites.FirstOrDefault(s => s.name.Contains(planter.planterData.planterSpriteID));
                    }
                    else if (planter.planterData.planterID == "planterB")
                    {
                        planterIconB.sprite = gardenManager.planterSprites.FirstOrDefault(s => s.name.Contains(planter.planterData.planterSpriteID));
                    }
                    else if (planter.planterData.planterID == "planterC")
                    {
                        planterIconC.sprite = gardenManager.planterSprites.FirstOrDefault(s => s.name.Contains(planter.planterData.planterSpriteID));
                    }
                }
            }
        }
        else
        {
            Debug.Log("Garden manager was not ready.");
        }
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

        seedContainer.Initialise(seedStock, true, true, true);
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

        foreach (var planter in gardenManager.planterPackages)
        {
            if (!planter.planterData.isActive)
            {
                if (planter.planterData.planterID == "planterA")
                {
                    MouseDownSelectPlanterA();
                }
                else if (planter.planterData.planterID == "planterB")
                {
                    MouseDownSelectPlanterB();
                }
                else if (planter.planterData.planterID == "planterC")
                {
                    MouseDownSelectPlanterC();
                }
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
        activePlanter = dataManager.planters.FirstOrDefault(p => p.planterID == "planterA");
        planterFrame.transform.SetParent(planterA.transform);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }
    public void MouseDownSelectPlanterB()
    {
        activePlanter = dataManager.planters.FirstOrDefault(p => p.planterID == "planterB");
        planterFrame.transform.SetParent(planterB.transform);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }
    public void MouseDownSelectPlanterC()
    {
        activePlanter = dataManager.planters.FirstOrDefault(p => p.planterID == "planterC");
        planterFrame.transform.SetParent(planterC.transform);
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
    }

    public void StorePlanterData(PlanterData planterData)
    {
        if (seedContainer.selectedItem != null)
        {
            Player.Remove(seedContainer.selectedItem.objectID);

            planterData.seed = Items.FindByID(seedContainer.selectedItem.objectID);
            planterData.seedHealth = seedContainer.selectedItem.health;
            planterData.isActive= true;

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

            if (unlockedPlanters >= 3)
            {
                Invoke("DynamicPlanterSelection", 0.01f);
            }

            UpdatePlanterIcons();
        }
    }
}
