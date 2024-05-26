using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionsTreasures : MonoBehaviour
{
    public GameObject treasureContainer;
    public GameObject mysteryPrefab;
    private void OnEnable()
    {
        foreach (Transform child in treasureContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in Items.all)
        {
            if (item.type == ItemType.Treasure)
            {

                if (!Player.GetEntry(item.objectID, name, out var x))
                {
                    var prefab = BoxFactory.CreateItemIcon(item, false, 64, 14, false);
                    prefab.transform.SetParent(treasureContainer.transform, false);

                    Image[] images = prefab.transform.Find("ImageContainer").GetComponentsInChildren<Image>();

                    foreach (Image image in images)
                    {
                        if (!image.name.ToLower().Contains("shadow"))
                        {
                            image.color = Color.black;
                            var mystery = Instantiate(mysteryPrefab);
                            mystery.transform.SetParent(image.gameObject.transform, false);
                        }
                    }
                }
                else
                {
                    var prefab = BoxFactory.CreateItemIcon(item, false, 64, 14, true);
                    prefab.transform.SetParent(treasureContainer.transform, false);

                    var script = prefab.GetComponent<ItemIconData>();
                    script.printRarity = true;
                }
            }
        }
    }
}
