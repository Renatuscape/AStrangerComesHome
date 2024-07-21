using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankExchangeUi : MonoBehaviour
{
    public BankExchangeController exchangeController;
    public TextMeshProUGUI commissionRate;
    public TextMeshProUGUI exchangePriceText;
    public TextMeshProUGUI valueToBuy;
    public TextMeshProUGUI hellersToSellText;
    public TextMeshProUGUI shillingsForHellersText;
    public ReusableDialogueBox tellerDialogue;

    public Button btnAttemptExchange;
    public Button btnHellerMaxExchange;

    public TMP_Dropdown dropdownBuy;
    public TMP_Dropdown dropdownSell;
    public Slider toBuySlider;
    bool isReady = false;
    private void Setup()
    {
        //PopulateDropdown(dropdownBuy);
        //PopulateDropdown(dropdownSell);

        dropdownBuy.onValueChanged.AddListener(delegate
        {
            SelectCurrency(dropdownBuy);
        });
        dropdownSell.onValueChanged.AddListener(delegate
        {
            SelectCurrency(dropdownSell);
        });

        toBuySlider.onValueChanged.AddListener(delegate
        {
            OnSliderChange();
        });

        btnAttemptExchange.onClick.AddListener(() => AttemptExchange());
        btnHellerMaxExchange.onClick.AddListener(() => ExchangeAllHellers());

        dropdownBuy.value = 1;
        dropdownSell.value = 0;
        toBuySlider.value = 1;

        isReady = true;
    }

    private void OnEnable()
    {
        if (!isReady)
        {
            Setup();
        }

        exchangeController.commission = MoneyExchange.CalculateCommission();
        commissionRate.text = exchangeController.commission + "%";

        SelectCurrency(dropdownBuy);
        SelectCurrency(dropdownSell);
        OnSliderChange();
        CalculateExchangePrice();
        UpdateHellerInfo();
    }

    void CalculateExchangePrice()
    {
        if (isReady)
        {
            exchangeController.CalculateExchangePrice();
            exchangePriceText.text = exchangeController.adjustedExchangePrice.ToString();
        }
    }

    void UpdateHellerInfo()
    {
        int hellersInInventory = Player.GetCount(StaticTags.Heller, gameObject.name);
        int shillingsInInventory = Player.GetCount(StaticTags.Shilling, gameObject.name);
        int maxShillings = MoneyExchange.GetCurrency(Currency.Shilling).max;

        if (hellersInInventory < 100 || shillingsInInventory >= maxShillings)
        {
            btnHellerMaxExchange.interactable = false;
            hellersToSellText.text = "0";
            shillingsForHellersText.text = "0";
        }
        else
        {
            btnHellerMaxExchange.interactable = true;

            int leftover = hellersInInventory % 100;

            int totalToSell = hellersInInventory - leftover;
            int shillingsToAdd = totalToSell / 100;

            if (shillingsInInventory + shillingsToAdd > maxShillings)
            {
                int difference = shillingsInInventory + shillingsToAdd - maxShillings;
                shillingsToAdd -= difference;
                totalToSell -= difference * 100;
            }

            hellersToSellText.text = totalToSell.ToString();
            shillingsForHellersText.text = shillingsToAdd.ToString();
        }
    }

    public void OnSliderChange()
    {
        if (isReady)
        {
            var currencyToSellData = MoneyExchange.GetCurrency(exchangeController.currencyToSell);
            var currencyToBuyData = MoneyExchange.GetCurrency(exchangeController.currencyToBuy);

            if (exchangeController.currencyToBuy < exchangeController.currencyToSell)
            {
                int calculatedValue = Mathf.CeilToInt(toBuySlider.value * (currencyToSellData.value / currencyToBuyData.value));
                valueToBuy.text = calculatedValue.ToString();
                exchangeController.amountToBuy = Mathf.CeilToInt(calculatedValue);
            }
            else
            {
                valueToBuy.text = toBuySlider.value.ToString();
                exchangeController.amountToBuy = Mathf.CeilToInt(toBuySlider.value);

                //int calculatedValue = Mathf.CeilToInt(toBuySlider.value * (currencyToBuyData.value / currencyToSellData.value));
                //valueToBuy.text = calculatedValue.ToString();
                //exchangeController.amountToBuy = Mathf.CeilToInt(calculatedValue);
            }

            CalculateExchangePrice();
        }
    }

    public void SelectCurrency(TMP_Dropdown dropdown)
    {
        if (isReady)
        {
            if (dropdown == dropdownBuy)
            {
                exchangeController.currencyToBuy = (Currency)dropdown.value + 1;
            }
            else
            {
                exchangeController.currencyToSell = (Currency)dropdown.value + 1;
            }

            OnSliderChange();
        }
    }

    public void AttemptExchange()
    {
        tellerDialogue.OpenAndPrintText($"Exchange {exchangeController.adjustedExchangePrice} {exchangeController.currencyToSell.ToString().ToLower()}" +
            $"{(exchangeController.adjustedExchangePrice > 1 ? "s" : "")}" +
            $" for {exchangeController.amountToBuy} {exchangeController.currencyToBuy.ToString().ToLower()}" +
            $"{(exchangeController.amountToBuy > 1 ? "s" : "")}?" +
            $"\n\nCommission is not applied when exchanging for a lower denomination.",
            ConfirmExchange);
    }

    public void ConfirmExchange()
    {
        if (exchangeController.PerformExchange(out var errorMessage))
        {
            Debug.Log("Exchange returned true.");
        }
        else
        {
            tellerDialogue.OpenAndPrintText(errorMessage);
        }
    }

    public void ExchangeAllHellers()
    {
        exchangeController.ExchangeMaxHellers();
        UpdateHellerInfo();
    }
}
