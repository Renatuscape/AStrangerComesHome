using UnityEngine;

public static class MoneyExchange
{
    public static Character teller;
    public static DayOfWeek freeExchangeDay = DayOfWeek.Solden;
    public static float hellerValue = 0.01f;
    public static int shillingValue = 1;
    public static int crownValue = 100;
    public static int guilderValue = 1000;

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
        playerHellers = Player.GetCount("MIS000-JUN-NN", "Money Manager, GetPlayerMoney()"); // get copper
        playerShillings = Player.GetCount("MIS001-COM-NN", "Money Manager, GetPlayerMoney()"); // get silver
        playerCrowns = Player.GetCount("MIS002-UNC-NN", "Money Manager, GetPlayerMoney()");
        playerGuilders = Player.GetCount("MIS003-RAR-NN", "Money Manager, GetPlayerMoney()");

        total += playerShillings; // get silver
        total += (playerCrowns) * crownValue;
        total += (playerGuilders) * guilderValue;

        return total;
    }

    #region Buy/Sell Methods
    //  BUY/SELL METHODS
    public static bool Purchase(int costInSilver)
    {
        if (GetPlayerMoney() < costInSilver) //get total and update the values with current data
        {
            return false; //If this doesn't trigger, exchange to silver should be successful
        }

        else
        {
            while (playerShillings < costInSilver)
            {
                bool goldExchanged = ExchangeCrownsToShillings(1, out int silver);

                if (!goldExchanged)
                {
                    ExchangeGuildersToCrowns(1, out int gold);
                }

                GetPlayerMoney();

                if (playerShillings < costInSilver && playerCrowns == 0 && playerGuilders == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool Sell(Item item, int amount = 1, bool randomCoins = true)
    {
        if (Player.GetCount(item.objectID, "Money Manager, Sell()") > 0)
        {
            var price = CalculateSellPrice(item);

            if (randomCoins)
            {
                AddRandomDenomination(price);
            }
            else
            {
                AddHighestDenomination(price);
            }
            Player.Remove(item.objectID, amount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int CalculateSellPrice(Item item)
    {
        int price = item.basePrice;

        float evaluation = 0.1f;
        int mercantile = Player.GetCount("ATT002", "CalculateSellPrice");
        int charm = Player.GetCount("ATT001", "CalculateSellPrice");

        evaluation += (mercantile * 0.05f);
        evaluation += (charm * 0.05f);
        Debug.Log("Evaluation: " + evaluation);

        Mathf.FloorToInt(price * evaluation);

        if (price < 1)
        {
            price = 1;
        }   

        return price;
    }
    public static void AddHighestDenomination(int valueInSilver) {
        int gold = 0;
        int silver = 0;
        int sovereign = 0;

        while (valueInSilver > 0)
        {
            if (valueInSilver >= guilderValue)
            {
                valueInSilver -= guilderValue;
                sovereign++;
            }
            else if (valueInSilver >= crownValue)
            {
                valueInSilver -= crownValue;
                gold++;
            }
            else
            {
                valueInSilver -= 1;
                silver++;
            }
        }

        Player.Add("MIS003-RAR-NN", sovereign);
        Player.Add("MIS002-UNC-NN", gold);
        Player.Add("MIS001-COM-NN", silver);
    }

    public static void AddRandomDenomination(int valueInSilver)
    {
        int gold = 0;
        int silver = 0;
        int sovereign = 0;

        while (valueInSilver > 0)
        {
            // Choose a random denomination to add to the inventory
            int randomDenomination = Random.Range(1, 4); // Random number between 1 and 3 inclusive

            switch (randomDenomination)
            {
                case 1: // Sovereign
                    if (valueInSilver >= guilderValue)
                    {
                        valueInSilver -= guilderValue;
                        sovereign++;
                    }
                    break;
                case 2: // Gold crown
                    if (valueInSilver >= crownValue)
                    {
                        valueInSilver -= crownValue;
                        gold++;
                    }
                    break;
                case 3: // Silver crown
                    valueInSilver -= shillingValue;
                    silver++;
                    break;
            }
        }

        // Add coins to player's inventory
        Player.Add("MIS003-RAR-NN", sovereign);
        Player.Add("MIS002-UNC-NN", gold);
        Player.Add("MIS001-COM-NN", silver);
    }

    #endregion

    #region Exchange Methods
    //  EXCHANGE METHODS
    public static bool ExchangeCrownsToShillings(int crowns, out int shillings)
    {
        if (Player.GetCount("MIS002-UNC-NN", "Money Manager, ExchangeGoldToSilver()") < crowns)
        {
            shillings = 0;
            return false;
        }

        Player.Remove("MIS002-UNC-NN", crowns);
        shillings = crowns * crownValue;
        Player.Add("MIS001-COM-NN", shillings);
        return true;
    }

    public static bool ExchangeGuildersToCrowns(int guilders, out int crowns)
    {
        if (Player.GetCount("MIS003-RAR-NN", "Money Manager, ExchangeSovereignToGold()") < guilders)
        {
            crowns = 0;
            return false;
        }

        Player.Remove("MIS003-RAR-NN", guilders);
        crowns = guilders * guilderValue;
        Player.Add("MIS002-UNC-NN", crowns);
        return true;
    }

    public static bool ExchangeHellersToShillings(int hellers, out int shillings)
    {
        shillings = 0;

        if (Player.GetCount("MIS000-JUN-NN", "Money Manager, ExchangeCopperToSilver()") < hellers)
        {
            Debug.Log("Too few hellers in inventory.");
            return false;
        }

        if (hellers % 100 == 0)
        {
            Player.Remove("MIS000-JUN-NN", hellers);
            var valueInSilver = hellers * hellerValue;
            shillings = (int)valueInSilver;
            Player.Add("MIS001-COM-NN", shillings);

            Debug.Log($"{hellers} hellers exchanged for {shillings} shillings.");
            return true;
        }
        Debug.Log($"Hellers ({hellers}) were not divisble by {hellerValue}");
        return false;
    }

    public static bool ExchangeShillingsToCrowns(int shillings, out int crowns)
    {
        if (Player.GetCount("MIS001-COM-NN", "Money Manager, ExchangeSilverToGold()") < shillings)
        {
            crowns = 0;
            return false;
        }
        Player.Remove("MIS001-COM-NN", shillings);
        crowns = shillings * shillingValue;
        Player.Add("MIS002-UNC-NN", crowns);
        return true;
    }

    public static bool ExchangeCrownsToGuilders(int crowns, out int guilders)
    {
        if (Player.GetCount("MIS002-UNC-NN", "Money Manager, ExchangeGoldToSovereign()") < crowns)
        {
            guilders = 0;
            return false;
        }

        Player.Remove("MIS002-UNC-NN", crowns);
        guilders = crowns / guilderValue;
        Player.Add("MIS003-RAR-NN", guilders);
        return true;
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
                return 2;
            }

            if (affection >= 90)
            {
                rate = 0;
            }
            else if (affection >= 70)
            {
                rate = 0.3f;
            }
            else if (affection >= 50)
            {
                rate = 0.7f;
            }
            else if (affection >= 30)
            {
                rate = 1.0f;
            }
            else
            {
                rate = 1.5f;
            }

            rate = rate + ((float)TransientDataScript.GetWeekDay() * 0.2f); //increase rate by day of week

            return rate * 5;
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
