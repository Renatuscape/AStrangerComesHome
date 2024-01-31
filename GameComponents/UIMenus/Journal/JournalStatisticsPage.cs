using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalStatisticsPage : MonoBehaviour
{
    public DataManagerScript dataManager;
    public PlayerIconPrefab playerIconPrefab;
    public PlayerSprite playerSprite;
    public TextMeshProUGUI namePlate;
    public TextMeshProUGUI totalDays;
    public TextMeshProUGUI totalMoney;
    void OnEnable()
    {
        playerIconPrefab.playerSprite = playerSprite;
        playerIconPrefab.UpdateImages();
        var player = Characters.FindByID("ARC000");

        if (player != null)
        {
            namePlate.text = Characters.FindByID("ARC000").ForceTrueNamePlate();
        }
        else
        {
            namePlate.text = dataManager.playerName;
        }

        totalDays.text = $"Days passed: {dataManager.totalGameDays}";
        totalMoney.text = $"Gold: {dataManager.playerGold}";
    }
}
