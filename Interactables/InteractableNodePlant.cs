using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNodePlant : MonoBehaviour
{
    public string plantNodeID;
    public InteractableBundlePlant plantBundle;
    public bool disableSproutOnFail;
    public bool disableVesselOnFail;
    public bool disableVesselOnLoot;
    public SpriteRenderer plantRenderer;
    public List<SpriteRenderer> vesselRenderers;
    public BoxCollider2D col;
    bool isEnabled;
    void Start()
    {
        col = GetComponent<BoxCollider2D>();

        plantNodeID = "WorldNodeLoot_" +  gameObject.name;

        if (plantBundle != null && plantBundle.CheckBundle(plantNodeID))
        {
            if (!disableSproutOnFail && !disableVesselOnFail) // avoid stupid situations where the plant has grown fully but fails to spawn
            {
                plantBundle.spawnChance = 100;
            }

            plantBundle.Setup();
            plantRenderer.sprite = plantBundle.plant.sprite;
            isEnabled = true;
        }
        else
        {
            Debug.Log($"Plant bundle failed checks for {plantNodeID}. PlantBundle != null is {plantBundle != null}.");
            col.enabled = false;

            if (disableSproutOnFail)
            {
                plantRenderer.enabled = false;
            }
            if (disableVesselOnFail)
            {
                plantRenderer.enabled = false;

                foreach (var rend in  vesselRenderers)
                {
                    rend.enabled = false;
                }
            }

            if (!disableVesselOnFail && !disableSproutOnFail)
            {
                SetSprout();
            }
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked " + plantNodeID);

        if (plantBundle != null && isEnabled)
        {
            Debug.Log("Attempting to yield loot.");
            ParticleFactory.ClickEffectWeeds(gameObject);

            col.enabled = false;
            plantBundle.YieldLoot();
            plantRenderer.enabled = false;

            if (disableVesselOnLoot)
            {
                Debug.Log("Attempting to disable vessel sprites.");

                foreach (var rend in vesselRenderers)
                {
                    StartCoroutine(FadeOutObject(rend));
                }
            }
            else
            {
                Debug.Log("Attempting to set sprout.");
                SetSprout();
            }
        }
        Debug.Log($"PlantBundle was {(plantBundle != null ? "not null" : "null")} and isEnabled was {isEnabled}.");
    }

    IEnumerator FadeOutObject(SpriteRenderer rend)
    {
        float alpha = rend.color.a;
        float fadeAmount = 0.001f;

        while (rend.color.a > 0)
        {
            alpha -= fadeAmount;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            fadeAmount += 0.001f;
            yield return new WaitForSeconds(0.01f);
        }

        gameObject.SetActive(false);
    }

    void SetSprout()
    {
        int maxGrowth = plantBundle.cooldown;
        int progress = plantBundle.playerEntry != null ? plantBundle.playerEntry.amount : 0;

        Item seed = plantBundle.seed;

        if (progress < maxGrowth * 0.3f)
        {
            plantRenderer.sprite = SpriteFactory.GetSprout(seed.objectID, 1);
        }
        else if (progress > maxGrowth * 0.3f && progress < maxGrowth * 0.6f)
        {
            plantRenderer.sprite = SpriteFactory.GetSprout(seed.objectID, 2);
        }
        else if (progress > maxGrowth * 0.6f)
        {
            plantRenderer.sprite = SpriteFactory.GetSprout(seed.objectID, 3);
        }
    }
}
