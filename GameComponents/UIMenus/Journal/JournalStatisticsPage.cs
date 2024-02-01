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
    public TextMeshProUGUI namePlate;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI totalDays;
    public TextMeshProUGUI totalMoney;

    void OnEnable()
    {
        playerIcon.playerSprite = playerSprite;
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


        totalDays.text = $"Days passed: {dataManager.totalGameDays}";
        totalMoney.text = $"Gold: {dataManager.playerGold}";

        namePlate.font = fontManager.header.font;
        titleText.font = fontManager.subtitle.font;
        totalDays.font = fontManager.script.font;
        totalMoney.font = fontManager.script.font;
    }
}
