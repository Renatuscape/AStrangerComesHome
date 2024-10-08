﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankAccountMenu : MonoBehaviour
{
    public ReusableDialogueBox tellerDialogue;
    public TextMeshProUGUI accountBalanceText;
    public TextMeshProUGUI currentDebtText;
    public TextMeshProUGUI balanceSliderText;
    public TextMeshProUGUI debtSliderText;
    public Slider balanceSlider;
    public Slider debtSlider;

    public Button btnDeposit;
    public Button btnWithdraw;
    public Button btnRepay;
    public Button btnViewProjects;

    public int debt;
    public int balance;

    CurrencyData guilderData;
    bool isReady = false;
    void Setup()
    {
        guilderData = MoneyExchange.GetCurrency(Currency.Guilder);

        btnDeposit.onClick.AddListener(() => Deposit());
        btnWithdraw.onClick.AddListener(() => Withdraw());
        btnRepay.onClick.AddListener(() => Repay());

        balanceSlider.onValueChanged.AddListener(delegate
        {
            BalanceSliderValueChange();
        });
        debtSlider.onValueChanged.AddListener(delegate
        {
            DebtSliderValueChange();
        });

        isReady = true;
    }

    private void OnEnable()
    {
        if (!isReady)
        {
            Setup();
        }

        UpdateDebtValues();
        UpdateBalanceValues();
    }

    public void DebtSliderValueChange()
    {
        UpdateDebtValues();
    }

    public void BalanceSliderValueChange()
    {
        UpdateBalanceValues();
    }
    public void Repay()
    {
        var shillings = Player.GetCount(StaticTags.Shilling, name);
        int repayment = 0;

        if (shillings >= debtSlider.value && debtSlider.value <= debt)
        {
            repayment = Mathf.CeilToInt(debtSlider.value);
        }
        else if (debtSlider.value > debt)
        {
            var difference = debtSlider.value - debt;
            repayment = Mathf.CeilToInt(debtSlider.value - difference);
        }
        else if (shillings < debtSlider.value)
        {
            repayment = shillings;
        }

        Player.Remove(StaticTags.CurrentDebt, repayment);
        Player.Remove(StaticTags.Shilling, repayment);
        UpdateDebtValues();

        if (debt == 0)
        {
            PrintMessage("DELIGHTED", "Congratulations!\nYou have repaid your loan in full.");
        }
    }

    public void Withdraw()
    {
        var guilders = Player.GetCount(StaticTags.Guilder, name);
        int withdrawal;

        if (guilders + balanceSlider.value <= guilderData.max)
        {
            withdrawal = Mathf.CeilToInt(balanceSlider.value);
        }
        else if (balance < balanceSlider.value)
        {
            withdrawal = Mathf.CeilToInt(balance);

            PrintMessage("ANNOYED", "You cannot withdraw more money than you have in your account. I took the liberty of withdrawing everything for you.");
        }
        else
        {
            var difference = guilders + balanceSlider.value - guilderData.max;

            withdrawal = Mathf.CeilToInt(balanceSlider.value - difference);

            PrintMessage("THINKING", $"You do not have the space for that many guilders in your wallet. I can only give you {balanceSlider.value - difference} at the moment.");
        }

        Player.Remove(StaticTags.AccountBalance, withdrawal);
        Player.Add(StaticTags.Guilder, Mathf.CeilToInt(withdrawal));
        UpdateBalanceValues();
    }

    public void Deposit()
    {
        var guilders = Player.GetCount(StaticTags.Guilder, name);
        int deposit;

        if (guilders >= balanceSlider.value) //guilders + balanceSlider.value <= guilderData.max)
        {
            deposit = Mathf.CeilToInt(balanceSlider.value);
        }
        else
        {
            deposit = guilders;
            PrintMessage("DISGUSTED", $"It would be wonderful if you had that many guilders to deposit, but you do not. Let us deposit all {guilders} guilders from your wallet.");
        }

        Player.Remove(StaticTags.Guilder, deposit);
        Player.Add(StaticTags.AccountBalance, Mathf.CeilToInt(deposit));
        UpdateBalanceValues();
    }

    void PrintMessage(string expression, string message)
    {
        tellerDialogue.OpenAndPrintText(message, expression);
    }

    void UpdateDebtValues()
    {
        debt = Player.GetCount(StaticTags.CurrentDebt, name);
        var shillings = Player.GetCount(StaticTags.Shilling, name);

        if (debt > shillings)
        {
            debtSlider.maxValue = shillings;
        }
        else
        {
            debtSlider.maxValue = debt;
        }

        if (debt == 0)
        {
            debtSlider.gameObject.SetActive(false);
            btnRepay.interactable = false;
            debtSliderText.text = "No Debt";
        }
        else if (shillings == 0)
        {
            debtSlider.gameObject.SetActive(false);
            btnRepay.interactable = false;
            debtSliderText.text = "No Shillings";
        }
        else
        {
            debtSlider.gameObject.SetActive(true);
            btnRepay.interactable = true;
            debtSliderText.text = "Repay " + debtSlider.value;
        }

        currentDebtText.text = debt + " shillings";
    }

    void UpdateBalanceValues()
    {
        balance = Player.GetCount(StaticTags.AccountBalance, name);
        var guilders = Player.GetCount(StaticTags.Guilder, name);

        if (guilders < 1 || guilders == 0)
        {
            btnDeposit.interactable = false;
        }
        else
        {
            btnDeposit.interactable = true;
        }

        if (balance < 1 || guilders == guilderData.max)
        {
            btnWithdraw.interactable = false;
        }
        else
        {
            btnWithdraw.interactable = true;
        }

        accountBalanceText.text = balance + " guilders";
        balanceSliderText.text = balanceSlider.value.ToString();
    }
}
