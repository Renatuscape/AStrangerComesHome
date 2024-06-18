using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GiftMenu : MonoBehaviour
{
    public Character character;
    public PortraitRenderer portraitRenderer;
    public PageinatedContainer pageinatedContainer;
    public TextMeshProUGUI giftingText;
    public GameObject selectedGiftContainer;
    public GameObject selectedGiftInfo;
    public Item selectedGift;
    public List<Item> availableInventory = new();
    public List<GiftItem> spawnedItems = new();

    private void OnDisable()
    {
        pageinatedContainer.ClearPrefabs();
    }

    public void Setup(Character character)
    {
        foreach (var giftItem in spawnedItems)
        {
            giftItem.itemData.Return("GiftMenu setup");
        }

        spawnedItems.Clear();
        availableInventory.Clear();

        this.character = character;
        portraitRenderer.EnableForGifting(character);
        selectedGiftInfo.SetActive(false);
        selectedGift = null;
        gameObject.SetActive(true);
        TransientDataScript.SetGameState(GameState.AlchemyMenu, name, gameObject);
        GetInventory();
        PrintInventory();

        giftingText.text = $"Choose a gift for {character.NamePlate()}";
    }

    void GetInventory()
    {
        foreach (Item item in Items.all)
        {
            if (item.type != ItemType.Script &&
                item.type != ItemType.Misc &&
                item.rarity != ItemRarity.Script &&
                item.rarity != ItemRarity.Unique)
            {
                if (Player.GetCount(item.objectID, name) > 0)
                {
                    availableInventory.Add(item);
                }
            }
        }
    }

    void PrintInventory()
    {
        var inventoryObjects = pageinatedContainer.Initialise(availableInventory, true, true, true);

        foreach (var obj in inventoryObjects)
        {

            var giftItem = obj.AddComponent<GiftItem>();
            var iconData = obj.GetComponent<ItemIconData>();
            spawnedItems.Add(giftItem);
            giftItem.itemData = iconData;
            giftItem.giftMenu = this;
        }
    }

    public void SelectGift(Item item)
    {
        selectedGift = item;
        selectedGiftInfo.SetActive(true);

        foreach (Transform child in selectedGiftContainer.transform)
        {
            Destroy(child.gameObject.GetComponent<GiftItem>());
            child.gameObject.GetComponent<ItemIconData>().Return("GiftMenu on SelectGift.");
        }

        var newChoice = BoxFactory.CreateItemIcon(item, false, 64);
        newChoice.transform.SetParent(selectedGiftContainer.transform, false);

        giftingText.text =$"Give {selectedGift.name} to {character.NamePlate()}?";
    }

    public void Gift()
    {
        selectedGiftInfo.SetActive(false);
        TransientDataScript.SetAsGifted(character);

        foreach (var giftItem in spawnedItems)
        {
            giftItem.disabled = true;
        }

        int appreciation = 1;

        if (character.giftsDislike.FirstOrDefault(i => i == selectedGift.objectID) != null)
        {
            LogAlert.QueueTextAlert($"{character.NamePlate()}\'Oh...\'");
            appreciation = 0;
        }
        else if (character.giftsLove.FirstOrDefault(i => i == selectedGift.objectID) != null)
        {
            LogAlert.QueueTextAlert($"{character.NamePlate()}\'I love this!\'");
            appreciation += 2;
        }
        else if (character.giftsLike.FirstOrDefault(i => i == selectedGift.objectID) != null || selectedGift.type == ItemType.Treasure)
        {
            LogAlert.QueueTextAlert($"{character.NamePlate()}\'Thank you!\'");
            appreciation += 1;
        }

        appreciation += (int)selectedGift.rarity;

        int current = Player.GetCount(character.objectID, name);

        if (current + appreciation > 0)
        {
            Player.Add(character.objectID, appreciation);
        }

        Player.Remove(selectedGift.objectID);
        giftingText.text = "";
    }


    public void Close()
    {
        foreach (var giftItem in spawnedItems)
        {
            giftItem.gameObject.GetComponent<ItemIconData>().Return("GiftMenu setup");
        }

        spawnedItems.Clear();
        availableInventory.Clear();

        gameObject.SetActive(false);
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }
}

public class GiftItem : MonoBehaviour, IPointerClickHandler
{
    public GiftMenu giftMenu;
    public ItemIconData itemData;
    public bool disabled;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!disabled)
        {
            giftMenu.SelectGift(itemData.item);
        }
        else
        {
            LogAlert.QueueTextAlert("It's unseemely to give another gift so soon.");
        }
    }
}