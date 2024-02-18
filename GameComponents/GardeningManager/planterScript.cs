using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planterScript : MonoBehaviour
{
    public DataManagerScript dataManager;
    public GardenManager gardenManager;
    public BoxCollider2D boxCollider;
    public WhichPlanter thisPlanter;
    public GameObject weedsPrefab;
    public Skill overgrowth;

    public int currentWeeds;
    public int maxWeeds = 5;
    public float weedTimer;
    public int weedTick;

    void Awake()
    {
        gardenManager = transform.parent.GetComponent<GardenManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        weedTick = Random.Range(30, 120);
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView == CameraView.Garden)
        {
            if (currentWeeds == 0)
            {
                Debug.Log($"{thisPlanter} click registered.");
                gardenManager.ClickPlanter(thisPlanter);
            }
            else
            {
                Debug.Log("I need to clear out these weeds first!");
            }
        }

    }

    public void FixedUpdate()
    {
        weedTimer += Time.fixedDeltaTime;
        if (weedTimer >= weedTick)
        {
            weedTimer = 0;

            if (currentWeeds < maxWeeds)
                WeedTick();
        }
    }

    private void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            if (TransientDataScript.CameraView == CameraView.Garden)
            {
                if (currentWeeds == 0)
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

    void WeedTick()
    {
        var spawnChance = Random.Range(0, 100);

        if (spawnChance > 40)
        {
            currentWeeds++;
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
