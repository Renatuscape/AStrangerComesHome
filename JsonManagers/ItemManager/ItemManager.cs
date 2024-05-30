using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

public class ItemManager : MonoBehaviour
{
    public List<Item> debugItemList = Items.all;
    public bool allObjectsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 8;

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 0;

        var info = new DirectoryInfo(Application.streamingAssetsPath + "/JsonData/Items/");
        var fileInfo = info.GetFiles();

        foreach (var file in fileInfo)
        {
            if (file.Extension == ".json")
            {
                numberOfFilesToLoad++;
                Task loadingTask = LoadFromJsonAsync(Path.GetFileName(file.FullName)); // Pass only the file name
                loadingTasks.Add(loadingTask);
            }
        }

        return Task.WhenAll(loadingTasks);
    }

    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Item[] items;
    }

    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Items/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            ItemsWrapper dataWrapper = JsonUtility.FromJson<ItemsWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.items != null)
                {
                    foreach (Item item in dataWrapper.items)
                    {
                        InitialiseItem(item, Items.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjectsLoaded = true;
                        Debug.Log("All ITEMS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Items array is null in JSON data. Check that the list has a wrapper with the \'items\' tag and that the object class is serializable.");
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

    public static void InitialiseItem(Item item, List<Item> itemList)
    {
        // Set base object values
        item.objectType = ObjectType.Item;

        // Set item-specific values
        ItemIDReader(ref item);

        if (item.type != ItemType.Misc)
        {
            item.basePrice = CalculatePrice(ref item);
        }

        if (item.maxValue == 0)
        {
            if (item.rarity == ItemRarity.Unique)
            {
                item.maxValue = 1;
            }
            else if (item.type == ItemType.Book)
            {
                item.maxValue = StaticGameValues.maxBookValue;
            }
            else
            {
                item.maxValue = StaticGameValues.maxItemValue;
            }
        }

        item.objectID = item.objectID.Split('-')[0];
        itemList.Add(item);
    }

    public static void ItemIDReader(ref Item item)
    {
        item.type = TypeFinder(ref item);
        item.rarity = RarityFinder(ref item);
        item.sprite = SpriteFactory.GetItemSprite(item.objectID);// item.image = ImageFinder(ref item.objectID);

        //if (item.image != null)
        //{
        //    item.sprite = SpriteCreator(ref item.image);
        //}
        //else
        //{
        //    Debug.LogError($"{item.objectID}.image was null. Could not create sprite.");
        //}

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
    public static ItemType TypeFinder(ref Item item)
    {
        var objectID = item.objectID;
        if (objectID.Contains("PLA"))
        {
            return ItemType.Plant;
        }
        else if (objectID.Contains("SEE"))
        {
            return ItemType.Seed;
        }
        else if (objectID.Contains("CAT"))
        {
            return ItemType.Catalyst;
        }
        else if (objectID.Contains("BOO"))
        {
            return ItemType.Book;
        }
        else if (objectID.Contains("MAT"))
        {
            return ItemType.Material;
        }
        else if (objectID.Contains("TRA"))
        {
            return ItemType.Trade;
        }
        else if (objectID.Contains("TRE"))
        {
            return ItemType.Treasure;
        }
        else if (objectID.Contains("SCR"))
        {
            return ItemType.Script;
        }
        else
        {
            return ItemType.Misc;
        }
    }

    public static ItemRarity RarityFinder(ref Item item)
    {
        if (item.objectID.Contains("EXT"))
        {
            return ItemRarity.Extraordinary;
        }
        else if (item.objectID.Contains("MYT"))
        {
            return ItemRarity.Mythical;
        }
        else if (item.objectID.Contains("RAR"))
        {
            return ItemRarity.Rare;
        }
        else if (item.objectID.Contains("UNC"))
        {
            return ItemRarity.Uncommon;
        }
        else if (item.objectID.Contains("UNI"))
        {
            return ItemRarity.Unique;
        }
        else if (item.objectID.Contains("JUN"))
        {
            return ItemRarity.Junk;
        }
        else if (item.objectID.Contains("SCR"))
        {
            return ItemRarity.Script;
        }
        else
        {
            return ItemRarity.Common;
        }
    }

    //public static void SproutFinder(ref Item seed)
    //{
    //    seed.stage1 = FindSprite(1, seed.objectID);
    //    seed.stage2 = FindSprite(2, seed.objectID);
    //    seed.stage3 = FindSprite(3, seed.objectID);

    //    Sprite FindSprite(int frame, string objectID)
    //    {
    //        string fileDirectory = Application.streamingAssetsPath + "/Sprites/Items/Sprouts/";
    //        string filePath = fileDirectory + objectID.Substring(0, 6) + $"-{frame}.png";
    //        Texture2D texture;

    //        if (!File.Exists(filePath))
    //        {
    //            //Debug.LogWarning($"Sprout not found for {objectID} stage {frame}. Using default.");
    //            filePath = fileDirectory + objectID.Substring(0, 3) + $"000-{frame}.png";

    //            if (!File.Exists(filePath))
    //            {
    //                Debug.LogError($"Default image not found for type {objectID.Substring(0, 3)}! No image set for {objectID}");
    //                return null;
    //            }
    //        }

    //        byte[] imageBytes = File.ReadAllBytes(filePath);
    //        texture = new Texture2D(2, 2); // Create an empty texture
    //        texture.filterMode = FilterMode.Point; // Set filter mode to Point for pixel-perfect clarity. PREVENTS BLURRINESS
    //        texture.LoadImage(imageBytes); // Load the image bytes

    //        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    //    }
    //}

    //public static Texture2D ImageFinder (ref string objectID)
    //{
    //    string fileDirectory = Application.streamingAssetsPath + "/Sprites/Items/";
    //    string filePath = fileDirectory + objectID.Substring(0, 6) + ".png";
    //    Texture2D imageTexture;

    //    if (!File.Exists(filePath))
    //    {
    //        //Debug.LogWarning($"Image not found for {objectID}. Using default.");
    //        filePath = fileDirectory + objectID.Substring(0, 3) + "000.png";

    //        if (!File.Exists(filePath))
    //        {
    //            Debug.LogError($"Default image not found for type {objectID.Substring(0, 3)}! No image set for {objectID}");
    //            return null;
    //        }
    //    }

    //    byte[] imageBytes = File.ReadAllBytes(filePath);
    //    imageTexture = new Texture2D(2, 2); // Create an empty texture
    //    imageTexture.filterMode = FilterMode.Point; // Set filter mode to Point for pixel-perfect clarity. PREVENTS BLURRINESS
    //    imageTexture.LoadImage(imageBytes); // Load the image bytes

    //    return imageTexture;
    //}

    //public static Sprite SpriteCreator(ref Texture2D texture)
    //{
    //    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    //}

    public static int CalculatePrice(ref Item item)
    {
        float price = 5;

        int seedPrice = 1;
        int plantPrice = 30;
        int materialPrice = 35;
        int catalystPrice = 40;
        int tradePrice = 50;
        int bookPrice = 100;
        int treasurePrice = 250;

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

        return  (int)uniquePrice;

        static int ExtractItemNumberFromID(string itemID)
        {
            // Extract the three numbers from index 5
            string numberPart = itemID[5].ToString();
            int itemNumber;
            int.TryParse(numberPart, out itemNumber);
            return itemNumber;
        }
    }
}
