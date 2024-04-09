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
    public List<Item> availableInventory;
    public List<Button> buttons;
    public List<ItemType> displayTypes;

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
                button.onClick.AddListener(() => ToggleType(buttonType));

                Debug.Log($"{capturedButton.gameObject.name} button was assigned type {buttonType}");
            }
            else
            {
                button.onClick.AddListener(() => BtnShowAll());
            }
        }
    }
    public void Setup(Character character)
    {
        this.character = character;
        portraitRenderer.EnableForGifting(character);
        selectedGiftInfo.SetActive(false);
        selectedGift = null;
        gameObject.SetActive(true);
        TransientDataCalls.SetGameState(GameState.Dialogue, name, gameObject);
        GetInventory();
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

    }

    void ApplyFilter()
    {

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

        ApplyFilter();
    }

    public void ToggleType(ItemType type)
    {
        if (displayTypes.Contains(type))
        {
            displayTypes.RemoveAt(displayTypes.IndexOf(type));
        }
        else
        {
            displayTypes.Add(type);
        }

        ApplyFilter();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }
}

public class GiftItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TransientDataCalls.PrintFloatText($"{item.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TransientDataCalls.DisableFloatText();
    }
}