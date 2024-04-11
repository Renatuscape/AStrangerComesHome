using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageMenu : MonoBehaviour
{
    public PortraitRenderer portraitRenderer;
    public Character character;
    public GameObject mechUpContainer;
    public GameObject magicUpContainer;
    public List<UpgradeIcon> prefabList;
    bool upgradesLoaded = false;

    void Awake()
    {
        if (TransientDataScript.GameState == GameState.MainMenu)
        {
            gameObject.SetActive(false);
        }
    }

    public void Initialise(Character character)
    {
        Debug.Log("Initialising garage menu.");
        this.character = character;
        TransientDataCalls.SetGameState(GameState.ShopMenu, name, gameObject);
        portraitRenderer.EnableForGarage(character.objectID);
        gameObject.SetActive(true);
        Debug.Log(gameObject);

        if (!upgradesLoaded)
        {
            foreach (Upgrade upgrade in Upgrades.all)
            {
                var prefab = BoxFactory.CreateUpgradeIcon(upgrade, true, true, true);

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

    public void Close()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }
}
