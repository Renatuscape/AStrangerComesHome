using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//HANDLES PLANTING SEEDS
public class PlantingManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public PageinatedContainer seedContainer;

    public GameObject planterFrame;
    public GameObject planterA;
    public Image planterIconA;
    public GameObject planterB;
    public Image planterIconB;
    public GameObject planterC;
    public Image planterIconC;
    public Sprite planterIconFree;

    public WhichPlanter activePlanter;
    public bool readyToPlant;

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
        planterFrame.SetActive(false);
        planterFrame.SetActive(false);
        readyToPlant = false;

        //spawn prefabs here
        PrintSeeds();

        DynamicPlanterSelection();
        UpdatePlanterIcons();
    }


    private void UpdatePlanterIcons()
    {
        if (dataManager.planterIsActiveA)
        {
            planterIconA.sprite = Items.all.FirstOrDefault(x => x.objectID == dataManager.seedA).sprite;
        }
        else
        {
            planterIconA.sprite = planterIconFree;
        }
        if (dataManager.planterIsActiveB)
        {
            planterIconB.sprite = Items.all.FirstOrDefault(x => x.objectID == dataManager.seedB).sprite;
        }
        else
        {
            planterIconB.sprite = planterIconFree;
        }
        if (dataManager.planterIsActiveC)
        {
            planterIconC.sprite = Items.all.FirstOrDefault(x => x.objectID == dataManager.seedC).sprite;
        }
        else
        {
            planterIconC.sprite = planterIconFree;
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

        readyToPlant = false;
    }

    private void DynamicPlanterSelection()
    {
        if (!dataManager.planterIsActiveA)
            MouseDownSelectPlanterA();
        else if (!dataManager.planterIsActiveB)
            MouseDownSelectPlanterB();
        else if (!dataManager.planterIsActiveC)
            MouseDownSelectPlanterC();
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
        activePlanter = WhichPlanter.PlanterA;
        planterFrame.transform.SetParent(planterA.transform);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }
    public void MouseDownSelectPlanterB()
    {
        activePlanter = WhichPlanter.PlanterB;
        planterFrame.transform.SetParent(planterB.transform);
        planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        planterFrame.SetActive(true);
    }
    public void MouseDownSelectPlanterC()
    {
        activePlanter = WhichPlanter.PlanterC;
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
                //PLANTER A
                if (activePlanter == WhichPlanter.PlanterA)
                {
                    if (!dataManager.planterIsActiveA && !string.IsNullOrEmpty(seedContainer.selectedItem.objectID))
                    {
                        StorePlanterData(ref dataManager.seedA, ref dataManager.seedHealthA, ref dataManager.planterIsActiveA);
                    }
                    else
                        Debug.Log("This planter is occupied."); //add option to remove plant?
                }
                //PLANTER B
                if (activePlanter == WhichPlanter.PlanterB)
                {
                    if (!dataManager.planterIsActiveB && !string.IsNullOrEmpty(seedContainer.selectedItem.objectID))
                    {
                        StorePlanterData(ref dataManager.seedB, ref dataManager.seedHealthB, ref dataManager.planterIsActiveB);
                    }
                    else
                        Debug.Log("This planter is occupied."); //add option to remove plant?
                }
                //PLANTER C
                if (activePlanter == WhichPlanter.PlanterC)
                {
                    if (!dataManager.planterIsActiveC && !string.IsNullOrEmpty(seedContainer.selectedItem.objectID))
                    {
                        StorePlanterData(ref dataManager.seedC, ref dataManager.seedHealthC, ref dataManager.planterIsActiveC);
                    }
                    else
                        Debug.Log("This planter is occupied."); //add option to remove plant?
                }
            }
            else
            {
                Debug.Log("I am all out of this type of seeds.");
            }
        }
    }

    public void StorePlanterData(ref string storedSeed, ref int storedHealth, ref bool planterIsActive)
    {
        if (seedContainer.selectedItem != null)
        {
            Player.Remove(seedContainer.selectedItem.objectID);

            storedSeed = seedContainer.selectedItem.objectID;
            storedHealth = seedContainer.selectedItem.health;
            planterIsActive = true;

            if (dataManager.planterIsActiveA && dataManager.planterIsActiveB && dataManager.planterIsActiveC)
                gameObject.SetActive(false); //Close planting manager if all planters are occupied

            Invoke("DynamicPlanterSelection", 0.01f);

            UpdatePlanterIcons();
        }
    }
}
