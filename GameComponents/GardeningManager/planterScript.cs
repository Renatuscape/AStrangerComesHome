using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanterScript : MonoBehaviour
{
    public string planterID;
    public DataManagerScript dataManager;
    public GardenManager gardenManager;
    public BoxCollider2D boxCollider;
    public PlanterData planterData;
    public GameObject weedsPrefab;
    public Skill overgrowth;

    public int maxWeeds = 3;
    public float weedTimer;
    public int weedTick;

    bool foundPlanterData = false;

    void Awake()
    {
        weedTick = Random.Range(30, 120);
    }

    private void OnEnable()
    {
        FindPlanterData();
    }

    void FindPlanterData()
    {
        planterData = dataManager.planters.FirstOrDefault(p => p.planterID == planterID);

        if (planterData != null)
        {
            planterData.weeds = 0;
            foundPlanterData = true;
        }
        else
        {
            Debug.LogWarning("Could not find planter data that corresponds to planter ID " + planterID + ". Check ID.");
        }
    }
    private void OnMouseDown()
    {
        if (!foundPlanterData)
        {
            FindPlanterData();
        }

        if (foundPlanterData && TransientDataScript.IsTimeFlowing() && TransientDataScript.CameraView == CameraView.Garden)
        {
            Debug.Log($"{planterData.planterID} click registered.");
            gardenManager.ClickPlanter(planterData);
        }
    }

    private void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            weedTimer += Time.deltaTime;

            if (weedTimer >= weedTick)
            {
                weedTimer = 0;

                if (planterData.weeds < maxWeeds)
                    WeedTick();
            }

            if (TransientDataScript.GameState == GameState.Overworld)
            {
                if (TransientDataScript.CameraView == CameraView.Garden)
                {
                    if (planterData.weeds == 0)
                    {
                        boxCollider.enabled = true;
                    }
                    else
                    {
                        boxCollider.enabled = false;
                    }
                }
                else
                {
                    boxCollider.enabled = false;
                }
            }
            else
            {
                boxCollider.enabled = false;
            }
        }
    }

    void WeedTick()
    {
        var spawnChance = Random.Range(0, 100);

        if (spawnChance > 40)
        {
            planterData.weeds ++;
            var rand = Random.Range(-0.33f, 0.4f);
            var weed = Instantiate(weedsPrefab);
            weed.GetComponent<WeedsPrefab>().planterParent = this;
            weed.name = "Weeds";
            weed.transform.parent = gameObject.transform;
            weed.transform.localPosition = new Vector3(rand, -0.12f, 0);

            var weedFlip = Random.Range(0, 100);

            if (weedFlip < 40)
                weed.GetComponent<SpriteRenderer>().flipX = true;
        }

        weedTick = Random.Range(30, 120);
    }
}
