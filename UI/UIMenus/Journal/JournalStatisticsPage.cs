using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalStatisticsPage : MonoBehaviour
{
    public FontManager fontManager;
    public DataManagerScript dataManager;
    public PageinatedList pageinatedList;
    public RectTransform pageinatedContainer;
    public Button btnSkills;
    public Button btnSpheres;
    public Button btnUpgrades;

    public PlayerSprite playerSprite;
    public GameObject personaliaContainer;
    public TextMeshProUGUI namePlate;
    public TextMeshProUGUI namePlateShadow;
    public TextMeshProUGUI licenseInfo;
    public GameObject upgradeContainer;
    public GameObject otherContainer;
    public List<UpgradeIcon> upgrades;
    bool upgradesSetUp = false;

    private void Start()
    {
        btnSkills.onClick.AddListener(() => EnableSkillList());
        btnSpheres.onClick.AddListener(() => EnableSphereList());
        btnUpgrades.onClick.AddListener(() => EnableUpgradeList());

        if (!upgradesSetUp)
        {
            BuildUpgradePage();
        }
    }
    void OnEnable()
    {
        int licenseGrade = Player.GetCount("SCR012", name);

        if (Characters.all.Count > 0)
        {
            var player = Characters.FindByID("ARC000");

            if (player != null)
            {
                namePlate.text = player.ForceTrueNamePlate();
                namePlateShadow.text = player.trueName;
            }
            else
            {
                namePlate.text = dataManager.playerName;
                namePlateShadow.text = dataManager.playerName;
            }
        }
        else
        {
            namePlate.text = dataManager.playerName;
        }

        if (licenseGrade > 0)
        {
            string serviceTime;

            if (dataManager.totalGameDays > 336)
            {
                var years = Mathf.FloorToInt((float)dataManager.totalGameDays / 336);
                serviceTime = $"{years} year{(years != 1 ? "s" : "")} in service";
            }
            else
            {
                var months = Mathf.FloorToInt((float)dataManager.totalGameDays / 28);
                serviceTime = $"{months} month{(months != 1 ? "s" : "")} in service";
            }

            licenseInfo.text = $"Grade {licenseGrade} License\n{serviceTime}";

            // namePlate.font = fontManager.header.font;
            // licenseInfo.font = fontManager.script.font;

            personaliaContainer.SetActive(true);
        }
        else
        {
            personaliaContainer.SetActive(false);
        }

        if (upgradesSetUp)
        {
            UpdateUpgradeNumbers();
        }
        else
        {
            BuildUpgradePage();
        }

        upgradeContainer.SetActive(true);
    }

    public void EnableUpgradeList()
    {
        upgradeContainer.gameObject.SetActive(true);
        otherContainer.gameObject.SetActive(false);
    }
    public void EnableSkillList()
    {
        upgradeContainer.gameObject.SetActive(false);
        otherContainer.gameObject.SetActive(true);

        //List<IdIntPair> list = new List<IdIntPair>();
        ListCategory gardeningSkills = new() { categoryName = "Gardening", listContent = new()};
        ListCategory alchemySkills = new() { categoryName = "Alchemy", listContent = new()};
        ListCategory magicSkills = new() { categoryName = "Magic", listContent = new()};

        foreach (var skill in Skills.all)
        {
            if (!skill.objectID.Contains("ATT") && Player.GetEntry(skill.objectID, "Journal Statistics", out var entry))
            {
                IdIntPair newEntry = new() { objectID = entry.objectID, description = skill.name + $" {entry.amount}" };

                if (skill.objectID.Contains("GAR"))
                {
                    gardeningSkills.listContent.Add(newEntry);
                }
                else if (skill.objectID.Contains("ALC"))
                {
                    alchemySkills.listContent.Add(newEntry);
                }
                else if (skill.objectID.Contains("MAG"))
                {
                    magicSkills.listContent.Add(newEntry);
                }
            }
        }

        if (gardeningSkills.listContent.Count == 0)
        {
            IdIntPair newEntry = new();
            newEntry.description = "No gardening skills";
            gardeningSkills.listContent.Add(newEntry);
        }
        if (alchemySkills.listContent.Count == 0)
        {
            IdIntPair newEntry = new();
            newEntry.description = "No alchemy skills";
            alchemySkills.listContent.Add(newEntry);
        }
        if (magicSkills.listContent.Count == 0)
        {
            IdIntPair newEntry = new();
            newEntry.description = "No magic skills";
            magicSkills.listContent.Add(newEntry);
        }

        pageinatedContainer.sizeDelta = new Vector2(pageinatedContainer.sizeDelta.x, 435);
        pageinatedList.InitialiseWithCategories(new List<ListCategory>() { gardeningSkills, alchemySkills, magicSkills });
    }

    public void EnableSphereList()
    {
        upgradeContainer.gameObject.SetActive(false);
        otherContainer.gameObject.SetActive(true);

        List<IdIntPair> list = new List<IdIntPair>();

        foreach (var skill in Skills.all)
        {
            if (skill.objectID.Contains("ATT") && Player.GetEntry(skill.objectID, "Journal Statistics", out var entry))
            {
                IdIntPair newEntry = new();
                newEntry.objectID = entry.objectID;
                newEntry.description = skill.name + $" {entry.amount}";
                list.Add(newEntry);
            }
        }

        pageinatedContainer.sizeDelta = new Vector2(pageinatedContainer.sizeDelta.x, 500);
        pageinatedList.InitialiseWithoutCategories(list);
    }
    void UpdateUpgradeNumbers()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.level = Player.GetCount(upgrade.upgrade.objectID, "JournalStatistics Update Numbers");
            upgrade.SetLevelText();
            upgrade.UpdateSlider();
        }
    }

    void BuildUpgradePage()
    {
        if (Upgrades.all.Count >= 7)
        {
            int upgradeIndex = 0;

            foreach (Transform slot in upgradeContainer.transform)
            {
                Destroy(slot.gameObject.GetComponent<Image>());
                var upgrade = BoxFactory.CreateUpgradeIcon(Upgrades.all[upgradeIndex], true, false, true, true);
                upgrade.gameObject.transform.SetParent(slot.transform, false);
                var rect = upgrade.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(100, 100);

                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                rect.anchoredPosition = new Vector3(0, 0, 0);

                upgrades.Add(upgrade.gameObject.GetComponent<UpgradeIcon>());
                upgradeIndex++;
            }

            upgradesSetUp = true;
        }
    }
}
