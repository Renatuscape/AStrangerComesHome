using System.Linq;
using UnityEngine;

public class GuildRewardPrefab : MonoBehaviour
{
    public ListItemPrefab listPrefab;
    public GuildRewardTier tierData;
    public string defaultTitle;
    public bool claimed;

    public void Setup(ListItemPrefab listPrefab, GuildRewardTier tierData)
    {
        this.listPrefab = listPrefab;
        this.tierData = tierData;
        defaultTitle = listPrefab.textMesh.text;
        CheckState();
    }

    public void CheckState()
    {
        if (Player.claimedLoot.FirstOrDefault(l => l.objectID == tierData.tierID) != null)
        {
            Debug.Log("Reward tier found in claimedLoot: " + tierData.tierID);
            SetClaimed();
        }
        else if (RequirementChecker.CheckRequirements(tierData.requirements))
        {
            SetReadyForClaim();
        }
        else
        {
            SetIncomplete();
            Debug.Log("NOT found in claimedLoot: " + tierData.tierID);
        }
    }

    public void Claim()
    {
        if (!claimed)
        {
            listPrefab.button.onClick.RemoveAllListeners();

            Player.claimedLoot.Add(new() { objectID = tierData.tierID, amount = 1 });
            SetClaimed();

            foreach (var entry in tierData.rewards)
            {
                Player.Add(entry);
            }
        }
    }

    void SetClaimed()
    {
        claimed = true;
        listPrefab.bgImage.color = Color.white;
        listPrefab.button.transition = UnityEngine.UI.Selectable.Transition.None;
        listPrefab.button.interactable = false;

        listPrefab.button.onClick.RemoveAllListeners();
        listPrefab.textMesh.text = "<s>" + defaultTitle + "</s>";
    }

    void SetReadyForClaim()
    {
        claimed = false;
        listPrefab.bgImage.color = new Color(0.8071f, 0.9150f, 0.8301f);
        listPrefab.button.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
        listPrefab.button.interactable = true;

        listPrefab.button.onClick.RemoveAllListeners();
        listPrefab.button.onClick.AddListener(Claim);

        listPrefab.textMesh.text = defaultTitle;
    }

    void SetIncomplete()
    {
        claimed = false;
        listPrefab.bgImage.color = new Color(0.9622642f, 0.8787469f, 0.8578676f);
        listPrefab.button.transition = UnityEngine.UI.Selectable.Transition.None;
        listPrefab.button.interactable = false;
        listPrefab.textMesh.text = defaultTitle;

        listPrefab.button.onClick.RemoveAllListeners();
    }
}