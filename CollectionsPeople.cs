using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionsPeople : MonoBehaviour
{
    public FontManager fontManager;
    public List<Character> people;
    public int index = 0;

    public TextMeshProUGUI peoplePageTitle;
    public TextMeshProUGUI peoplePageTrueName;
    public TextMeshProUGUI peoplePageDescription;
    public Image peoplePagePicture;

    private void OnEnable()
    {
        PopulatePeople();
        PrintPerson();
        peoplePageTitle.font = fontManager.header.font;
        peoplePageTrueName.font = fontManager.subtitle.font;
        peoplePageDescription.font = fontManager.script.font;
    }

    void PopulatePeople()
    {
        people = new List<Character>();
        foreach (var character in Characters.all)
        {
            if (!character.excludeFromPrint && Player.GetCount(character.objectID, ToString()) > 0)
            {
                people.Add(character);
            }
        }

        if (people.Count == 0)
        {
            peoplePageTitle.text = "";
            peoplePageTrueName.gameObject.SetActive(false);
            peoplePageDescription.text = "I should add my old friends here when I find the time.";
            peoplePagePicture.color = Color.black;
        }
        else
        {
            peoplePagePicture.color = Color.white;
        }
    }

    void PrintPerson()
    {
        if (index >= people.Count)
        {
            index = 0;
        }

        if (people != null && people.Count > 0)
        {
            peoplePageTitle.text = people[index].namePlate;
            peoplePageTrueName.text = people[index].NamePlate();
            peoplePageDescription.text = people[index].description;
            peoplePagePicture.sprite = people[index].sprite;

            if (peoplePageTitle.text == peoplePageTrueName.text)
            {
                peoplePageTrueName.gameObject.SetActive(false);
            }
            else
            {
                peoplePageTrueName.gameObject.SetActive(true);
            }

            peoplePageDescription.text += $"\nDebug:" +
                $"\nAffecton {Player.GetCount(people[index].objectID, ToString())}" +
                $"\nName returned {people[index].NamePlate()}" +
                $"\nType {people[index].type}";
        }

    }

    public void ChangePeoplePage(bool reverse = false)
    {
        if (people.Count > 0)
        {
            if (!reverse)
            {
                index++;
            }
            else
            {
                index--;
            }

            if (index >= people.Count)
            {
                index = 0;
            }

            if (index < 0)
            {
                index = people.Count - 1;
            }

            PrintPerson();
        }
    }
}