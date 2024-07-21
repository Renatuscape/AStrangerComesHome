using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[Serializable]
public class CurrencyData
{
    public Currency type;
    public string objectID;
    public float value;
    public int max;
}
public static class MoneyExchange
{
    public static List<CurrencyData> currencyManifest = new()
    {
        new ()
        {
            type = Currency.Heller,
            objectID = StaticTags.Heller,
            value = 0.1f,
            max = 5000
        },
        new ()
        {
            type = Currency.Shilling,
            objectID = StaticTags.Shilling,
            value = 1,
            max = 3000

        },
        new ()
        {
            type = Currency.Crown,
            objectID = StaticTags.Crown,
            value = 10,
            max = 500
        },
        new ()
        {
            type = Currency.Guilder,
            objectID = StaticTags.Guilder,
            value = 100,
            max = 150
        }
    };

    public static CurrencyData GetCurrency(Currency currency)
    {
        var foundData = currencyManifest.FirstOrDefault(c => c.type == currency);

        if (foundData == null)
        {
            Debug.Log("Found data for currency returned null: " + currency.ToString());
        }

        return foundData;
    }

    public static Character teller;
    public static DayOfWeek freeExchangeDay = DayOfWeek.Solden;

    public static int playerHellers;
    public static int playerShillings;
    public static int playerCrowns;
    public static int playerGuilders;

    public static float commission = 0.3f;

    public static Character GetTeller()
    {
        if (Characters.all.Count > 0)
        {
            if (teller == null)
            {
                teller = Characters.FindByTag("Teller", "Money Manager, GetTeller()");
            }

            return teller;
        }
        else
        {
            Debug.Log("MoneyExchange tried to find Teller, but characters were not yet loaded from Json.");
            return null;
        }
    }

    public static int GetPlayerMoney()
    {
        int total = 0;
        playerHellers = Player.GetCount(StaticTags.Heller, "Money Manager, GetPlayerMoney()"); // get copper
        playerShillings = Player.GetCount(StaticTags.Shilling, "Money Manager, GetPlayerMoney()"); // get silver
        playerCrowns = Player.GetCount(StaticTags.Crown, "Money Manager, GetPlayerMoney()");
        playerGuilders = Player.GetCount(StaticTags.Guilder, "Money Manager, GetPlayerMoney()");

        total += playerShillings; // get silver
        total += playerCrowns * (int)GetCurrency(Currency.Crown).value;
        total += playerGuilders * (int)GetCurrency(Currency.Guilder).value;

        return total;
    }

    public static int GetSpaceInWallet()
    {
        int total = 0;

        var shillingData = GetCurrency(Currency.Shilling);
        var crownData = GetCurrency(Currency.Crown);
        var guilderData = GetCurrency(Currency.Guilder);

        playerShillings = Player.GetCount(StaticTags.Shilling, "Money Manager, GetPlayerMoney()"); // get silver
        playerCrowns = Player.GetCount(StaticTags.Crown, "Money Manager, GetPlayerMoney()");
        playerGuilders = Player.GetCount(StaticTags.Guilder, "Money Manager, GetPlayerMoney()");

        total += shillingData.max - playerShillings;
        total += Mathf.CeilToInt((crownData.max - playerCrowns) * crownData.value);
        total += Mathf.CeilToInt(guilderData.max - playerGuilders);
        return total;
    }

    #region Buy/Sell Methods
    //  BUY/SELL METHODS

    public static bool Purchase(int costInShillings)
    {
        if (GetPlayerMoney() < costInShillings) //get total and update the values with current data
        {
            return false; //If this doesn't trigger, exchange to silver should be successful
        }
        else
        {
            Debug.Log($"Attempting to purchase. Cost: {costInShillings}, total: {GetPlayerMoney()}");

            int totalPaid = 0;

            while (totalPaid < costInShillings)
            {
                int shillingsInInventory = Player.GetCount(StaticTags.Shilling, "Money Manager, Purchase()");
                int crownsInInventory = Player.GetCount(StaticTags.Crown, "Money Manager, Purchase()");
                int guildersInInventory = Player.GetCount(StaticTags.Guilder, "Money Manager, Purchase()");

                if (shillingsInInventory >= costInShillings - totalPaid)
                {
                    Player.Remove(StaticTags.Shilling, costInShillings - totalPaid, true);
                    totalPaid += costInShillings - totalPaid;
                }
                else // Exchange higher denominations
                {
                    // If not enough shillings, try to exchange higher denominations
                    if (crownsInInventory > 0)
                    {
                        var crownExchange = CurrencyExchange(Currency.Crown, Currency.Shilling, true, 1, out var shillings);
                        if (!crownExchange)
                        {
                            return false;
                        }
                        else
                        {
                            Debug.Log($"Exchanged 1 crown for {shillings} shillings.");
                        }
                    }
                    else if (guildersInInventory > 0)
                    {
                        var guilderExchange = CurrencyExchange(Currency.Guilder, Currency.Shilling, true, 1, out var shillings);

                        if (!guilderExchange)
                        {
                            return false;
                        }
                        else
                        {
                            Debug.Log($"Exchanged 1 guilder for {shillings} shillings.");
                        }
                    }
                    else
                    {
                        Debug.Log($"All exchanges failed.");
                        return false; // Unable to exchange
                    }

                    shillingsInInventory = Player.GetCount(StaticTags.Shilling, "Money Manager, Purchase()");

                    if (shillingsInInventory >= costInShillings - totalPaid)
                    {
                        Player.Remove(StaticTags.Shilling, costInShillings - totalPaid, true);
                        totalPaid += costInShillings - totalPaid;
                    }
                    else
                    {
                        Player.Remove(StaticTags.Shilling, shillingsInInventory, true);
                        totalPaid += shillingsInInventory;
                    }
                }
            }

            if (totalPaid == costInShillings)
            {
                Debug.Log($"Purchase success with conversion. Cost: {costInShillings}, paid: {totalPaid}, new total: {GetPlayerMoney()}");
                return true; // Purchase successful
            }

            Debug.Log($"Failed outside while loop with totalPaid: {totalPaid} of price {costInShillings}.");
            return false; // Unable to purchase
        }
    }

    public static int CalculateSellPrice(Item item)
    {
        int price = item.basePrice;

        float evaluation = 0.1f;
        int judgement = Player.GetCount(StaticTags.Judgement, "CalculateSellPrice");
        int rhetorics = Player.GetCount(StaticTags.Rhetorics, "CalculateSellPrice");

        evaluation += judgement * 0.05f;
        evaluation += rhetorics * 0.05f;

        Mathf.FloorToInt(price * evaluation);

        if (price < 1)
        {
            price = 1;
        }

        // Debug.Log($"Item({item.objectID}) base price: {item.basePrice}. Evaluation: {evaluation}. Returned price: {price}.");
        return price;
    }

    public static bool AddHighestDenomination(int valueToAddInShillings, bool disregardInventoryLimit, out int totalAdded) {
        Debug.Log($"Attempting to add {valueToAddInShillings} using highest denomination.");
        var guilderData = GetCurrency(Currency.Guilder);
        var crownData = GetCurrency(Currency.Crown);

        if (!disregardInventoryLimit && GetSpaceInWallet() < valueToAddInShillings)
        {
            Debug.Log($"Transaction failed. Not enough space in wallet.");
            totalAdded = 0;
            return false;
        }

        int crowns = 0;
        int shillings = 0;
        int guilders = 0;

        while (valueToAddInShillings > 0)
        {
            Debug.Log($"Value was {valueToAddInShillings}. Testing against guilders: {guilderData.value}, crowns: {crownData.value} and shillings: 1.");

            if (valueToAddInShillings >= guilderData.value && Player.GetCount(guilderData.objectID, "AddHighest") < guilderData.max)
            {
                valueToAddInShillings -= (int)guilderData.value;
                guilders++;
            }
            else if (valueToAddInShillings >= crownData.value && Player.GetCount(crownData.objectID, "AddHighest") < crownData.max)
            {
                valueToAddInShillings -= (int)crownData.value;
                crowns++;
            }
            else
            {
                valueToAddInShillings -= 1;
                shillings++;
            }
        }

        var guildersAdded = Player.Add(StaticTags.Guilder, guilders, true);
        var crownsAdded = Player.Add(StaticTags.Crown, crowns, true);
        var shillingsAdded = Player.Add(StaticTags.Shilling, shillings, true);
        totalAdded = (guildersAdded * 100) + (crownsAdded * 10) + shillingsAdded;

        Debug.Log($"The equivalent of {totalAdded} shillings was added total. Guilders: {guildersAdded}, crowns: {crownsAdded}, shillings: {shillingsAdded}");
        return true;
    }
    #endregion

    #region Exchange Methods
    //  EXCHANGE METHODS

    public static bool CurrencyExchange(Currency fromCurrency, Currency toCurrency, bool performExchange, int amountIn, out int amountOut)
    {
        var fromCurrencyData = GetCurrency(fromCurrency);
        var toCurrencyData = GetCurrency(toCurrency);

        amountOut = Mathf.CeilToInt(amountIn * (fromCurrencyData.value / toCurrencyData.value));

        if (Player.GetCount(toCurrencyData.objectID, "MoneyExchange") + amountOut <= toCurrencyData.max && Player.GetCount(fromCurrencyData.objectID, "MoneyExchange") >= amountIn)
        {
            if (performExchange)
            {
                Player.Remove(fromCurrencyData.objectID, amountIn, true);
                Player.Add(toCurrencyData.objectID, amountOut, true);
            }

            return true;
        }
        else
        {
            amountOut = 0;
            return false;
        }
    }

    public static int GetExchangePrice(Currency currencyIn, Currency currencyOut, int amountToBuy)
    {
        Debug.Log("Calculate Exchange Price was called.");
        var currencyToSellData = GetCurrency(currencyIn);
        var currencyToBuyData = GetCurrency(currencyOut);

        if (currencyIn < currencyOut)
        {
            var valueOfPurchase = amountToBuy * currencyToBuyData.value;
            var priceOfPurchase = valueOfPurchase / currencyToSellData.value;

            return Mathf.CeilToInt(priceOfPurchase);
        }
        else
        {
            var priceOfPurchase = amountToBuy * (currencyToBuyData.value / currencyToSellData.value);

            return Mathf.CeilToInt(priceOfPurchase);
        }
    }
    #endregion

    #region Commission Methods
    //  COMMISSION METHODS
    public static float CalculateCommission()
    {
        if (teller == null)
        {
            teller = GetTeller();
        }
        else
        {
            int affection = Player.GetCount(teller.objectID, "Money Manager, CalculateCommission()");
            float rate;

            if (TransientDataScript.GetWeekDay() == freeExchangeDay && affection < 90)
            {
                return 1.5f;
            }

            if (affection >= 90)
            {
                rate = 0;
            }
            else if (affection >= 70)
            {
                rate = 0.3f * (1 + (float)TransientDataScript.GetWeekDay() * 0.2f);
            }
            else if (affection >= 50)
            {
                rate = 0.7f * (1 + (float)TransientDataScript.GetWeekDay() * 0.2f);
            }
            else if (affection >= 30)
            {
                rate = 1.0f * (1 + (float)TransientDataScript.GetWeekDay() * 0.3f);
            }
            else
            {
                rate = 1.5f * (1 + (float)TransientDataScript.GetWeekDay() * 0.4f);
            }

            rate = Mathf.Ceil(rate * rate);

            return rate;
        }

        Debug.Log("Teller not found.");
        return 1.5f;
    }

    public static void SubtractCommission(int itemCost)
    {
        float commission = CalculateCommission();
        int commissionCost = (int)(itemCost * commission);
        Purchase(commissionCost);
    }
    #endregion
}
