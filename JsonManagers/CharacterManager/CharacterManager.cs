using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public List<Character> debugCharacterList = Characters.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded = 0;
    public int numberOfFilesToLoad = 1;

    void Start()
    {
        LoadFromJson("Characters.json");
        //Remember to update numberOfFilesToLoad if more files are added
        //Items.DebugList();
    }

    [System.Serializable]
    public class CharactersWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Character[] characters;
    }

    public void LoadFromJson(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Characters/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            CharactersWrapper dataWrapper = JsonUtility.FromJson<CharactersWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.characters != null)
                {
                    foreach (Character character in dataWrapper.characters)
                    {
                        InitialiseCharacter(character, Characters.all);
                    }
                    filesLoaded++;
                    if (filesLoaded == numberOfFilesToLoad)
                    {
                        allObjecctsLoaded = true;
                        Debug.Log("All CHARACTERS successfully loaded from Json.");
                    }
                }
                else
                {
                    Debug.LogError("Character array is null in JSON data. Check that the list has a wrapper with the \'characters\' tag and that the object class is tagged serializable.");
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

    public static void InitialiseCharacter(Character character, List<Character> characterList)
    {
        objectIDReader(ref character);
        characterList.Add(character);
    }

    public static void objectIDReader(ref Character character)
    {
        character.type = TypeFinder(ref character);
        character.image = ImageFinder(ref character.objectID);

        if (character.image != null)
        {
            character.sprite = SpriteCreator(ref character.image);
        }
        else
        {
            Debug.LogError($"{character.objectID}.image was null. Could not create sprite.");
        }
    }
    public static CharacterType TypeFinder(ref Character character)
    {
        var objectID = character.objectID;
        
        if (objectID.Contains("ARC"))
        {
            return CharacterType.Arcana;
        }
        else if (objectID.Contains("UNI"))
        {
            return CharacterType.Unique;
        }
        else
        {
            return CharacterType.Generic;
        }
    }

    public static Texture2D ImageFinder(ref string objectID)
    {
        string fileDirectory = Application.streamingAssetsPath + "/Sprites/Characters/";
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