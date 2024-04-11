using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageMenu : MonoBehaviour
{
    public Character character;
    public GameObject prefabContainer;
    public List<GameObject> prefabList;
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Initialise(Character character)
    {
        this.character = character;

        foreach (Upgrade upgrade in Upgrades.all)
        {
            var prefab = BoxFactory.CreateUpgradeIcon(upgrade, true, true);
            prefab.transform.SetParent(prefabContainer.transform, false);
            prefabList.Add(prefab);
        }
    }
}
