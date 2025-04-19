using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public void DisplayItemSprite(Upgrade item, GameObject prefab, Transform parentTransform)
    {
        GameObject newItem = Instantiate(prefab, parentTransform);
        newItem.name = item.name;
        Image imageComponent = newItem.GetComponent<Image>();
        imageComponent.sprite = item.sprite;
    }

    public static void Initialise(Upgrade upgrade)
    {
        upgrade.objectType = ObjectType.Upgrade;
        upgrade.maxValue = StaticGameValues.maxUpgradeValue;

        ItemIDReader(ref upgrade);
    }

    public static void ItemIDReader(ref Upgrade upgrade)
    {
        upgrade.type = TypeFinder(ref upgrade.objectID);
        upgrade.image = ImageFinder(ref upgrade.objectID);

        if (upgrade.image != null)
        {
            upgrade.sprite = SpriteCreator(ref upgrade.image);
        }
        else
        {
            Debug.LogError($"{upgrade.objectID}.image was null. Could not create sprite.");
        }
    }
    public static UpgradeType TypeFinder(ref string objectID)
    {
        if (objectID.Contains("MEC"))
        {
            return UpgradeType.Mechanical;
        }
        else //if (objectID.Contains("MAG"))
        {
            return UpgradeType.Magical;
        }
    }

    public static Texture2D ImageFinder(ref string objectID)
    {
        string fileDirectory = Application.streamingAssetsPath + "/Sprites/Upgrades/";
        string filePath = fileDirectory + objectID.Substring(0, 6) + ".png";
        Texture2D imageTexture;

        if (!File.Exists(filePath))
        {
            //Debug.LogWarning($"Image not found for {objectID}. Using default.");
            filePath = fileDirectory + objectID.Substring(0, 3) + "000.png";

            if (!File.Exists(filePath))
            {
                Debug.LogError($"Default image not found for type {objectID.Substring(0, 3)}! No image set for {objectID}");
                return null;
            }
        }

        byte[] imageBytes = File.ReadAllBytes(filePath);
        imageTexture = new Texture2D(2, 2); // Create an empty texture
        imageTexture.filterMode = FilterMode.Point; // Set filter mode to Point for pixel-perfect clarity. PREVENTS BLURRINESS
        imageTexture.LoadImage(imageBytes); // Load the image bytes

        return imageTexture;
    }

    public static Sprite SpriteCreator(ref Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

