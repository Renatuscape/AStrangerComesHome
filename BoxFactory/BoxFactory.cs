using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxFactory : MonoBehaviour
{
    static BoxFactory boxFactory;
    public int fontBodySize;
    public int fontSubtitleSize;
    public int fontHeaderSize;
    public List<ItemIconData> itemIconPool;
    public List<GameObject> itemIconsOnLoan;

    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    GameObject iconPrefab;
    [SerializeField]
    GameObject iconRowPrefab;
    [SerializeField]
    GameObject iconRowPlainTextPrefab;
    [SerializeField]
    GameObject upgradePrefab;
    [SerializeField]
    FontManager fontManager;

    void Start()
    {
        boxFactory = GetComponent<BoxFactory>();

        GenerateItemIconPool(400);
    }

    GameObject InstantiateButton(string inputText, float width, TextAlignmentOptions textAlignment)
    {
        GameObject newButton = Instantiate(buttonPrefab);
        FormatText(newButton, inputText, textAlignment);
        SetWidth(newButton, width);

        return newButton;
    }

    GameObject InstantiateBodyText(string inputText, float width)
    {
        GameObject newBodyText = InstantiateButton(inputText, width, TextAlignmentOptions.Left);

        //Remove button component
        Destroy(newBodyText.GetComponent<Button>());

        //Hide background
        newBodyText.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        return newBodyText;
    }

    GameObject InstantiateUpgradeIcon(Upgrade upgrade, bool displayLevel, bool displayPrice, bool showFloatName)
    {
        var prefab = Instantiate(upgradePrefab);
        var script = prefab.GetComponent<UpgradeIcon>();

        script.Setup(upgrade, displayLevel, displayPrice, showFloatName);

        return prefab;
    }

    void GenerateItemIconPool(int instances)
    {
        for (int i = 0; i < instances; i++)
        {
            GameObject iconInstance = Instantiate(iconPrefab);

            var iconData = iconInstance.AddComponent<ItemIconData>();

            var tag = iconInstance.transform.Find("Tag").gameObject;
            var price = iconInstance.transform.Find("PriceTag").gameObject;

            iconData.countText = tag.transform.GetComponentInChildren<TextMeshProUGUI>();
            iconData.priceText = price.transform.GetComponentInChildren<TextMeshProUGUI>();

            iconData.images = iconInstance.transform.Find("ImageContainer").GetComponentsInChildren<Image>();

            foreach (Image image in iconData.images)
            {
                if (image.name.ToLower().Contains("shadow"))
                {
                    iconData.itemShadow = image;
                }
                else
                {
                    iconData.itemSprite = image;
                }
            }

            iconData.gameObject.transform.SetParent(gameObject.transform, false);
            iconData.gameObject.SetActive(false);
            itemIconPool.Add(iconData);
        }
    }

    ItemIconData InstantiateItemIcon(Item item, bool displayInventoryAmount, int size = 32, int fontSize = 14)
    {
        ItemIconData itemIcon = itemIconPool[0];
        itemIconPool.RemoveAt(0);

        itemIcon.item = item;

        foreach (Image image in itemIcon.images)
        {
            image.sprite = item.sprite;
        }

        if (!displayInventoryAmount || item.rarity == ItemRarity.Unique)
        {
            itemIcon.countText.transform.parent.gameObject.SetActive(false);
        }
        else if (displayInventoryAmount)
        {
            itemIcon.countText.transform.parent.gameObject.SetActive(true);
            itemIcon.countText.text = $"{Player.GetCount(item.objectID, name)}";
            itemIcon.countText.fontSize = fontSize;
        }

        RectTransform transform = itemIcon.gameObject.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(size, size);
        itemIcon.gameObject.name = item.objectID + "_Icon";
        itemIcon.gameObject.SetActive(true);

        boxFactory.itemIconsOnLoan.Add(itemIcon.gameObject);
        itemIcon.EnablePrice(false, 0);
        return itemIcon;
    }

    GameObject InstantiateIconRowPrefab(Item item, float amount)
    {
        GameObject newButton = Instantiate(iconRowPrefab);
        FormatText(newButton, $"({amount}) {item.name}", TextAlignmentOptions.Left);

        Image[] images = newButton.transform.Find("ImageContainer").GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            image.sprite = item.sprite;
        }

        return newButton;
    }

    private GameObject InstantiateIconRowPlainTextPrefab(Item item, float amount, bool displayAmount)
    {
        GameObject newButton = Instantiate(iconRowPlainTextPrefab);
        //FormatText(newButton, $"{item.name}{(displayAmount ? $" - {amount}" : "")}", TextAlignmentOptions.Left);

        TextMeshProUGUI[] buttonText = newButton.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI text in buttonText)
        {
            if (text.gameObject.name == "AmountText")
            {
                if (displayAmount)
                {
                    text.text = ParseText(amount.ToString());
                }
                else
                {
                    text.gameObject.SetActive(false);
                }
            }
            else
            {
                text.text = ParseText(item.name);
            }
        }

        Image[] images = newButton.transform.Find("ImageContainer").GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            image.sprite = item.sprite;
        }

        return newButton;
    }

    // FORMATTING OPTIONS
    string ParseText(string inputText)
    {
        return DialogueTagParser.ParseText(inputText);
    }
    void SetWidth(GameObject gameObject, float width)
    {
        float newSize = width;
        RectTransform transform = gameObject.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(newSize, transform.sizeDelta.y);
    }

    void FormatText(GameObject gameObject, string inputText, TextAlignmentOptions textAlignment, bool isBody = true, bool isSubtitle = false, bool isHeader = false)
    {
        if (gameObject == null)
        {
            Debug.Log("GameObject was null when arriving at FormatText.");
        }

        TextMeshProUGUI buttonText = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            if (isBody)
            {
                buttonText.fontSize = fontBodySize + GlobalSettings.TextSize;
                buttonText.font = fontManager.body.font;
            }
            else if (isSubtitle)
            {
                buttonText.fontSize = fontSubtitleSize;
                buttonText.font = fontManager.subtitle.font;
            }
            else if (isHeader)
            {
                buttonText.fontSize = fontHeaderSize;
                buttonText.font = fontManager.header.font;
            }

            buttonText.text = ParseText(inputText);
            buttonText.alignment = textAlignment;
        }
        else
        {
            Debug.Log("Format Text could not find textmesh in game object children.");
        }
    }


    //STATIC METHODS
    public static GameObject CreateButton(string inputText, float width = 145)
    {
        var newButton = boxFactory.InstantiateButton(inputText, width, TextAlignmentOptions.Center);

        return newButton;
    }

    public static GameObject CreateTextBox(string inputText, float width)
    {
        var newButton = boxFactory.InstantiateBodyText(inputText, width);

        return newButton;
    }

    public static GameObject CreateItemRow(Item item, float amount)
    {
        if (item == null)
        {
            Debug.Log("Item was null upon reaching Box Factory.");
        }
        return boxFactory.InstantiateIconRowPrefab(item, amount);
    }

    public static GameObject CreateItemRowPlainText(Item item, float amount, bool displayAmount)
    {
        if (item == null)
        {
            Debug.Log("Item was null upon reaching Box Factory.");
        }
        return boxFactory.InstantiateIconRowPlainTextPrefab(item, amount, displayAmount);
    }

    public static ItemIconData CreateItemIcon(Item item, bool displayInventoryAmount, int size = 32, int fontSize = 14)
    {
        return boxFactory.InstantiateItemIcon(item, displayInventoryAmount, size, fontSize);
    }

    public static GameObject CreateUpgradeIcon(Upgrade upgrade, bool displayLevel, bool displayPrice, bool showFloatName)
    {
        return boxFactory.InstantiateUpgradeIcon(upgrade, displayLevel, displayPrice, showFloatName);
    }

    public static void ReturnItemIconToPool(ItemIconData itemIconData, string caller)
    {
        // Debug.Log(itemIconData.gameObject.name + " itemIcon was successfully returned by " + caller);
        itemIconData.gameObject.transform.SetParent(boxFactory.gameObject.transform, false);
        itemIconData.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        itemIconData.gameObject.SetActive(false);

        boxFactory.itemIconPool.Add(itemIconData);
        boxFactory.itemIconsOnLoan.Remove(itemIconData.gameObject);
    }
}
