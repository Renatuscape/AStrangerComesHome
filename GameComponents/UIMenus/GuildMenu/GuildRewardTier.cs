using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class GuildRewardTier
{
    public string tierID;
    public string description;
    public int requiredCount;
    public int playerCount;
    public List<IdIntPair> requirements;
    public List<IdIntPair> rewards;
    public bool claimed;
    public TextMeshProUGUI textMesh;
    public Button button;

    public void Setup(int playerCount, Button button)
    {
        this.playerCount = playerCount;
        this.button = button;
        textMesh = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if (Player.claimedLoot.FirstOrDefault(l => l.objectID == tierID) != null)
        {
            UnityEngine.Debug.Log("Reward tier found in claimedLoot: " + tierID);
            SetComplete();
            button.interactable = false;
        }
        else
        {
            UnityEngine.Debug.Log("NOT found in claimedLoot: " + tierID);
        }
    }

    public void Claim()
    {
        if (!claimed)
        {
            Player.claimedLoot.Add(new() { objectID = tierID, amount = 1 });
            SetComplete();
            button.interactable = false;

            foreach (var entry in rewards)
            {
                Player.Add(entry);
            }
        }
    }

    void SetComplete()
    {
        if (!claimed)
        {
            claimed = true;
            textMesh.text = $"<color=#718c81><s>{textMesh.text}</s></color>";
            ColorBlock colorBlock = button.colors;
            colorBlock.disabledColor = new UnityEngine.Color(0.8071f, 0.9150f, 0.8301f);
            button.colors = colorBlock;
        }
    }
}