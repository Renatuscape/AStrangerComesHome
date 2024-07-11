using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourPicker : MonoBehaviour
{
    public DataManagerScript dataManager;
    public PlayerSprite playerSprite;
    public DialoguePlayerSprite playerIcon;

    public Image colourPreview;
    public TextMeshProUGUI hexText;

    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public Slider sliderA;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI transparencyText;

    public int targetIndex; //0 = name, 1 = hair, 2 = eyes, 3 = accessory, 4 = lip tint, 5 = hair accent

    private void OnEnable()
    {
        hexText.text = "Hex #" + dataManager.playerSprite.hairHexColour;
        AddListeners();

        if (targetIndex != 4)
        {
            sliderA.gameObject.SetActive(false);
        }

        if (targetIndex == 4)
        {
            SetSlidersFromHex(dataManager.playerSprite.lipTintHexColour);
            sliderA.value = dataManager.playerSprite.lipTintTransparency;

            sliderA.gameObject.SetActive(true);
            targetText.text = "Lip Tint Colour";
            transparencyText.text = Mathf.RoundToInt(sliderA.value * 100) + "%";
        }
        else if (targetIndex == 3)
        {
            SetSlidersFromHex(dataManager.playerSprite.accessoryHexColour);
            targetText.text = "Accessory Colour";
        }
        else if (targetIndex == 2)
        {
            SetSlidersFromHex(dataManager.playerSprite.eyesHexColour);
            targetText.text = "Eye Colour";
        }
        else if (targetIndex == 1)
        {
            SetSlidersFromHex(dataManager.playerSprite.hairHexColour);
            targetText.text = "Hair Colour";
        }
        else if (targetIndex == 0)
        {
            SetSlidersFromHex(dataManager.playerNameColour);
            targetText.text = "Name Colour";
        }
        else if (targetIndex == 5)
        {
            SetSlidersFromHex(dataManager.playerSprite.hairAccentHexColour);
            targetText.text = "Hair Accent";
        }

    }
    private void OnDisable()
    {
        sliderR.onValueChanged.RemoveAllListeners();
        sliderG.onValueChanged.RemoveAllListeners();
        sliderB.onValueChanged.RemoveAllListeners();
        sliderA.onValueChanged.RemoveAllListeners();
    }

    public void SetSlidersFromHex(string hexColor)
    {
        Debug.Log("SetSlidersFromHex called using index " + targetIndex + " and hex colour #" + hexColor);
        if (hexColor.Length >= 6)
        {
            var r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            // Convert the RGB values from the range of 0-255 to 0-1
            float rNormalized = r / 255f;
            float gNormalized = g / 255f;
            float bNormalized = b / 255f;

            if (r < sliderR.minValue || g < sliderG.minValue || b < sliderB.minValue)
            {
                Debug.Log($"An RGB value exceeded slider minimum value. {hexColor} was not accepted. Colour approximated.");
            }

            sliderR.value = rNormalized;
            sliderG.value = gNormalized;
            sliderB.value = bNormalized;
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
        sliderA.onValueChanged.AddListener(OnSliderChange);
    }

    private void OnSliderChange(float value)
    {
        if (targetIndex == 4)
        {
            GetSliderColour(ref dataManager.playerSprite.lipTintHexColour, true, out var colour);
            dataManager.playerSprite.lipTintTransparency = sliderA.value;
            playerSprite.lipTint.color = new Color (colour.r, colour.g, colour.b, sliderA.value);

            transparencyText.text = Mathf.RoundToInt(sliderA.value * 100) + "%";
        }
        else if (targetIndex == 3)
        {
            GetSliderColour(ref dataManager.playerSprite.accessoryHexColour, false, out var colour);
            playerSprite.playerHair.ApplyAccessoryColour(colour);
        }
        else if (targetIndex == 2)
        {
            AdjustImageColour(playerSprite.playerEyes.iris, ref dataManager.playerSprite.eyesHexColour);
        }
        else if (targetIndex == 1)
        {
            GetSliderColour(ref dataManager.playerSprite.hairHexColour, false, out var colour);
            playerSprite.playerHair.ApplyHairColour(colour);
        }
        else if (targetIndex == 0)
        {
            AdjustTextColour(nameText, ref dataManager.playerNameColour);
        }
        else if (targetIndex == 5)
        {
            GetSliderColour(ref dataManager.playerSprite.hairAccentHexColour, false, out var colour);
            playerSprite.playerHair.ApplyAccentColour(colour);
        }

        if (sliderR.value + sliderG.value + sliderB.value < 0.7f)
        {
            hexText.color = Color.white;
        }
        else
        {
            hexText.color = Color.black;
        }


        if (playerIcon != null)
        {
            // Replace with less expensive update menu in the future
            playerIcon.gameObject.SetActive(false);
            playerIcon.gameObject.SetActive(true);
        }
    }
    public void AdjustImageColour(Image targetImage, ref string storedHexString)
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
        string hexColour = ColorUtility.ToHtmlStringRGBA(adjustedColour);
        //Store and display hex value
        storedHexString = hexColour;
        hexText.text = "Hex #" + hexColour;
    }

    public void GetSliderColour(ref string storedHexString, bool enableAlpha, out Color adjustedColour)
    {
        //Apply adjustments based on slider values
        float red = sliderR.value;
        float green = sliderG.value;
        float blue = sliderB.value;
        float alpha = enableAlpha ? sliderA.value : 1;
        Debug.Log("Alpha was stored as " + alpha);

        //Create a new colour with adjusted RGB values
        adjustedColour = new Color(red, green, blue, alpha);

        //Apply new colour
        colourPreview.color = adjustedColour;

        //Convert colour to hex
        string hexColour = ColorUtility.ToHtmlStringRGBA(colourPreview.color);
        //Store and display hex value
        storedHexString = hexColour;

        hexText.text = "Hex #" + hexColour;
    }

    public void AdjustTextColour(TextMeshProUGUI text, ref string storedHexString)
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
    }
}
