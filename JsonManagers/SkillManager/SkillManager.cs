using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;
public class SkillManager : MonoBehaviour
{
    public List<Skill> debugSkillList = Skills.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    public Task StartLoading()
    {
        gameObject.SetActive(true);
        return LoadFromJsonAsync("Skills.json");
    }

    [System.Serializable]
    public class SkillWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Skill[] skills;
    }

    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Skills/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            SkillWrapper dataWrapper = JsonUtility.FromJson<SkillWrapper>(jsonData);

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
                    Debug.LogError("Skill array is null in JSON data. Check that the list has a wrapper with the \'skills\' tag.");
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

    public static void InitialiseSkill(Skill skill, List<Skill> skillList)
    {
        SkillIDReader(ref skill);
        skillList.Add(skill);
    }

    public static void SkillIDReader(ref Skill skill)
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
        string fileDirectory = Application.streamingAssetsPath + "/Sprites/Skills/";
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
