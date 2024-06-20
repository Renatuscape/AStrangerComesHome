using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildMenu : MonoBehaviour
{
    public PortraitRenderer portraitRenderer;
    public Character character;
    public GuildRewardsMenu rewardsMenu;
    public Button btnOpenRewardsMenu;
    public TextMeshProUGUI textMeshPassengers;
    public TextMeshProUGUI textMeshFare;
    public int totalPassengers;
    public int totalFare;

    void Start()
    {
        if (TransientDataScript.GameState != GameState.ShopMenu)
        {
            gameObject.SetActive(false);
        }
    }

    public void Initialise(Character character)
    {
        this.character = character;
        totalPassengers = Player.GetCount(StaticTags.TotalPassengers, name);
        totalFare = Player.GetCount(StaticTags.TotalFare, name);
        textMeshPassengers.text = "Total passengers: " + totalPassengers;
        textMeshFare.text = "Total fare earned: " + totalFare;
        TransientDataScript.SetGameState(GameState.ShopMenu, name, gameObject);

        gameObject.SetActive(true);

        portraitRenderer.EnableForGarage(character.objectID);
        btnOpenRewardsMenu.onClick.RemoveAllListeners();
        btnOpenRewardsMenu.onClick.AddListener(() => rewardsMenu.OpenRewardsMenu(character.objectID == StaticTags.Guildmaster, totalPassengers, totalFare));
    }
}
