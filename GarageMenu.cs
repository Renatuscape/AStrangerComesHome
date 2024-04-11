using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageMenu : MonoBehaviour
{
    public Character character;
    public GameObject mechUpContainer;
    public GameObject magicUpContainer;
    public List<UpgradeIcon> prefabList;
    bool upgradesLoaded = false;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Initialise(Character character)
    {
        this.character = character;

        if (!upgradesLoaded)
        {
            foreach (Upgrade upgrade in Upgrades.all)
            {
                var prefab = BoxFactory.CreateUpgradeIcon(upgrade, true, true);

                if (upgrade.type == UpgradeType.Magical)
                {
                    prefab.transform.SetParent(magicUpContainer.transform, false);
                }
                else
                {
                    prefab.transform.SetParent(mechUpContainer.transform, false);
                }

                prefabList.Add(prefab.GetComponent<UpgradeIcon>());
            }

            upgradesLoaded = true;
        }
        else
        {
            foreach (var upgrade in prefabList)
            {
                upgrade.UpdateText();
            }
        }
    }
}
