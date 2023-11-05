using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEditor;

[System.Serializable]
public struct ItemStruct
{
    public string itemID;
    public string name;
    public int basePrice; //automatically calculated from type, rarity and ID
    public ItemType type; //retrieve from ID
    public ItemRarity rarity; //retrieve from ID
    public Texture2D image; //retrieve from ID + folder
    public Sprite sprite;
    public List<Texture2D> animationFrames; //for animation frames
    public string description;
    public bool notBuyable; //from ID, second to last letter N/B (not/buyable)
    public bool notSellable; //from ID, last letter N/S (not/sellable)

    //SEEDS
    public string outputID; //the ID of the object this can turn into
    public Sprite stage1; //seed growth1, set by ID
    public Sprite stage2; //seed growth2, set by ID
    public Sprite stage3; //seed growth3, set by ID
    public int health;
    public int yield;
    public float growth; //0.1f - 3.0f growth multiplier
}

public class ItemManager : MonoBehaviour
{
    public List<ItemStruct> itemCodex;

    void Start()
    {
        LoadFromJson("Seeds.json");
        LoadFromJson("Plants.json");
        LoadFromJson("Materials.json");
        LoadFromJson("Treasures.json");
        LoadFromJson("Catalysts.json");
    }

    //The player's amount of an item will no longer be stored on the item itself
    //Item saving will be done by item ID and ints
    //Item ID: first three letters of type + three numbers + first three letters of rarity + buyable/sellable
    //The six first characters in an ID are unique and can be used for saving and matching. The rest is extra information used to set up objects.
    //Item sprites are saved as "itemID.png" and matched on load. If no image is found, use placeholder automatically.
    
    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public ItemStruct[] items;
    }

    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.dataPath + "/JsonData/Items/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            ItemsWrapper dataWrapper = JsonUtility.FromJson<ItemsWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.items != null)
                {
                    foreach (ItemStruct item in dataWrapper.items)
                    {
                        InitialiseItem(item);
                    }
                }
                else
                {
                    Debug.LogError("Items array is null in JSON data. Check that the list has a wrapper with the \'items\' tag.");
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
            public void DisplayItemSprite(ItemStruct item, GameObject prefab, Transform parentTransform)
            {
                GameObject newItem = Instantiate(prefab, parentTransform);
                newItem.name = item.name;
                Image imageComponent = newItem.GetComponent<Image>();
                imageComponent.sprite = item.sprite;
            }

    public void InitialiseItem(ItemStruct item)
    {
        ItemIDParser(ref item);
        itemCodex.Add(item);
        DisplayItemSprite(item, itemDisplayer, canvasTransform);
    }

    public static void ItemIDParser(ref ItemStruct item)
    {
        item.type = TypeFinder(ref item.itemID);
        item.rarity = RarityFinder(ref item.itemID);
        item.image = ImageFinder(ref item.itemID);
        item.sprite = SpriteCreator(ref item.image);

        if (item.itemID[item.itemID.Length - 2] == 'N')
        {
            item.notBuyable = true;
        }
        if (item.itemID[item.itemID.Length - 1] == 'N')
        {
            item.notSellable = true;
        }
    }
    public static ItemType TypeFinder(ref string itemID)
    {
        if (itemID.Contains("PLA"))
        {
            return ItemType.Plant;
        }
        else if (itemID.Contains("SEE"))
        {
            return ItemType.Seed;
        }
        else if (itemID.Contains("CAT"))
        {
            return ItemType.Catalyst;
        }
        else if (itemID.Contains("BOO"))
        {
            return ItemType.Book;
        }
        else if (itemID.Contains("MAT"))
        {
            return ItemType.Material;
        }
        else if (itemID.Contains("TRA"))
        {
            return ItemType.Trade;
        }
        else if (itemID.Contains("TRE"))
        {
            return ItemType.Treasure;
        }
        else
        {
            return ItemType.Misc;
        }
    }

    public static ItemRarity RarityFinder(ref string itemID)
    {
        if (itemID.Contains("EXT"))
        {
            return ItemRarity.Extraordinary;
        }
        else if (itemID.Contains("MYT"))
        {
            return ItemRarity.Mythical;
        }
        else if (itemID.Contains("RAR"))
        {
            return ItemRarity.Rare;
        }
        else if (itemID.Contains("UNC"))
        {
            return ItemRarity.Uncommon;
        }
        else if (itemID.Contains("UNI"))
        {
            return ItemRarity.Unique;
        }
        else
        {
            return ItemRarity.Common;
        }
    }

    public static Texture2D ImageFinder (ref string itemID)
    {
        string fileDirectory = Application.dataPath + "/JsonData/Items/ItemSprites/";
        string filePath = fileDirectory + itemID.Substring(0, 6) + ".png";
        Texture2D imageTexture;

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Sprite not found for {itemID}. Using default.");
            filePath = fileDirectory + itemID.Substring(0, 3) + "000.png";
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
