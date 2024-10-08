using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractMenu : MonoBehaviour
{
    public static InteractMenu instance;
    public StorySystem storySystem;
    public MenuSystem menuSystem;
    public CharacterCreator characterCreator;
    public List<GameObject> buttons;
    public GameObject prefabContainer;

    void Awake()
    {
        instance = GetComponent<InteractMenu>();
    }
    public void Initialise(Character character, Shop shop = null)
    {
        TransientDataScript.SetGameState(GameState.Dialogue, name, gameObject);
        // Debug.Log($"Initialising interact menu with {character.name}");

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

            if (character.canAccessGarage)
            {
                PrintGarageButton(character);
            }

            if (character.canAccessGuild)
            {
                PrintGuildButton(character);
            }

            if (character.canEditCharacter)
            {
                PrintEditCharacterButton(character);
            }

            if (character.type != CharacterType.Generic)
            {
                PrintGiftButton(character);
            }
        }

        gameObject.SetActive(true);
        StartCoroutine(UpdateLayout());
        PrintLeaveButton();
    }

    IEnumerator UpdateLayout()
    {
        Canvas.ForceUpdateCanvases();
        prefabContainer.GetComponent<VerticalLayoutGroup>().spacing = 4.8f;
        yield return null;
        prefabContainer.GetComponent<VerticalLayoutGroup>().spacing = 4.9f;
        Canvas.ForceUpdateCanvases();
        yield return null;
        prefabContainer.GetComponent<VerticalLayoutGroup>().spacing = 5;
        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
    }

    void PrintChatButton(Character character)
    {
        prefabContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        Canvas.ForceUpdateCanvases();

        var chatButton = GetButton($"Chat with {character.NamePlate()}");
        chatButton.GetComponent<Button>().onClick.AddListener(() => storySystem.OpenTopicMenu(character.objectID));

        prefabContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
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
            var shopButton = GetButton($"I should speak with my old friend before shopping");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else if (character.type == CharacterType.Unique && Player.GetCount(character.objectID, name) < 1)
        {
            var shopButton = GetButton($"I should introduce myself before shopping");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else
        {
            var shopButton = GetButton($"Browse {shop.name}");
            shopButton.GetComponent<Button>().onClick.AddListener(() => menuSystem.shopMenu.SetUpShop(character, shop));
            return true;
        }

    }

    bool PrintGarageButton(Character character)
    {
        string machinistID = StaticTags.Machinist;
        int machinistAffection = Player.GetCount(machinistID, name);
        
        if (machinistAffection < 1 && character.objectID == machinistID)
        {
            var shopButton = GetButton($"I should speak with my old friend before entering the garage");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else if (machinistAffection < 1 && character.objectID != machinistID)
        {
            var shopButton = GetButton($"I should visit |the Machinist| before entering the garage");
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

    bool PrintGuildButton(Character character)
    {
        string guildmasterID = StaticTags.Guildmaster;
        int guildLicense = Player.GetCount(StaticTags.GuildLicense, name);

        if (guildLicense < 1)
        {
            var shopButton = GetButton($"I should ask the Guildmaster about a license");
            shopButton.GetComponent<Button>().onClick.RemoveAllListeners();
            return false;
        }
        else
        {
            var shopButton = GetButton($"Enter guild office");
            shopButton.GetComponent<Button>().onClick.AddListener(() => menuSystem.guildMenu.Initialise(character));
            return true;
        }
    }

    void PrintGiftButton(Character character)
    {
        if (Player.GetCount(character.objectID, name) >= character.giftableLevel)
        {
            if (TransientDataScript.GiftCheck(character))
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

    void PrintEditCharacterButton(Character character)
    {
        if (RequirementChecker.CheckRequirements(character.editRequirements))
        {
            if (Player.GetCount(StaticTags.ResurrectionEssence, name) >= 1)
            {
                var editCharButton = GetButton("Use Resurrection Essence to change appearance");
                editCharButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    TransientDataScript.SetGameState(GameState.CharacterCreation, "Interact Menu", gameObject);
                    characterCreator.gameObject.SetActive(true);
                });
            }
            else
            {
                var editCharButton = GetButton($"{character.NamePlate()} wants Resurrection Essence");
                editCharButton.GetComponent<Button>().enabled = false;
            }
        }

        // Print nothing if checks fail
    }

    void PrintLeaveButton()
    {
        var leaveButton = GetButton($"Leave");
        buttons.Add(leaveButton);

        leaveButton.GetComponent<Button>().onClick.AddListener(() => TransientDataScript.SetGameState(GameState.Overworld, name, gameObject));
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
