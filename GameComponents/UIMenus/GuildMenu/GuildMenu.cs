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
            rewardsMenu.gameObject.SetActive(false);
        }
    }

    public void Initialise(Character character)
    {
        if (rewardsMenu.gameObject.activeInHierarchy)
        {
            rewardsMenu.gameObject.SetActive(false);
        }

        this.character = character;
        totalPassengers = Player.GetCount(StaticTags.TotalPassengers, name);
        totalFare = Player.GetCount(StaticTags.TotalFare, name);
        textMeshPassengers.text = "Total passengers: " + totalPassengers;
        textMeshFare.text = "Total fare earned: " + totalFare;
        TransientDataScript.SetGameState(GameState.ShopMenu, name, gameObject);

        gameObject.SetActive(true);

        portraitRenderer.EnableForShop(character.objectID);
        InitialiseRewardsMenu();

        btnOpenRewardsMenu.onClick.RemoveAllListeners();
        btnOpenRewardsMenu.onClick.AddListener(() => ToggleRewardsMenu());
    }

    void InitialiseRewardsMenu()
    {
        rewardsMenu.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 1000, 0);
        rewardsMenu.Initialise(character.objectID == StaticTags.Guildmaster, totalPassengers, totalFare);
    }

    public void ToggleRewardsMenu()
    {
        if (rewardsMenu.transform.localPosition.y > -120)
        {
            rewardsMenu.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, -120, 0);
        }
        else
        {
            rewardsMenu.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, 1000, 0);
        }
    }

    public void Close()
    {
        rewardsMenu.gameObject.SetActive(false);
        gameObject.SetActive(false);
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }
}
