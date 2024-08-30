using System.Collections.Generic;
using UnityEngine;

public class GardenPlanter : MonoBehaviour
{
    public GardenCoachPlanters planterManager;
    public PlanterData planterData;
    public SpriteRenderer plantRend;
    public List<GameObject> weeds;
    public GameObject weedsPrefab;
    int maxWeeds = 3;
    public float weedTimer;
    public int weedTick;

    void Awake()
    {
        weedTick = Random.Range(45, 120);
    }

    public void UpdateFromPlanterData()
    {
        gameObject.SetActive(true);

        if (planterData.isActive && planterData.seed != null)
        {
            if (planterData.progress < planterData.GetMaxGrowth())
            {
                SetSprout();
            }
            else
            {
                SetFinishedPlant();
            }
        }
        else
        {
            HidePlant();
        }
    }

    public void HidePlanter()
    {
        gameObject.SetActive(false);

        HidePlant();

        foreach (GameObject weed in weeds)
        {
            Destroy(weed);
        }
    }

    public void AddWeed(GameObject weed)
    {
        weeds.Add(weed);

        //Set 
    }

    public void HidePlant()
    {
        plantRend.color = new Color(1, 1, 1, 0);
    }

    public void SetFinishedPlant()
    {
        gameObject.SetActive(true);
        plantRend.sprite = SpriteFactory.GetItemSprite(planterData.seed.outputID);
        plantRend.color = new Color(1, 1, 1, 1);
    }
    public void SetSprout()
    {
        gameObject.SetActive(true);
        int maxGrowth = planterData.GetMaxGrowth();
        float progress = planterData.progress;

        if (progress < maxGrowth * 0.3f)
        {
            plantRend.sprite = SpriteFactory.GetSprout(planterData.seed.objectID, 1);
        }
        else if (progress > maxGrowth * 0.3f && progress < maxGrowth * 0.6f)
        {
            plantRend.sprite = SpriteFactory.GetSprout(planterData.seed.objectID, 2);
        }
        else if (progress > maxGrowth * 0.6f)
        {
            plantRend.sprite = SpriteFactory.GetSprout(planterData.seed.objectID, 3);
        }

        plantRend.color = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        if (TransientDataScript.IsTimeFlowing() && planterData != null)
        {
            weedTimer += Time.deltaTime;

            if (weedTimer >= weedTick)
            {
                weedTimer = 0;

                if (planterData.weeds < maxWeeds)
                    WeedTick();
            }

            //if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Garden)
            //{
            //    boxCollider.enabled = true;
            //}
            //else
            //{
            //    boxCollider.enabled = false;
            //}
        }
    }

    void WeedTick()
    {
        UpdateFromPlanterData();

        var spawnChance = Random.Range(0, 100);

        if (spawnChance > 40)
        {
            planterData.weeds++;
            var rand = Random.Range(-0.33f, 0.4f);
            var weed = Instantiate(weedsPrefab);
            weed.GetComponent<WeedsPrefab>().planterParent = this;
            weed.name = "Weeds";
            weed.transform.parent = gameObject.transform;
            weed.transform.localPosition = new Vector3(rand, -0.12f, -1);

            var weedFlip = Random.Range(0, 100);

            if (weedFlip < 40)
                weed.GetComponent<SpriteRenderer>().flipX = true;
        }

        weedTick = Random.Range(30, 120);
    }

    private void OnMouseEnter()
    {
        if (TransientDataScript.IsTimeFlowing() && TransientDataScript.CameraView == CameraView.Garden && planterData != null)
        {
            if (planterData.isActive && planterData.seed != null) {
                TransientDataScript.PrintFloatText($"{Items.GetEmbellishedItemText(planterData.seed, false, false, false)}\n{planterData.progress}/{planterData.GetMaxGrowth()}");
            }
            else
            {
                TransientDataScript.PrintFloatText($"Empty planter");
            }
        }
    }

    private void OnMouseExit()
    {
        TransientDataScript.DisableFloatText();
    }
    private void OnMouseDown()
    {
        Debug.Log("Planter was clicked " + gameObject);

        if (TransientDataScript.IsTimeFlowing() && TransientDataScript.CameraView == CameraView.Garden && planterData != null)
        {
            planterManager.ClickPlanter(planterData);
        }
        else
        {
            Debug.Log("Time is flowing = " + TransientDataScript.IsTimeFlowing());
            Debug.Log("Camera is garden = " + (TransientDataScript.CameraView == CameraView.Garden) + ". Camera was " + TransientDataScript.CameraView);
            Debug.Log("PlanterData not null = " + (planterData != null));
        }
    }
}
