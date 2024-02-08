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
        slider.value = slider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        
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

        if (slider.value != slider.maxValue)
        {
            shillingsPriceAdjusted = (100 + 100 * commission / 100) * slider.value;

            crownsPriceAdjusted = (MoneyExchange.crownValue + 100 * commission / 100) * slider.value;

            guildersPriceAdjusted = (MoneyExchange.guilderValue + 100 * commission / 100) * slider.value;
        }
        else
        {
            var hellers = Player.GetCount("MIS000-JUN-NN", "BankExchange, CalculatePrices()");
            var shillings = Player.GetCount("MIS001-COM-NN", "BankExchange, CalculatePrices()");
            var crowns = Player.GetCount("MIS002-UNC-NN", "BankExchange, CalculatePrices()");
            var guilders = Player.GetCount("MIS003-RAR-NN", "BankExchange, CalculatePrices()");

            if (hellers >= 100)
            {
                shillingsPriceAdjusted = (100 + 100 * commission / 100) * hellers;
                var shillingsRemainder = shillings % 100;
                shillingsPriceAdjusted = shillingsPriceAdjusted - shillingsRemainder;
            }
            else
            {
                shillingsPriceAdjusted = 0;
            }

            if (shillings >= MoneyExchange.crownValue)
            {
                crownsPriceAdjusted = (MoneyExchange.crownValue + 100 * commission / 100) * shillings;
                var crownsRemainder = crowns % 100;
                crownsPriceAdjusted = crownsPriceAdjusted - crownsRemainder;
            }
            else
            {
                crownsPriceAdjusted = 0;
            }

            if (MoneyExchange.GetPlayerMoney() >= MoneyExchange.guilderValue)
            {
                guildersPriceAdjusted = (MoneyExchange.guilderValue + 100 * commission / 100) * crowns;

                var guildersRemainder = guilders % 100;

                guildersPriceAdjusted = guildersPriceAdjusted - guildersRemainder;
            }
            else
            {
                guildersPriceAdjusted = 0;
            }
        }

        adjustedShillingPrice = (int)Mathf.Ceil(shillingsPriceAdjusted);
        adjustedCrownPrice = (int)Mathf.Ceil(crownsPriceAdjusted);
        adjustedGuilderPrice = (int)Mathf.Ceil(guildersPriceAdjusted);

        shillingPrice.text = adjustedShillingPrice + " Hellers";
        crownPrice.text = adjustedCrownPrice + " Shilling";
        guilderPrice.text = adjustedGuilderPrice + " Shilling";
    }

    public void OnSliderValueChanged(float value)
    {
        if (slider.value == slider.maxValue)
        {
            buySellValue.text = "Max";
        }
        else
        {
            buySellValue.text = slider.value.ToString();
        }

        CalculatePrices();
    }

    public void SellHellers()
    {
        int sliderValue = (int)slider.value * 100;
        int inventoryHellers = Player.GetCount("MIS000-JUN-NN", "BankExchange, SellHellers()");
        bool result = false;

        if (slider.value == slider.maxValue)
        {
            sliderValue = inventoryHellers;
        }

        int remainder = sliderValue - (sliderValue % 100);
        slider.value = remainder * 0.01f;
        var commissionCost = adjustedShillingPrice - remainder;

        if (commissionCost + remainder <= inventoryHellers)
        {
            result = MoneyExchange.ExchangeHellersToShillings(remainder, out int sovereign);

            if (result)
            {
                Player.Remove("MIS000-JUN-NN", commissionCost);
            }
        }
        else
        {
            if (slider.value > slider.minValue)
            {
                slider.value--;
                SellHellers();
            }
        }

        bankManager.UpdateWallet();

        Debug.Log($"Result of exchange: {result}. Attempted to exchange {remainder}. Calculated commission was {commissionCost} from price of {adjustedShillingPrice}");
    }
}
