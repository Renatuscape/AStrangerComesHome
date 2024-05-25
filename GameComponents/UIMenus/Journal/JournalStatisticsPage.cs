using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalStatisticsPage : MonoBehaviour
{
    public FontManager fontManager;
    public DataManagerScript dataManager;
    public PlayerIcon playerIcon;
    public PlayerSprite playerSprite;
    public GameObject personaliaContainer;
    public TextMeshProUGUI namePlate;
    public TextMeshProUGUI licenseInfo;
    public GameObject upgradeContainer;
    public GameObject otherContainer;
    public List<GameObject> prefabs;
    void OnEnable()
    {
        playerIcon.playerSprite = playerSprite;
        int licenseGrade = Player.GetCount("SCR012", name);
        playerIcon.UpdateImages();

        if (Characters.all.Count > 0)
        {
            var player = Characters.FindByID("ARC000");

            if (player != null)
            {
                namePlate.text = player.ForceTrueNamePlate();
            }
            else
            {
                namePlate.text = dataManager.playerName;
            }
        }
        else
        {
            namePlate.text = dataManager.playerName;
        }

        if (licenseGrade > 0)
        {
            licenseInfo.text = $"Grade {licenseGrade} License\nExperience: {Mathf.FloorToInt((float)dataManager.totalGameDays / 28)} months";

            // namePlate.font = fontManager.header.font;
            // licenseInfo.font = fontManager.script.font;

            personaliaContainer.SetActive(true);
        }
        else
        {
            personaliaContainer.SetActive(false);
        }
    }
}
