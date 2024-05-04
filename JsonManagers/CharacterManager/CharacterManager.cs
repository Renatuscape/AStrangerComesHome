using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CharacterManager : MonoBehaviour
{
    public List<Character> debugCharacterList = Characters.all;
    public bool allObjecctsLoaded = false;
    public int filesLoaded;
    public int numberOfFilesToLoad;

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        gameObject.SetActive(true);
        filesLoaded = 0;
        numberOfFilesToLoad = 1;

        Task loadingTask = LoadFromJsonAsync("Characters.json");

        return Task.WhenAll(loadingTasks);
    }

    [System.Serializable]
    public class CharactersWrapper //Necessary for Unity to read the .json contents as an object
    {
        public Character[] characters;
    }

    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = Application.streamingAssetsPath + "/JsonData/Characters/" + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
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
        character.objectType = ObjectType.Character;
        character.maxValue = StaticGameValues.maxCharacterValue;

        if (character.giftableLevel == 0)
        {
            character.giftableLevel = StaticGameValues.defaultGiftableLevel;
        }

        if (character.shops != null && character.shops.Count > 0)
        {
            foreach (var shop in character.shops)
            {
                shop.Initialise();
            }
        }

        character.NameSetup();
        objectIDReader(ref character);
        characterList.Add(character);
    }

    public static void objectIDReader(ref Character character)
    {
        character.type = TypeFinder(ref character);
        character.spriteCollection = SpriteFactory.GetUiSprite(character.objectID);

        if (character.spriteCollection != null)
        {
            character.sprite = character.spriteCollection.GetDefaultFrame();
        }
        else
        {
            Debug.LogError($"{character.objectID}.image was null. Could not create sprite.");
        }
    }
    public static CharacterType TypeFinder(ref Character character)
    {
        var objectID = character.objectID;

        if (objectID == "DEBUG")
        {
            objectID = "ARC001";
        }

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
}
