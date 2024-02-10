using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BankExchange : MonoBehaviour
{
    public BankManager bankManager;
    public TextMeshProUGUI commissionRate;
    public TextMeshProUGUI buySellValue;
    public Slider slider;

    public TextMeshProUGUI guilderPrice;
    public TextMeshProUGUI crownPrice;
    public TextMeshProUGUI shillingPrice;
    public TextMeshProUGUI obolPrice;

    int adjustedShillingPrice;
    int adjustedCrownPrice;
    int adjustedGuilderPrice;

    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.value = slider.minValue;
    }

    private void OnEnable()
    {
        CalculatePrices();
    }

    public void CalculatePrices()
    {
        var commission = MoneyExchange.CalculateCommission();
        commissionRate.text = commission + "%";

        float shillingsPriceAdjusted;
        float crownsPriceAdjusted;
        float guildersPriceAdjusted;

        shillingsPriceAdjusted = (100 + 100 * commission / 100) * slider.value;
        crownsPriceAdjusted = (MoneyExchange.crownValue + 100 * commission / 100) * slider.value;
        guildersPriceAdjusted = (MoneyExchange.guilderValue + 100 * commission / 100) * slider.value * 0.1f;

        adjustedShillingPrice = (int)Mathf.Ceil(shillingsPriceAdjusted);
        adjustedCrownPrice = (int)Mathf.Ceil(crownsPriceAdjusted);
        adjustedGuilderPrice = (int)Mathf.Ceil(guildersPriceAdjusted);

        shillingPrice.text = adjustedShillingPrice + " Hellers";
        crownPrice.text = adjustedCrownPrice + " Shilling";
        guilderPrice.text = adjustedGuilderPrice + " Crowns";
    }

    public void OnSliderValueChanged(float value)
    {
        buySellValue.text = slider.value.ToString();

        CalculatePrices();
    }

    public void SellHellers()
    {
        int sliderValue = (int)slider.value * 100;
        int inventoryHellers = Player.GetCount("MIS000-JUN-NN", "BankExchange, SellHellers()");
        bool result = false;

        int remainder = sliderValue - (sliderValue % 100);
        slider.value = remainder * 0.01f;
        var commissionCost = adjustedShillingPrice - remainder;

        if (commissionCost + remainder <= inventoryHellers)
        {
            result = MoneyExchange.ExchangeHellersToShillings(remainder, out int s);

            if (result)
            {
                AudioManager.PlayUISound("handleCoins2");
                Player.Remove("MIS000-JUN-NN", commissionCost);
            }
        }

        bankManager.UpdateWallet();
        Debug.Log($"Result of exchange: {result}. Attempted to exchange {remainder}. Calculated commission was {commissionCost} from price of {adjustedShillingPrice}");
    }

    public void BuyCrowns()
    {
        int sliderValue = (int)slider.value * 100;
        int inventoryShillings = Player.GetCount("MIS001-COM-NN", "BankExchange, BuyCrowns()");
        bool result = false;

        int remainder = sliderValue - (sliderValue % 100);
        slider.value = remainder * 0.01f;
        var commissionCost = adjustedCrownPrice - remainder;

        if (commissionCost + remainder <= inventoryShillings)
        {
            result = MoneyExchange.ExchangeShillingsToCrowns(remainder, out int crowns);

            if (result)
            {
                AudioManager.PlayUISound("handleCoins2");
                Player.Remove("MIS001-COM-NN", commissionCost);
            }
        }

        bankManager.UpdateWallet();
        Debug.Log($"Result of exchange: {result}. Attempted to exchange {remainder}. Calculated commission was {commissionCost} from price of {adjustedShillingPrice}");
    }

    public void BuyGuilders()
    {
        int sliderValue = (int)slider.value * 100;
        int inventoryCrowns = Player.GetCount("MIS002-UNC-NN", "BankExchange, BuyCrowns()");
        bool result = false;

        int remainder = sliderValue - (sliderValue % 100);
        slider.value = remainder * 0.01f;
        var commissionCost = adjustedGuilderPrice - remainder;

        if (commissionCost + remainder <= inventoryCrowns)
        {
            result = MoneyExchange.ExchangeCrownsToGuilders(remainder, out int crowns);

            if (result)
            {
                AudioManager.PlayUISound("handleCoins2");
                Player.Remove("MIS002-UNC-NN", commissionCost);
            }
        }

        bankManager.UpdateWallet();
        Debug.Log($"Result of exchange: {result}. Attempted to exchange {remainder}. Calculated commission was {commissionCost} from price of {adjustedShillingPrice}");
    }
}
