using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum Currency
{
    Heller,
    Shilling,
    Crown,
    Guilder,
    Total
}
public class BankManager : MonoBehaviour
{
    public GameObject bankMenu;
    public GameObject dialogueBox;

    public GameObject leaveButton;
    public GameObject exchangeButton;
    public GameObject loansButton;
    public GameObject chatButton;
    public GameObject rewardsButton;

    public GameObject exchangeMenu;
    public GameObject accountMenu;

    public bool isBankActive;

    public TextMeshProUGUI guilders;
    public TextMeshProUGUI crowns;
    public TextMeshProUGUI shillings;
    public TextMeshProUGUI hellers;
    public TextMeshProUGUI oldWorldObols;

    public TextMeshProUGUI playerTotal;

    public GameObject closedNotice;
    public Image background;

    private void Update()
    {
        if (TransientDataScript.GameState != GameState.BankMenu)
        {
            isBankActive = false;
            leaveButton.SetActive(false);
            exchangeButton.SetActive(false);
            loansButton.SetActive(false);
            chatButton.SetActive(false);
            rewardsButton.SetActive(false);

            if (TransientDataScript.GameState != GameState.Dialogue)
            {
                isBankActive = false;
                bankMenu.SetActive(false);
            }

        }
        else if (TransientDataScript.GameState == GameState.BankMenu && !isBankActive)
        {
            isBankActive = true;
            OpenBankMenu();
        }
    }

    public void OpenBankMenu()
    {
        TransientDataScript.SetGameState(GameState.BankMenu, name, gameObject);

        bankMenu.SetActive(true);
        dialogueBox.SetActive(false);
        exchangeMenu.SetActive(false);
        accountMenu.SetActive(false);
        rewardsButton.SetActive(false);

        if (TransientDataScript.GetWeekDay() == DayOfWeek.Lunden)
        {
            leaveButton.SetActive(true);
            exchangeButton.SetActive(false);
            loansButton.SetActive(false);
            chatButton.SetActive(false);
            rewardsButton.SetActive(false);
            closedNotice.SetActive(true);
            background.sprite = SpriteFactory.GetBackgroundSprite("Teller_Bank_Closed");
        }
        else if (Player.GetCount("ARC004", "BankManager") > 0)
        {
            leaveButton.SetActive(true);
            exchangeButton.SetActive(true);
            loansButton.SetActive(true);
            chatButton.SetActive(true);
            rewardsButton.SetActive(true);
            closedNotice.SetActive(false);
            background.sprite = SpriteFactory.GetBackgroundSprite("Teller_Bank_Open");
        }
        else
        {
            //portraits.gameObject.SetActive(true);
            //portraits.EnableForShop("ARC004");
            leaveButton.SetActive(true);
            exchangeButton.SetActive(false);
            loansButton.SetActive(false);
            rewardsButton.SetActive(false);
            chatButton.SetActive(true);
            closedNotice.SetActive(false);
            background.sprite = SpriteFactory.GetBackgroundSprite("Teller_Bank_Open");
        }
    }

    public void CloseBankMenu()
    {
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }

    public void OpenExchangeMenu()
    {
        exchangeMenu.SetActive(true);
        accountMenu.SetActive(false);
    }

    public void OpenAccountMenu()
    {
        exchangeMenu.SetActive(false);
        accountMenu.SetActive(true);
    }

    public void ChatButton()
    {
        exchangeMenu.SetActive(false);
        accountMenu.SetActive(false);

        TransientDataScript.gameManager.storySystem.OpenTopicMenu("ARC004");
    }
}
