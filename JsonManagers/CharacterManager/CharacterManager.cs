using UnityEngine;
public class CharacterManager : MonoBehaviour
{
    public static void Initialise(Character character)
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
        FindSprite(character);
    }

    public static void FindSprite(Character character)
    {
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

    public static void objectIDReader(ref Character character)
    {
        character.type = TypeFinder(ref character);

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
