using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourPicker : MonoBehaviour
{
    public DataManagerScript dataManager;
    public PlayerSprite playerSprite;
    public PlayerIconPrefab playerIcon;

    public Image colourPreview;
    public TextMeshProUGUI hexText;

    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI targetText;

    public int targetIndex; //0 = name, 1 = hair, 2 = eyes

    private void OnEnable()
    {
        hexText.text = "Hex #" + dataManager.hairHexColour;
        AddListeners();

        if (targetIndex == 2)
        {
            SetSlidersFromHex(dataManager.eyesHexColour);
        }
        else if (targetIndex == 1)
        {
            SetSlidersFromHex(dataManager.hairHexColour);
        }
        else if (targetIndex == 0)
        {
            SetSlidersFromHex(dataManager.playerNameColour);
        }

    }
    private void OnDisable()
    {
        sliderR.onValueChanged.RemoveAllListeners();
        sliderG.onValueChanged.RemoveAllListeners();
        sliderB.onValueChanged.RemoveAllListeners();
    }

    public void SetSlidersFromHex(string hexColor)
    {
        //Debug.Log("SetSlidersFromHex called using index " + targetIndex + "and hex colour #" + hexColor);
        if (hexColor.Length >= 6)
        {
            var r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // Convert the RGB values from the range of 0-255 to 0-1
            float rNormalized = r / 255f;
            float gNormalized = g / 255f;
            float bNormalized = b / 255f;

            if (r > sliderR.minValue || g > sliderG.minValue ||  b > sliderB.minValue)
            {
                Debug.Log("An RGB value exceeded slider minimum value. Colour approximated.");
            }
            sliderR.value = rNormalized;
            sliderG.value = gNormalized;
            sliderB.value = bNormalized;

            Debug.Log($"Sliders should be set to hex colour {hexColor}");
        }
        else
        {
            Debug.Log("No hex colour detected: " + hexColor);
        }
    }

    public void AddListeners()
    {
        sliderR.onValueChanged.AddListener(OnSliderChange);
        sliderG.onValueChanged.AddListener(OnSliderChange);
        sliderB.onValueChanged.AddListener(OnSliderChange);
    }

    private void OnSliderChange(float value)
    {
        if (targetIndex == 2)
        {
            AdjustImageColour(playerSprite.irises, ref dataManager.eyesHexColour, "Eye Colour");
        }

        else if (targetIndex == 1)
        {
            AdjustImageColour(playerSprite.hairColour, ref dataManager.hairHexColour, "Hair Colour");
        }
        else if (targetIndex == 0)
        {
            AdjustTextColour(nameText, ref dataManager.playerNameColour, "Name Colour");
        }

        if(sliderR.value + sliderG.value + sliderB.value < 0.7f)
        {
            hexText.color = Color.white;
        }
        else
            hexText.color = Color.black;

        if (playerIcon is not null)
        {
            playerIcon.UpdateImages();
        }
    }
    public void AdjustImageColour(Image targetImage, ref string storedHexString, string targetName)
    {
        //Get the current colour of the target image
        Color origColor = targetImage.color;

        //Apply adjustments based on slider values
        float red = sliderR.value;
        float green = sliderG.value;
        float blue = sliderB.value;

        //Create a new colour with adjusted RGB values
        Color adjustedColour = new Color(red, green, blue, origColor.a);

        //Apply new colour
        targetImage.color = adjustedColour;
        colourPreview.color = adjustedColour;

        //Convert colour to hex
        string hexColour = ColorUtility.ToHtmlStringRGBA(colourPreview.color);
        //Store and display hex value
        storedHexString = hexColour;
        hexText.text = "Hex #" + hexColour;
        targetText.text = targetName;
    }

    public void AdjustTextColour(TextMeshProUGUI text, ref string storedHexString, string targetName)
    {
        //Get the current colour of the target image
        Color origColor = text.color;

        //Apply adjustments based on slider values
        float red = sliderR.value;
        float green = sliderG.value;
        float blue = sliderB.value;

        //Create a new colour with adjusted RGB values
        Color adjustedColour = new Color(red, green, blue, origColor.a);

        //Apply new colour
        text.color = adjustedColour;
        colourPreview.color = adjustedColour;

        //Convert colour to hex
        string hexColour = ColorUtility.ToHtmlStringRGBA(colourPreview.color);
        //Store and display hex value
        storedHexString = hexColour;
        hexText.text = "Hex #" + hexColour;
        targetText.text = targetName;
    }
}
