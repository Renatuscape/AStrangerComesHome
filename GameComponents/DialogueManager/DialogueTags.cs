using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTags : MonoBehaviour
{
    public DataManagerScript dataManager;

    public Character player;
    public Character alchemist;
    public Character botanist;
    public Character machinist;
    public Character teller;
    public Character archaeologist;
    public Character bibliothecarian;
    public Character occultist;
    public Character magus;
    public Character stranger;

    public SerializableDictionary<string, string> tagDictionary = new SerializableDictionary<string, string>()
        {
            { "PlayerName", "Morgan" },
            { "he", "subject" },
            { "him", "object" },
            { "his", "genitive" },
            { "He", "Subject" },
            { "Him", "Object" },
            { "His", "Genitive" },
            { "The Traveller", "Morgan" },
            { "Traveller", "Morgan" },
            { "The Alchemist", "Bob" },
            { "Alchemist", "Bob" },
            { "The Botanist", "Rita" },
            { "Botanist", "Rita" },
            { "The Machinist", "Joe" },
            { "Machinist", "Joe" },
            { "The Teller", "Alex" },
            { "Teller", "Alex" },
            { "The Archaeologist", "Ellis" },
            { "Archaeologist", "Ellis" },
            { "The Bibliothecarian", "Aaron" },
            { "Bibliothecarian", "Aaron" },
            { "The Occultist", "Mira" },
            { "Occultist", "Mira" },
            { "The Magus", "Polly" },
            { "Magus", "Polly" },
            { "The Stranger", "Evan" },
            { "Stranger", "Evan" }
        };

    private void OnEnable()
    {
        UpdateDialogueTags();
    }
    public void UpdateDialogueTags()
    {
        tagDictionary["PlayerName"] = player.namePlate;
        tagDictionary["he"] = dataManager.pronounSub;
        tagDictionary["him"] = dataManager.pronounObj;
        tagDictionary["his"] = dataManager.pronounGen;
        tagDictionary["He"] = UpperCaseFormatter(dataManager.pronounSub);
        tagDictionary["Him"] = UpperCaseFormatter(dataManager.pronounObj);
        tagDictionary["His"] = UpperCaseFormatter(dataManager.pronounGen);
        tagDictionary["The Traveller"] = "<color=#" + dataManager.playerNameColour + ">The Traveller</color>";
        tagDictionary["Traveller"] = "<color=#" + dataManager.playerNameColour + ">Traveller</color>";

        tagDictionary["The Alchemist"] = alchemist.namePlate;
        tagDictionary["Alchemist"] = alchemist.namePlate.Replace("The ", "");
        tagDictionary["The Botanist"] = botanist.namePlate;
        tagDictionary["Botanist"] = botanist.namePlate.Replace("The ", "");
        tagDictionary["The Machinist"] = machinist.namePlate;
        tagDictionary["Machinist"] = machinist.namePlate.Replace("The ", "");
        tagDictionary["The Teller"] = teller.namePlate;
        tagDictionary["Teller"] = teller.namePlate.Replace("The ", "");
        tagDictionary["The Archaeologist"] = archaeologist.namePlate;
        tagDictionary["Archaeologist"] = archaeologist.namePlate.Replace("The ", "");
        tagDictionary["The Bibliothecarian"] = bibliothecarian.namePlate;
        tagDictionary["Bibliothecarian"] = bibliothecarian.namePlate.Replace("The ", "");
        tagDictionary["The Occultist"] = occultist.namePlate;
        tagDictionary["Occultist"] = occultist.namePlate.Replace("The ", "");
        tagDictionary["The Magus"] = magus.namePlate;
        tagDictionary["Magus"] = magus.namePlate.Replace("The ", "");
        tagDictionary["The Stranger"] = stranger.namePlate;
        tagDictionary["Stranger"] = stranger.namePlate.Replace("The ", "");
    }

    public string UpperCaseFormatter(string input)
    {
        if (input.Length > 0)
        {
            string firstLetter = input.Substring(0, 1);
            string remainingLetters = input.Substring(1);
            string result = firstLetter.ToUpper() + remainingLetters;
            return result;
        }
        else
            return input;
    }
}