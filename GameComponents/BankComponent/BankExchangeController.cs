using System;
using UnityEngine;

public class BankExchangeController : MonoBehaviour
{
    public Currency currencyToBuy = Currency.Shilling;
    public Currency currencyToSell = Currency.Heller;

    public int amountToBuy;
    public int adjustedExchangePrice;
    public float commission;
    public bool PerformExchange(out string errorMessage)
    {
        Debug.Log($"Attempting to exchange {adjustedExchangePrice} {currencyToSell} for {amountToBuy} {currencyToBuy}.");
        string sellID = MoneyExchange.GetCurrency(currencyToSell).objectID;

        if (Player.GetCount(sellID, gameObject.name) >= adjustedExchangePrice)
        {
            var buyData = MoneyExchange.GetCurrency(currencyToBuy);

            if (Player.GetCount(buyData.objectID, gameObject.name) + amountToBuy <= buyData.max)
            {
                Player.Remove(sellID, adjustedExchangePrice);

                Player.Add(buyData.objectID, amountToBuy);

                Debug.Log("Puchase succeeded.");
                errorMessage = "";
                return true;
            }
            else
            {
                Debug.Log("Puchase failed due to max inventory.");
                errorMessage = "Not enough inventory space for that many " + currencyToBuy.ToString().ToLower() + "s. Cannot hold more than " + buyData.max + ".";
                return false;
            }
        }

        Debug.Log("Puchase failed due to low amount.");
        errorMessage = "Not enough " + currencyToSell.ToString().ToLower() + "s to complete exchange.";
        return false;
    }

    public void CalculateExchangePrice()
    {
        Debug.Log("Calculate Exchange Price was called.");
        var currencyToSellData = MoneyExchange.GetCurrency(currencyToSell);
        var currencyToBuyData = MoneyExchange.GetCurrency(currencyToBuy);

        if (currencyToBuy < currencyToSell)
        {
            var valueOfPurchase = amountToBuy * currencyToBuyData.value;
            var priceOfPurchase = valueOfPurchase / currencyToSellData.value;

            //No commission when exchanging for a lower denomination

            adjustedExchangePrice = Mathf.CeilToInt(priceOfPurchase / currencyToBuyData.value);
        }
        else
        {
            var priceOfPurchase = amountToBuy * (currencyToBuyData.value / currencyToSellData.value);

            float commissionCost = (priceOfPurchase * commission) / 100;
            float amountAfterCommission = priceOfPurchase + commissionCost;

            adjustedExchangePrice = Mathf.CeilToInt(amountAfterCommission);
        }
    }

    public void ExchangeMaxHellers()
    {
        int hellersInInventory = Player.GetCount(StaticTags.Heller, gameObject.name);
        int shillingsInInventory = Player.GetCount(StaticTags.Shilling, gameObject.name);
        int maxShillings = MoneyExchange.GetCurrency(Currency.Shilling).max;

        int leftover = hellersInInventory % 100;

        int totalToSell = hellersInInventory - leftover;
        int shillingsToAdd = totalToSell / 100;

        if (shillingsInInventory + shillingsToAdd > maxShillings)
        {
            int difference = shillingsInInventory + shillingsToAdd - maxShillings;
            shillingsToAdd -= difference;
            totalToSell -= difference * 100;
        }

        Player.Remove(StaticTags.Heller, totalToSell);
        Player.Add(StaticTags.Shilling, shillingsToAdd);
    }
}