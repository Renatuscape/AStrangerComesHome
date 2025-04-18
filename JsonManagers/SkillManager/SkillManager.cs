using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Set up skills with correct data after they are loaded from JSON
public class SkillManager : MonoBehaviour
{
    public static void Initialise(Skill skill)
    {
        skill.objectType = ObjectType.Skill;

        SkillIDReader(ref skill);

        if (skill.maxValue == 0)
        {
            if (skill.type == SkillType.Attunement)
            {
                skill.maxValue = StaticGameValues.maxAttributeValue;
            }
            else
            {
                skill.maxValue = StaticGameValues.maxSkillValue;
            }
        }
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
            return SkillType.Attunement;
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
