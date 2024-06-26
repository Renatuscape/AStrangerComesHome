using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankManager : MonoBehaviour
{
    public GameObject bankMenu;

    public GameObject leaveButton;
    public GameObject exchangeButton;
    public GameObject loansButton;
    public GameObject chatButton;

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
        exchangeMenu.SetActive(false);
        accountMenu.SetActive(false);

        if (TransientDataScript.GetWeekDay() == DayOfWeek.Lunden)
        {
            leaveButton.SetActive(true);
            exchangeButton.SetActive(false);
            loansButton.SetActive(false);
            chatButton.SetActive(false);
            closedNotice.SetActive(true);
            background.sprite = SpriteFactory.GetBackgroundSprite("Teller_Bank_Closed");
        }
        else if (Player.GetCount("ARC004", "BankManager") > 0)
        {
            leaveButton.SetActive(true);
            exchangeButton.SetActive(true);
            loansButton.SetActive(true);
            chatButton.SetActive(true);
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
            chatButton.SetActive(true);
            closedNotice.SetActive(false);
            background.sprite = SpriteFactory.GetBackgroundSprite("Teller_Bank_Open");
        }

        UpdateWallet();
    }

    public void UpdateWallet()
    {
        var playerHellers = Player.GetCount("MIS000", "BankManager");
        var playerShillings = Player.GetCount("MIS001", "BankManager");
        var playerCrowns = Player.GetCount("MIS002", "BankManager");
        var playerGuilders = Player.GetCount("MIS003", "BankManager");
        var playerObols = Player.GetCount("MIS010", "BankManager");

        StartCoroutine(DelayedAdjust(guilders, playerGuilders));
        StartCoroutine(DelayedAdjust(crowns, playerCrowns));
        StartCoroutine(DelayedAdjust(shillings, playerShillings));
        StartCoroutine(DelayedAdjust(hellers, playerHellers));
        StartCoroutine(DelayedAdjust(oldWorldObols, playerObols));
        StartCoroutine(DelayedAdjust(playerTotal, MoneyExchange.GetPlayerMoney()));

    }

    IEnumerator DelayedAdjust(TextMeshProUGUI textMesh, int newNumber)
    {
        var textMeshContent = textMesh.text.Split(' ');
        var currentNumber = int.Parse(textMeshContent[1]);

        while (currentNumber != newNumber)
        {
            int difference = Mathf.Abs(newNumber - currentNumber);

            int incrementAmount;


            if (difference > 1000000)
            {
                incrementAmount = 1000000;
            }
            else if (difference > 100000)
            {
                incrementAmount = 100000;
            }
            else if (difference > 50000)
            {
                incrementAmount = 50000;
            }
            else if(difference > 10000)
            {
                incrementAmount = 10000;
            }
            else if (difference > 5000)
            {
                incrementAmount = 5000;
            }
            else if (difference > 1000)
            {
                incrementAmount = 500;
            }
            else
            {
                incrementAmount = difference > 20 ? 20 : 1;
            }

            if (currentNumber < newNumber)
            {
                currentNumber += incrementAmount;
            }
            else
            {
                currentNumber -= incrementAmount;
            }

            textMesh.text = textMeshContent[0] + " " + currentNumber;
            StartCoroutine(JumpNumber(textMesh));
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator JumpNumber(TextMeshProUGUI textMesh)
    {
        float nudgeAmount = 1f;
        textMesh.gameObject.transform.position = new Vector3(textMesh.transform.position.x + nudgeAmount, textMesh.transform.position.y + nudgeAmount, textMesh.transform.position.z);

        yield return new WaitForSeconds(0.03f);
        textMesh.gameObject.transform.position = new Vector3(textMesh.transform.position.x - nudgeAmount, textMesh.transform.position.y - nudgeAmount, textMesh.transform.position.z);
        AudioManager.PlaySoundEffect("handleCoins", -0.1f);
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
