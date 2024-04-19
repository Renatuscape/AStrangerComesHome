using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GiftMenu : MonoBehaviour
{
    public Character character;
    public PortraitRenderer portraitRenderer;
    public GameObject inventoryContainer;
    public TextMeshProUGUI giftingText;
    public GameObject selectedGiftContainer;
    public GameObject selectedGiftInfo;
    public Item selectedGift;
    public List<Item> availableInventory = new();
    public List<Button> buttons;
    public List<ItemType> displayTypes;
    public List<GiftItem> spawnedItems = new();

    private void Start()
    {
        ItemType[] allItemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));

        foreach (Button button in buttons)
        {
            if (!button.gameObject.name.Contains("All"))
            {
                // Capture the button and local copy of buttonType
                Button capturedButton = button;
                ItemType buttonType = allItemTypes.FirstOrDefault(t => capturedButton.gameObject.name.Contains(t.ToString()));

                // Add listener with local copy of buttonType
                button.onClick.AddListener(() => ToggleType(button, buttonType));

                Debug.Log($"{capturedButton.gameObject.name} button was assigned type {buttonType}");
            }
            else
            {
                button.onClick.AddListener(() => BtnShowAll());
            }
        }

        BtnShowAll();
    }
    public void Setup(Character character)
    {
        foreach (var giftItem in spawnedItems)
        {
            Destroy(giftItem.gameObject);
        }

        spawnedItems.Clear();
        availableInventory.Clear();

        this.character = character;
        portraitRenderer.EnableForGifting(character);
        selectedGiftInfo.SetActive(false);
        selectedGift = null;
        gameObject.SetActive(true);
        inventoryContainer.SetActive(true);
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

        availableInventory = availableInventory.OrderBy(i => i.type)
                                               .ThenBy(i => i.basePrice)
                                               .ThenBy(i => i.name)
                                               .ToList();
    }

    void PrintInventory()
    {
        foreach (Item item in  availableInventory)
        {
            var newItem = BoxFactory.CreateItemIcon(item, true, 64, 18);
            newItem.transform.SetParent(inventoryContainer.transform, false);

            var giftItem = newItem.AddComponent<GiftItem>();
            spawnedItems.Add(giftItem);
            giftItem.item = item;
            giftItem.giftMenu = this;
        }
        ApplyFilter();
    }

    void ApplyFilter()
    {
        foreach (var giftItem in spawnedItems)
        {
            if (displayTypes.Contains(giftItem.item.type))
            {
                giftItem.gameObject.SetActive(true);
            }
            else
            {
                giftItem.gameObject.SetActive(false);
            }
        }
    }

    public void SelectGift(Item item)
    {
        selectedGift = item;
        selectedGiftInfo.SetActive(true);

        foreach (Transform child in selectedGiftContainer.transform)
        {
            Destroy(child.gameObject);
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
            TransientDataScript.PushAlert($"{character.NamePlate()}\'Oh...\'");
            appreciation = 0;
        }
        else if (character.giftsLove.FirstOrDefault(i => i == selectedGift.objectID) != null)
        {
            TransientDataScript.PushAlert($"{character.NamePlate()}\'I love this!\'");
            appreciation += 2;
        }
        else if (character.giftsLike.FirstOrDefault(i => i == selectedGift.objectID) != null || selectedGift.type == ItemType.Treasure)
        {
            TransientDataScript.PushAlert($"{character.NamePlate()}\'Thank you!\'");
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

    public void BtnShowAll()
    {
        displayTypes = new();

        ItemType[] allItemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));

        foreach (ItemType type in allItemTypes)
        {
            Debug.Log($"Toggle all types found {type}");
            if (type != ItemType.Script && type != ItemType.Misc)
            {
                displayTypes.Add(type);
            }
        }

        foreach (Button button in buttons)
        {
            button.gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
        }

        ApplyFilter();
    }

    public void ToggleType(Button button, ItemType type)
    {
        if (displayTypes.Contains(type))
        {
            button.gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
            displayTypes.RemoveAt(displayTypes.IndexOf(type));
        }
        else
        {
            button.gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
            displayTypes.Add(type);
        }

        Canvas.ForceUpdateCanvases();
        ApplyFilter();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }
}

public class GiftItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GiftMenu giftMenu;
    public Item item;
    public bool disabled;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!disabled)
        {
            giftMenu.SelectGift(item);
        }
        else
        {
            TransientDataScript.PushAlert("It's unseemely to give another gift so soon.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TransientDataScript.PrintFloatEmbellishedItem(item, true, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TransientDataScript.DisableFloatText();
    }
}