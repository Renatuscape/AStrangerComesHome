using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class InteractMenu : MonoBehaviour
{
    public static InteractMenu instance;
    public StorySystem storySystem;
    public MenuSystem menuSystem;
    public List<GameObject> buttons;
    public GameObject prefabContainer;

    void Awake()
    {
        instance = GetComponent<InteractMenu>();
    }
    public void Initialise(Character character, Shop shop = null)
    {
        TransientDataCalls.SetGameState(GameState.Dialogue, name, gameObject);
        Debug.Log($"Initialising interact menu with {character.name}");

        buttons = new();

        if (character != null && !string.IsNullOrEmpty(character.objectID))
        {
            PrintChatButton(character);

            if (shop == null || string.IsNullOrEmpty(shop.objectID))
            {
                FindViableShops(character);
            }
            else
            {
                PrintShopButton(character, shop);
            }

            if (character.type != CharacterType.Generic)
            {
                PrintGiftButton(character);
            }

            if (character.runsGarage)
            {
                PrintGarageButton(character);
            }
        }

        gameObject.SetActive(true);
        PrintLeaveButton();
    }

    void PrintChatButton(Character character)
    {
        var chatButton = GetButton($"Chat with {character.name}");
        chatButton.GetComponent<Button>().onClick.AddListener(() => storySystem.OpenTopicMenu(character.objectID));
    }

    void FindViableShops(Character character)
    {
        foreach (var shop in character.shops)
        {
            if (shop.CheckRequirements())
            {
                var isReadyToShop = PrintShopButton(character, shop);

                if (!isReadyToShop)
                {
                    break; //Do not print multiple warnings about speaking to your friend or introducing yourself
                }
            }
        }
    }

    bool PrintShopButton(Character character, Shop shop)
    {
        if (character.type == CharacterType.Arcana && Player.GetCount(character.objectID, name) < 1)
        {
            var shopButton = GetButton($"I should speak with my old friend before shopping.");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else if (character.type == CharacterType.Unique && Player.GetCount(character.objectID, name) < 1)
        {
            var shopButton = GetButton($"I should introduce myself before shopping.");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else
        {
            var shopButton = GetButton($"Shop at {shop.name}");
            shopButton.GetComponent<Button>().onClick.AddListener(() => menuSystem.shopMenu.SetUpShop(character, shop));
            return true;
        }

    }

    bool PrintGarageButton(Character character)
    {
        string machinistID = "ARC002";
        int machinistAffectio = Player.GetCount(machinistID, name);
        
        if (machinistAffectio < 1 && character.objectID == machinistID)
        {
            var shopButton = GetButton($"I should speak with my old friend before entering the garage.");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else if (machinistAffectio < 1 && character.objectID != machinistID)
        {
            var shopButton = GetButton($"I should visit |the Machinist| before entering the garage.");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else
        {
            var shopButton = GetButton($"Enter garage");
            shopButton.GetComponent<Button>().onClick.AddListener(() => menuSystem.garageMenu.Initialise(character));
            return true;
        }
    }

    void PrintGiftButton(Character character)
    {
        if (Player.GetCount(character.objectID, name) > 0)
        {
            if (TransientDataCalls.GiftCheck(character))
            {
                var giftButton = GetButton($"Gift already given");
                giftButton.GetComponent<Button>().enabled = false;
            }

            else
            {
                var giftButton = GetButton($"Gift");
                giftButton.GetComponent<Button>().onClick.AddListener(() => menuSystem.giftMenu.Setup(character));
            }
        }
    }

    void PrintLeaveButton()
    {
        var leaveButton = GetButton($"Leave");
        buttons.Add(leaveButton);

        leaveButton.GetComponent<Button>().onClick.AddListener(() => TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject));
    }

    GameObject GetButton(string buttonText)
    {
        var parsedText = DialogueTagParser.ParseText(buttonText);
        var button = BoxFactory.CreateButton(parsedText);
        button.transform.SetParent(prefabContainer.transform);
        buttons.Add(button);
        button.GetComponent<Button>().onClick.AddListener(() => CloseInteractMenu());

        return button;
    }

    public void CloseInteractMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }

        buttons.Clear();
    }

    public static void Open(Character character, Shop shop = null)
    {
        if (instance == null)
        {
            Debug.LogWarning("Instance of InteractMenu was null.");
        }
        else
        {
            if (shop == null)
            {
                instance.Initialise(character);
            }
            else
            {
                instance.Initialise(character, shop);
            }
        }
    }
}
