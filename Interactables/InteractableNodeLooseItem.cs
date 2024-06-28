using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InteractableNodeLooseItem : InteractableNode
{
    public bool checkForAnimation;
    public Item item;
    public InteractableBundle bundle;
    void Start()
    {
        bundle.bundleID = "WorldNodeLoot_" + gameObject.name + "_" + nodeID + "_" + (bundle.disableRespawn ? "disableRespawn" : "allowRespawn") + (bundle.exactCooldown ? "_ECD#" : "_CD#") + bundle.cooldown;
        rend = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        CheckLoot();
    }

    void CheckLoot()
    {
        if (bundle.CheckIfLootable())
        {
            item = Items.FindByID(nodeID);
            
            if (item != null)
            {
                rend.sprite = item.sprite;

                if (checkForAnimation)
                {
                    animatedSprite = AnimationLibrary.GetAnimatedObject(item.objectID);

                    if (animatedSprite != null)
                    {
                        // Start idle animation
                    }
                }
            }
        }
        else
        {
            gameObject.gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (bundle.CheckIfLootable())
        {
            col.enabled = false;
            ClaimLoot();
            bundle.SaveNodeToPlayer();
            StartCoroutine(FadeOutObject());
        }
    }
    
    void ClaimLoot()
    {
        Player.Add(item.objectID, bundle.alwaysMaxYield ? bundle.maxYieldPerItem : Random.Range(1, bundle.maxYieldPerItem + 1));
    }
}
