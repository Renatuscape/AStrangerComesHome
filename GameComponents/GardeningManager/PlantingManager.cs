using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

//HANDLES PLANTING SEEDS
public class PlantingManager : MonoBehaviour
{
    public DataManagerScript dataManager;

    public GameObject seedContainter;
    public GameObject gardenSeedPrefab;
    public GameObject planterFrame;
    public GameObject seedFrame;
    public GameObject planterA;
    public GameObject planterB;
    public GameObject planterC;

    public Image plantPreview;
    public WhichPlanter activePlanter;
    public Item activeSeed;
    public bool readyToPlant;
    public List<GameObject> seedPrefabs;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        seedFrame.SetActive(false);
        planterFrame.SetActive(false);
        plantPreview.sprite = null;
    }

    private void OnEnable()
    {
        seedFrame.SetActive(false);
        planterFrame.SetActive(false);
        readyToPlant = false;

        //spawn prefabs here
        PrintSeeds();

        DynamicPlanterSelection();
    }

    private void PrintSeeds()
    {
        foreach (Item item in Items.all)
        {
            if (item.type == ItemType.Seed)
            {

                if (item.GetCountPlayer() > 0)
                {
                    var prefab = Instantiate(gardenSeedPrefab);
                    prefab.name = item.name;
                    prefab.transform.SetParent(seedContainter.transform, false);
                    prefab.GetComponent<GardenSeedPrefab>().EnableObject(item, this);
                    seedPrefabs.Add(prefab);
                }
            }
        }
    }

    private void RemoveSeeds()
    {
        seedFrame?.SetActive(false);
        seedFrame?.transform.SetParent(planterA.transform, false);

        readyToPlant = false;

        foreach (var seed in seedPrefabs)
        {
            Destroy(seed);
        }

        seedPrefabs.Clear();
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

        seedFrame?.SetActive(false);
        seedFrame?.transform.SetParent(planterA.transform, false);
        planterFrame?.SetActive(false);
        planterFrame?.transform.SetParent(planterA.transform, false);

        //delete prefabs here
        RemoveSeeds();
    }
    void Update()
    {
        if (TransientDataScript.CameraView != CameraView.Garden)
            gameObject.SetActive(false);

        if (activeSeed != null && planterFrame.activeInHierarchy == true)
            readyToPlant = true;

        //planterFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        
    }

    public void SelectSeed(GameObject seedObject) //must pass the spawned prefab. Might have to be done in a script triggered by event
    {
        activeSeed = seedObject.GetComponent<GardenSeedPrefab>().itemSource;

        if (activeSeed.GetOutput() != null)
        {
        seedFrame.transform.SetParent(seedObject.transform);
        seedFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        plantPreview.sprite = activeSeed.GetOutput().sprite;
        seedFrame.SetActive(true);
        }
        else
        {
            seedFrame.transform.SetParent(seedObject.transform);
            seedFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

            plantPreview.sprite = null;
            seedFrame.SetActive(true);
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
        if (readyToPlant && activeSeed != null) //both seed and planter has been selected
        {
            if (Player.GetCount(activeSeed.objectID, name) > 0)
            {
                //PLANTER A
                if (activePlanter == WhichPlanter.PlanterA)
                {
                    if (!dataManager.planterIsActiveA && !string.IsNullOrEmpty(activeSeed.objectID))
                    {
                        StorePlanterData(ref dataManager.seedA, ref dataManager.seedHealthA, ref dataManager.planterIsActiveA);
                    }
                    else
                        Debug.Log("This planter is occupied."); //add option to remove plant?
                }
                //PLANTER B
                if (activePlanter == WhichPlanter.PlanterB)
                {
                    if (!dataManager.planterIsActiveB && !string.IsNullOrEmpty(activeSeed.objectID))
                    {
                        StorePlanterData(ref dataManager.seedB, ref dataManager.seedHealthB, ref dataManager.planterIsActiveB);
                    }
                    else
                        Debug.Log("This planter is occupied."); //add option to remove plant?
                }
                //PLANTER C
                if (activePlanter == WhichPlanter.PlanterC)
                {
                    if (!dataManager.planterIsActiveC && !string.IsNullOrEmpty(activeSeed.objectID))
                    {
                        StorePlanterData(ref dataManager.seedC, ref dataManager.seedHealthC, ref dataManager.planterIsActiveC);
                    }
                    else
                        Debug.Log("This planter is occupied."); //add option to remove plant?
                }

                if (Player.GetCount(activeSeed.objectID, name) == 0)
                {
                    GameObject outSeed = seedPrefabs.FirstOrDefault(x => x.name == activeSeed.name);

                    if (seedPrefabs.Count > 1) //If there are more seeds available, hop to next one
                    {
                        var index = seedPrefabs.IndexOf(outSeed);
                        int newIndex = 0;

                        if (newIndex < seedPrefabs.Count)
                        {
                            newIndex = index + 1;
                        }

                        seedFrame.transform.SetParent(seedPrefabs[newIndex].transform);
                        activeSeed = seedPrefabs[newIndex].GetComponent<GardenSeedPrefab>().itemSource;

                        seedFrame.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
                    }
                    else //if no more seeds are available, save the seed frame from deletion
                    {
                        readyToPlant = false;
                        seedFrame?.SetActive(false);
                        seedFrame?.transform.SetParent(planterA.transform, false);
                    }

                    seedPrefabs.Remove(outSeed);
                    Destroy(outSeed);
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
        if (activeSeed != null)
        {
            activeSeed.AddToPlayer(-1);
            storedSeed = activeSeed.objectID;
            storedHealth = activeSeed.health;
            planterIsActive = true;

            if (dataManager.planterIsActiveA && dataManager.planterIsActiveB && dataManager.planterIsActiveC)
                gameObject.SetActive(false); //Close planting manager if all planters are occupied

            Invoke("DynamicPlanterSelection", 0.5f);
        }
    }
}
