using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEditor;

public class SkillManager : MonoBehaviour
{
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;
    void Start()
    {
        LoadFromJson("Skills.json");
    }

    [System.Serializable]
    public class ItemsWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Skill[] skills;
    }

    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.dataPath + "/JsonData/Skills/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            ItemsWrapper dataWrapper = JsonUtility.FromJson<ItemsWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.skills != null)
                {
                    foreach (Skill skill in dataWrapper.skills)
                    {
                        InitialiseSkill(skill, Skills.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All SKILLS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Items array is null in JSON data. Check that the list has a wrapper with the \'skills\' tag.");
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

    public static void InitialiseSkill(Skill skill, List<Skill> skillList)
    {
        ItemIDReader(ref skill);
        skillList.Add(skill);
    }

    public static void ItemIDReader(ref Skill skill)
    {
        skill.type = TypeFinder(ref skill.objectID);
        skill.image = ImageFinder(ref skill.objectID);

        if (skill.image != null)
        {
            skill.sprite = SpriteCreator(ref skill.image);
        }
        else
        {
            Debug.LogError($"{skill.objectID}.image was null. Could not create sprite.");
        }
    }
    public static SkillType TypeFinder(ref string objectID)
    {
        if (objectID.Contains("ALC"))
        {
            return SkillType.Alchemy;
        }
        else if (objectID.Contains("GAR"))
        {
            return SkillType.Gardening;
        }
        else if (objectID.Contains("MAG"))
        {
            return SkillType.Magic;
        }
        else
        {
            return SkillType.Attribute;
        }
    }

    public static Texture2D ImageFinder(ref string objectID)
    {
        string fileDirectory = Application.dataPath + "/Sprites/Skills/";
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
