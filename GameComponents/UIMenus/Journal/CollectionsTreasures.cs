using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionsTreasures : MonoBehaviour
{
    public GameObject treasureContainer;
    public GameObject mysteryPrefab;
    public List<ItemIconData> itemIcons = new();
    public List<GameObject> instantiatedMysteries = new();

    private void Awake()
    {
        foreach (Transform child in treasureContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnEnable()
    {
        CleanAndReturnPrefabs();

        foreach (Item item in Items.all)
        {
            if (item.type == ItemType.Treasure)
            {

                var prefab = BoxFactory.CreateItemIcon(item, false, 64, 14);
                prefab.transform.SetParent(treasureContainer.transform, false);
                var iconData = prefab.GetComponent<ItemIconData>();

                if (!Player.GetEntry(item.objectID, name, out var x))
                {

                    iconData.itemSprite.color = Color.black;
                    var mystery = Instantiate(mysteryPrefab);
                    mystery.name = "mystery";
                    mystery.transform.SetParent(iconData.itemSprite.gameObject.transform, false);
                    instantiatedMysteries.Add(mystery);

                    iconData.disableFloatText = true;
                }
                else
                {

                    var script = prefab.GetComponent<ItemIconData>();
                    script.printRarity = true;
                    iconData.disableFloatText = false;
                }

                itemIcons.Add(prefab);
            }
        }
    }

    private void OnDisable()
    {
        CleanAndReturnPrefabs();
    }

    void CleanAndReturnPrefabs()
    {
        foreach (var itemIconData in itemIcons)
        {
            itemIconData.itemSprite.color = Color.white;
            itemIconData.disableFloatText = false;

            itemIconData.Return("CollectionsTreasures in onEnable");
        }

        itemIcons.Clear();

        if (instantiatedMysteries.Count > 0)
        {
            for (int i = instantiatedMysteries.Count - 1; i >= 0; i--)
            {
                Destroy(instantiatedMysteries[i]);
            }
        }

        instantiatedMysteries.Clear();
    }
}
