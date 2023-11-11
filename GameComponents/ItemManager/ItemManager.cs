using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEditor;
using UnityEngine.Rendering;

[System.Serializable]
public class Item : IRewardable
{
    public string objectID;
    public string name;
    public int basePrice; //automatically calculated from type, rarity and ID
    public int maxStack = 99;
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
    public int health; //Adds to price and growth time
    public int yield; //Adds to price and growth time

    public void AddToPlayer(int amount = 1)
    {
        if (amount > 0)
        {
            Player.AddItem(objectID, amount);
        }
    }
}

public static class Items
{
    public static List<Item> allItems = new();

    public static Item FindByID(string searchWord)
    {
        foreach (Item item in allItems)
        {
            if (item.objectID.Contains(searchWord))
            {
                return item;
            }
        }
        return null;
    }
    public static void DebugList()
    {
        Debug.LogWarning("Items.DebugList() called");

        foreach (Item item in allItems)
        {
            Debug.Log($"Item ID: {item.objectID}\tItem Name: {item.name}");
        }
    }
}

public class ItemManager : MonoBehaviour
{
    public List<Item> itemCodex = Items.allItems;
    public SerializableDictionary<string, int> playerItems;

    void Start()
    {
        LoadFromJson("Seeds.json");
        LoadFromJson("Plants.json");
        LoadFromJson("Materials.json");
        LoadFromJson("Treasures.json");
        LoadFromJson("Catalysts.json");
        LoadFromJson("Books.json");

        Items.DebugList();
    }

    //The player's amount of an item will no longer be stored on the item itself
    //Item saving will be done by item ID and ints
    //Item ID: first three letters of type + three numbers + first three letters of rarity + buyable/sellable
    //The six first characters in an ID are unique and can be used for saving and matching. The rest is extra information used to set up objects.
    //Item sprites are saved as "itemID.png" and matched on load. If no image is found, use placeholder automatically.

    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Item[] items;
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
                    foreach (Item item in dataWrapper.items)
                    {
                        InitialiseItem(item, itemCodex);
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
    public void DisplayItemSprite(Item item, GameObject prefab, Transform parentTransform)
    {
        GameObject newItem = Instantiate(prefab, parentTransform);
        newItem.name = item.name;
        Image imageComponent = newItem.GetComponent<Image>();
        imageComponent.sprite = item.sprite;
    }

    public static void InitialiseItem(Item item, List<Item> itemList)
    {
        ItemIDReader(ref item);
        CalculatePrice(ref item);
        itemList.Add(item);
        Player.AddItem(item.objectID, 0);
    }

    public static void ItemIDReader(ref Item item)
    {
        item.type = TypeFinder(ref item.objectID, ref item);
        item.rarity = RarityFinder(ref item.objectID, ref item);
        item.image = ImageFinder(ref item.objectID);

        if (item.image != null)
        {
            item.sprite = SpriteCreator(ref item.image);
        }
        else
        {
            Debug.LogError($"{item.objectID}.image was null. Could not create sprite.");
        }

        if (item.objectID[11] == 'N')
        {
            item.notBuyable = true;
        }
        else if (item.objectID[11] != 'B')
        {
            Debug.LogError($"{item.objectID} ID was not formatted correctly. Could not find N/B at index[11]");
        }
        if (item.objectID[12] == 'N')
        {
            item.notSellable = true;
        }
        else if (item.objectID[12] != 'S')
        {
            Debug.LogError($"{item.objectID} ID was not formatted correctly. Could not find N/S at index[12]");
        }
    }
    public static ItemType TypeFinder(ref string itemID, ref Item item)
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
            item.maxStack = 9;
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

    public static ItemRarity RarityFinder(ref string itemID, ref Item item)
    {
        if (itemID.Contains("COM"))
        {
            return ItemRarity.Common;
        }
        else if (itemID.Contains("UNC"))
        {
            return ItemRarity.Uncommon;
        }
        else if (itemID.Contains("RAR"))
        {
            return ItemRarity.Rare;
        }
        else if (itemID.Contains("EXT"))
        {
            return ItemRarity.Extraordinary;
        }
        else if (itemID.Contains("MYT"))
        {
            return ItemRarity.Mythical;
        }
        else if (itemID.Contains("UNI"))
        {
            item.maxStack = 1;
            return ItemRarity.Unique;
        }
        else
        {
            return ItemRarity.Junk;
        }
    }

    public static Texture2D ImageFinder(ref string itemID)
    {
        string fileDirectory = Application.dataPath + "/Sprites/Items/";
        string filePath = fileDirectory + itemID.Substring(0, 6) + ".png";
        Texture2D imageTexture;

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Image not found for {itemID} at {fileDirectory + itemID.Substring(0, 6)}\".png\". Using default.");
            filePath = fileDirectory + itemID.Substring(0, 3) + "000.png";

            if (!File.Exists(filePath))
            {
                Debug.LogError($"Default image not found for type {itemID.Substring(0, 3)} at {fileDirectory + itemID.Substring(0, 6)}\".png\"! No image set for {itemID}");
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

    public static void CalculatePrice(ref Item item)
    {
        float price = 10;

        int seedPrice = 15;
        int plantPrice = 35;
        int materialPrice = 50;
        int catalystPrice = 80;
        int tradePrice = 100;
        int bookPrice = 180;
        int treasurePrice = 350;

        float rarityMultiplier = 0.3f;

        if ((int)item.rarity >= 0)
        {
            rarityMultiplier = (int)Mathf.Pow((int)item.rarity + 1, 4);
        }

        switch (item.type)
        {
            case ItemType.Seed:
                price = seedPrice * rarityMultiplier * item.health * item.yield;
                break;
            case ItemType.Plant:
                price = plantPrice * rarityMultiplier;
                break;
            case ItemType.Material:
                price = materialPrice * rarityMultiplier;
                break;
            case ItemType.Catalyst:
                price = catalystPrice * rarityMultiplier;
                break;
            case ItemType.Trade:
                price = tradePrice * rarityMultiplier;
                break;
            case ItemType.Book:
                price = bookPrice * rarityMultiplier;
                break;
            case ItemType.Treasure:
                price = treasurePrice * rarityMultiplier;
                break;
            default:
                break;
        }

        // Create pricing variety
        int itemNumber = ExtractItemNumberFromID(item.objectID);
        float numberModifier = itemNumber * 10 * (2 + (int)item.rarity);
        float uniquePrice = price + numberModifier;

        item.basePrice = (int)uniquePrice;

        static int ExtractItemNumberFromID(string itemID)
        {
            // Extract the three numbers from index 2 to 5
            string numberPart = itemID.Substring(5);
            int itemNumber;
            int.TryParse(numberPart, out itemNumber);
            return itemNumber;
        }
    }
}
