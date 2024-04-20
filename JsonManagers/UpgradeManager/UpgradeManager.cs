using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;
    public void StartLoading()
    {
        gameObject.SetActive(true);
        LoadFromJson("Upgrades.json");
    }

    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Upgrade[] upgrades;
    }

    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Upgrades/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            ItemsWrapper dataWrapper = JsonUtility.FromJson<ItemsWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.upgrades != null)
                {
                    foreach (Upgrade skill in dataWrapper.upgrades)
                    {
                        InitialiseUpgrade(skill, Upgrades.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All UPGRADES successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Upgrades array is null in JSON data. Check that the list has a wrapper with the \'upgrades\' tag.");
                }
            }
            else
            {
                Debug.LogError("JSON data is malformed. No wrapper found?");
                Debug.Log(jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + jsonPath);
        }
    }

    //ITEM DISPLAY TEST
    public GameObject itemDisplayer;
    public Transform canvasTransform;
    public void DisplayItemSprite(Upgrade item, GameObject prefab, Transform parentTransform)
    {
        GameObject newItem = Instantiate(prefab, parentTransform);
        newItem.name = item.name;
        Image imageComponent = newItem.GetComponent<Image>();
        imageComponent.sprite = item.sprite;
    }

    public static void InitialiseUpgrade(Upgrade upgrade, List<Upgrade> upgradeList)
    {
        upgrade.objectType = ObjectType.Upgrade;
        upgrade.maxValue = StaticGameValues.maxUpgradeValue;

        ItemIDReader(ref upgrade);
        upgradeList.Add(upgrade);
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

