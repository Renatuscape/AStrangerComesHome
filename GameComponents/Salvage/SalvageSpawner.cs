using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SalvageSpawner : MonoBehaviour
{
    TransientDataScript transientData;
    public GameObject salvageBox;
    public int fate;

    public int salvageRNG;
    public float waitingTime;
    public float timer;
    public List<Item> coins = new();
    public List<Item> junkItems = new();
    public List<Item> commonItems = new();
    public List<Item> uncommonItems = new();
    public List<Item> rareItems = new();
    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        timer = 0;
        waitingTime = 300;
    }
    private void Update()
    {
        if ((TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.MapMenu) && transientData.currentLocation == null)
        {
            timer += Time.deltaTime;

            if (timer >= waitingTime)
            {
                SyncSkills();
                timer = 0;
                waitingTime = Random.Range(120, 500 - (fate * 10));
                SalvageSpawn();
            }
        }
    }

    void SyncSkills()
    {
        fate = Player.GetCount(StaticTags.Fate, "SalvageSpawner");
    }

    void SalvageSpawn()
    {
        if (GameObject.Find("spawnedSalvage") == null)
        {
            if (junkItems.Count == 0 || commonItems.Count == 0 || uncommonItems.Count == 0 || rareItems.Count == 0 || coins.Count == 0)
            {
                PopulateItemList();
            }

            var newSalvage = Instantiate(salvageBox);
            newSalvage.transform.localPosition = new Vector3(-20f, -6f, 0);
            newSalvage.name = "spawnedSalvage";

            var salvageScript = newSalvage.GetComponent<SalvageBoxMechanics>();

            salvageScript.fate = fate;

            salvageScript.coins = coins;
            salvageScript.junkItems = junkItems;
            salvageScript.commonItems = commonItems;
            salvageScript.uncommonItems = uncommonItems;
            salvageScript.rareItems = rareItems;

            transientData.activePrefabs.Add(newSalvage);
            ParallaxControllerHelper.AddObjectToParallax(newSalvage, "Road", -30, 30);
        }
    }

    void PopulateItemList()
    {
        junkItems = Items.all.FindAll(x => x.rarity == ItemRarity.Junk && x.type != ItemType.Misc && x.type != ItemType.Catalyst && x.type != ItemType.Plant);
        commonItems = Items.all.FindAll(x => x.rarity == ItemRarity.Common && !x.notBuyable && !x.notSellable && x.type != ItemType.Misc && x.type != ItemType.Catalyst && x.type != ItemType.Plant);
        uncommonItems = Items.all.FindAll(x => x.rarity == ItemRarity.Uncommon && !x.notBuyable && !x.notSellable && x.type != ItemType.Misc && x.type != ItemType.Catalyst && x.type != ItemType.Plant);
        rareItems = Items.all.FindAll(x => x.rarity == ItemRarity.Rare && !x.notBuyable && !x.notSellable && x.type != ItemType.Misc && x.type != ItemType.Catalyst && x.type != ItemType.Plant);

        coins.Add(Items.all.FirstOrDefault(x => x.objectID == "MIS000"));
        coins.Add(Items.all.FirstOrDefault(x => x.objectID == "MIS001"));
        coins.Add(Items.all.FirstOrDefault(x => x.objectID == "MIS002"));
        coins.Add(Items.all.FirstOrDefault(x => x.objectID == "MIS003"));
        coins.Add(Items.all.FirstOrDefault(x => x.objectID == "MIS010"));
    }
}
