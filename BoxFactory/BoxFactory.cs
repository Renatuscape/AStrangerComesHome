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

    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    GameObject iconPrefab;
    [SerializeField]
    GameObject iconRowPrefab;
    [SerializeField]
    FontManager fontManager;

    void Start()
    {
        boxFactory = GetComponent<BoxFactory>();
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

    public static GameObject CreateItemRewardRow(Item item, float amount)
    {
        if (item == null)
        {
            Debug.Log("Item was null upon reaching Box Factory.");
        }
        return boxFactory.InstantiateIconRowPrefab(item, amount);
    }
    public static GameObject CreateItemIcon(Item item, bool displayInventoryAmount)
    {
        return null;
    }
}
