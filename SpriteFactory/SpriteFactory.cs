using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpriteFactory : MonoBehaviour
{
    public static SpriteFactory instance;

    public CharacterExpressionCatalogue expressionCatalogue;
    public CharacterEyesCatalogue eyesCatalogue;
    public CharacterHairCatalogue hairCatalogue;
    public CharacterBodyCatalogue bodyCatalogue;

    public List<Sprite> unsortedUiSprites = new();
    public List<Sprite> unsortedWorldSprites = new();
    public List<Sprite> backgroundSprites = new();
    public List<Sprite> itemSprites = new();
    public List<Sprite> sproutSprites = new();
    public List<Sprite> genericPassengerSprites = new();
    public List<Sprite> uniquePassengerSprites = new();

    public List<SpriteCollection> uiSpriteLibrary = new();

    public SpriteCollection defaultUiCharacter;
    public Sprite defaultWorldCharacter;
    public SpriteCollection defaultItem;
    public SpriteCollection defaultUpgrade;

    TaskCompletionSource<bool> buildCompleteTaskSource;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Sprite GetPassengerByID(string spriteID)
    {
        var foundSprite = instance.uniquePassengerSprites.FirstOrDefault(s => s.name == spriteID);

        if (foundSprite == null)
        {
            return instance.genericPassengerSprites.FirstOrDefault(s => s.name == spriteID);
        }
        else
        {
            return foundSprite;
        }
    }

    public static Sprite GetRandomPassengerSprite()
    {
        return instance.genericPassengerSprites[Random.Range(0, instance.genericPassengerSprites.Count)];
    }

    public static Sprite GetItemSprite(string objectID)
    {
        var foundSprite = instance.itemSprites.FirstOrDefault(c => objectID.Contains(c.name));

        if (foundSprite == null)
        {
            foundSprite = instance.itemSprites.FirstOrDefault(c => c.name.Contains(objectID.Substring(0, 3) + "000"));
        }
        return foundSprite;
    }

    public static Sprite GetSprout(string objectID, int stage)
    {
        var foundSprite = instance.sproutSprites.FirstOrDefault(c => c.name == objectID + "-" + stage);

        if (foundSprite == null)
        {
            foundSprite = instance.sproutSprites.FirstOrDefault(c => c.name == "SEE000-" + stage);
        }

        return foundSprite;
    }

    public static SpriteCollection GetUiSprite(string objectID)
    {
        var foundCollection = instance.uiSpriteLibrary.FirstOrDefault(c => c.objectID == objectID);

        if (foundCollection == null)
        {
            // Debug.LogWarning("Character sprite returned null: " + objectID);
            return instance.defaultUiCharacter;
        }
        else
        {
            return foundCollection;
        }
    }

    public static Sprite GetBackgroundSprite(string backgroundName, string defaultType = "shop")
    {
        var foundCollection = instance.backgroundSprites.FirstOrDefault(c => c.name.ToLower().Contains(backgroundName.ToLower()));

        if (foundCollection == null)
        {
            foundCollection = instance.backgroundSprites.FirstOrDefault(c => c.name.ToLower().Contains("default") && c.name.ToLower().Contains(defaultType.ToLower()));
        }

        return foundCollection;
    }

    public static Sprite GetWorldCharacterSprite(string objectID)
    {
        var foundSprite = instance.unsortedWorldSprites.FirstOrDefault(c => c.name.Contains(objectID));

        if (foundSprite != null)
        {
            return foundSprite;
        }

        return instance.defaultWorldCharacter;
    }

    public async Task WaitForBuildCompletionAsync()
    {
        buildCompleteTaskSource = new TaskCompletionSource<bool>();
        await BuildSpriteLibraryAsync();
        await buildCompleteTaskSource.Task;
    }

    async Task BuildSpriteLibraryAsync()
    {
        unsortedUiSprites = unsortedUiSprites.OrderBy(x => x.name).ToList();
        List<Sprite> unsortedFrames = new();

        foreach (Sprite sprite in unsortedUiSprites)
        {
            if (sprite.name.Contains('-'))
            {
                unsortedFrames.Add(sprite);
            }
            else
            {
                CreateSpriteCollection(sprite);
            }
        }

        foreach (Sprite sprite in unsortedFrames)
        {
            SortSpriteEvent(sprite);
        }

        await Task.Yield(); // This is just to make this method asynchronous, you can remove it if you have async operations in the future.

        // Signal that the build operation is complete
        buildCompleteTaskSource.SetResult(true);
    }

    void SortSpriteEvent(Sprite sprite)
    {
        // Example data: ARC001-DEFAULT-1-F#50
        // Example data: UNI004-HAPPY-3
        string[] spriteData = sprite.name.Split('-');
        SpriteCollection collection = uiSpriteLibrary.FirstOrDefault(c => c.objectID == spriteData[0]);

        if (collection != null)
        {
            var sEvent = collection.GetEvent(spriteData[1]);

            if (sEvent == null)
            {
                sEvent = new() { eventID = spriteData[1] };
                collection.events.Add(sEvent);
            }

            if (sprite.name.Contains("F#")) // Frames should ALWAYS be the last part of a name
            {
                string frameData = spriteData[spriteData.Length - 1].Replace("F#", "");
                float parsedFloat = float.Parse(frameData);
                sEvent.recommendedSecondsPerFrame = parsedFloat / 100;
            }
            else
            {
                // Set default value for recommended seconds per frame here if FPS has not been set yet (careful about overriding, since there are multiple sprites in one event)
            }

            sEvent.frames.Add(sprite);
        }
        else
        {
            Debug.LogWarning($"{sprite.name} had no default collection. Does a default sprite exist?");
        }
    }

    void CreateSpriteCollection(Sprite sprite)
    {
        SpriteEvent defaultEvent = new() { eventID = "DEFAULT" };
        defaultEvent.frames = new() { sprite };

        SpriteCollection newCollection = new() { objectID = sprite.name };
        newCollection.events = new() { defaultEvent };

        if (sprite.name.ToLower().Contains("defaultcharacter"))
        {
            defaultUiCharacter = newCollection;
        }
        else if (sprite.name.ToLower().Contains("defaultitem"))
        {
            defaultItem = newCollection;
        }
        else if (sprite.name.ToLower().Contains("defaultupgrade"))
        {
            defaultUpgrade = newCollection;
        }
        else
        {
            uiSpriteLibrary.Add(newCollection);
        }
    }
}

[System.Serializable]
public class SpriteCollection
{
    public string objectID;
    public List<SpriteEvent> events;

    public SpriteEvent GetEvent(string eventID)
    {
        return events.FirstOrDefault((e) => e.eventID == eventID);
    }

    public Sprite GetDefaultFrame()
    {
        return events.FirstOrDefault((e) => e.eventID == "DEFAULT").GetCurrentFrame();
    }
    public Sprite GetFirstFrameFromEvent(string eventID)
    {
        if (string.IsNullOrEmpty(eventID))
        {
            return GetDefaultFrame();
        }
        var dEvent = events.FirstOrDefault((e) => eventID.Contains(e.eventID));

        if (dEvent == null)
        {
            Debug.LogWarning($"Event ID {eventID} returned null. Using default.");
            return GetDefaultFrame();
        }

        var firstFrame = dEvent.GetFirstFrame();
        return firstFrame;
    }
    public Sprite GetCurrentFrameFromEvent(string eventID)
    {
        return events.FirstOrDefault((e) => e.eventID == eventID).GetCurrentFrame();
    }
}

[System.Serializable]
public class SpriteEvent
{
    public string eventID;
    public int frameIndex = 0;
    public float recommendedSecondsPerFrame = 0;
    public List<Sprite> frames = new();

    public Sprite GetFirstFrame()
    {
        return frames[0];
    }
    public Sprite GetCurrentFrame()
    {
        return frames[frameIndex];
    }
    public Sprite GetNextFrame()
    {
        frameIndex++;

        if (frameIndex >= frames.Count)
        {
            frameIndex = 0;
        }

        return frames[frameIndex];
    }

    public bool CheckForLastFrame()
    {
        if (frameIndex >= frames.Count - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
