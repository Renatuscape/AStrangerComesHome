using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SalvageBoxMechanics : MonoBehaviour
{
    private SpriteRenderer sprite;
    private BoxCollider2D objectCollider;
    private Animator anim;

    private float alphaSetting = 1;

    public List<Item> coins;
    public List<Item> junkItems;
    public List<Item> commonItems;
    public List<Item> uncommonItems;
    public List<Item> rareItems;

    public ItemIntPair slotA = null;
    public ItemIntPair slotB = null;
    public ItemIntPair slotC = null;
    public ItemIntPair slotD = null;
    public ItemIntPair slotE = null;

    public List<ItemIntPair> loot = new();

    public int fate;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        RollSlots();
        loot = ConsolidateLoot();
    }

    void LateUpdate()
    {
        if (transform.position.x <= -28 || transform.position.x >= 28)
        {
            Destroy(gameObject);
        }

        if (transform.position.x <= -28 || transform.position.x >= 28)
        {
            Destroy(gameObject);
        }
    }

    void RollSlots()
    {
        slotA = RollLoot();

        var slotRoll = Random.Range(0 + (fate * 2), 100);

        if (slotRoll >= 35)
        {
            slotB = RollLoot();
        }

        if (slotRoll >= 75)
        {
            slotC = RollLoot();
        }

        if (slotRoll >= 90)
        {
            slotD = RollLoot(true); //Higher rewards are guaranteed
        }

        if (slotRoll >= 99)
        {
            slotE = RollLoot(true); //Higher rewards are guaranteed
        }
    }

    ItemIntPair RollLoot(bool rareSlot = false)
    {
        var rarityRoll = Random.Range(0 + (fate * 2), 100);
        var coinRoll = Random.Range(0 + (fate * 2), 100);

        if (rareSlot)
        {
            rarityRoll += 30 + (fate * 2);
        }

        if (coinRoll < 65)
        {
            return RollCoins(rarityRoll);
        }
        else
        {
            return RollItems(rarityRoll);
        }
    }

    ItemIntPair RollCoins(int rarityRoll)
    {
        if (rarityRoll >= 95)
        {
            return new()
            {
                item = coins.Find(x => x.rarity == ItemRarity.Rare),
                amount = Random.Range(1, 5)
            };
        }
        else if (rarityRoll >= 80)
        {
            return new()
            {
                item = coins.Find(x => x.rarity == ItemRarity.Uncommon),
                amount = Random.Range(3, 20)
            };
        }
        else if (rarityRoll >= 60)
        {
            return new()
            {
                item = coins.Find(x => x.rarity == ItemRarity.Common),
                amount = Random.Range(5, 30)
            };
        }
        else
        {
            return new()
            {
                item = coins.Find(x => x.rarity == ItemRarity.Junk),
                amount = Random.Range(10, 50)
            };
        }
    }

    ItemIntPair RollItems(int rarityRoll)
    {
        if (rarityRoll >= 95)
        {
            return new()
            {
                item = rareItems[Random.Range(0, rareItems.Count)],
                amount = 1
            };
        }
        else if (rarityRoll >= 80)
        {
            return new()
            {
                item = uncommonItems[Random.Range(0, uncommonItems.Count)],
                amount = Random.Range(1, 3)
            };
        }
        else if (rarityRoll >= 50)
        {
            return new()
            {
                item = commonItems[Random.Range(0, commonItems.Count)],
                amount = Random.Range(1, 4)
            };
        }
        else
        {
            return new()
            {
                item = junkItems[Random.Range(0, junkItems.Count)],
                amount = Random.Range(1, 5)
            };
        }
    }

    List<ItemIntPair> ConsolidateLoot()
    {
        var lootList = new List<ItemIntPair>();
        var slots = new[] { slotA, slotB, slotC, slotD, slotE };

        foreach (var slot in slots)
        {
            if (slot.item != null)
            {
                var existingSlot = lootList.Find(existingSlot => existingSlot != slot && existingSlot.item == slot.item);

                if (existingSlot != null)
                {
                    Debug.Log($"Salvage loot consolidated {existingSlot.item.name} ({existingSlot.amount}) and {slot.item.name} ({slot.amount}) into one.");
                    existingSlot.amount += slot.amount;
                    slot.amount = 0;
                }
                else
                {
                    lootList.Add(slot);
                }
            }
        }

        if (slotB == null || slotB.amount == 0)
        {
            lootList.Remove(slotB);
        }
        if (slotC == null || slotC.amount == 0)
        {
            lootList.Remove(slotC);
        }
        if (slotD == null || slotD.amount == 0)
        {
            lootList.Remove(slotD);
        }

        return lootList;
    }   

    void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            Destroy(objectCollider);
            anim.SetTrigger("Active");


            foreach (ItemIntPair entry in loot)
            {
                if (entry.item != null && entry.amount > 0)
                {
                    Player.Add(entry.item.objectID, entry.amount);

                    TransientDataScript.PushAlert($"Found {entry.item.name} ({entry.amount})!");
                }
            }

            StartCoroutine(AlphaFade());
        }
    }

    IEnumerator AlphaFade()
    {
        yield return new WaitForSeconds(.08f);
        alphaSetting = alphaSetting - 0.05f;

        if (alphaSetting > 0)
            StartCoroutine(AlphaFade());

        else if (alphaSetting <= 0)
            Destroy(gameObject);
    }
}