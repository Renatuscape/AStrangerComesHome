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

    public static int maxHellers = 9999;
    public static int maxShillings = 999;
    public static int maxCrowns = 999;
    public static int maxGuilders = 999;

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
            int totalPaid = 0;

            while (totalPaid < costInSilver)
            {
                int silverInInventory = Player.GetCount("MIS001-COM-NN", "Money Manager, Purchase()");

                if (silverInInventory == 0)
                {
                    bool goldExchanged = ExchangeCrownsToShillings(1, out int silver);

                    if (!goldExchanged)
                    {
                        ExchangeGuildersToCrowns(1, out int gold);
                    }
                }

                if (silverInInventory >= costInSilver)
                {
                    Player.Remove("MIS001-COM-NN", costInSilver);
                    return true;
                }

                else
                {
                    Player.Remove("MIS001-COM-NN", silverInInventory);
                    totalPaid += silverInInventory;
                }
            }
            if (totalPaid == costInSilver)
            {
                return true;
            }
        }
        return false;
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
    public static void AddHighestDenomination(int valueInSovereigns) {
        int crowns = 0;
        int shillings = 0;
        int guilders = 0;

        while (valueInSovereigns > 0)
        {
            if (valueInSovereigns >= guilderValue)
            {
                valueInSovereigns -= guilderValue;
                guilders++;
            }
            else if (valueInSovereigns >= crownValue)
            {
                valueInSovereigns -= crownValue;
                crowns++;
            }
            else
            {
                valueInSovereigns -= 1;
                shillings++;
            }
        }

        Player.Add("MIS003-RAR-NN", guilders);
        Player.Add("MIS002-UNC-NN", crowns);
        Player.Add("MIS001-COM-NN", shillings);
    }

    public static void AddRandomDenomination(int valueInSovereigns)
    {
        int crown = 0;
        int shilling = 0;
        int guilder = 0;

        while (valueInSovereigns > 0)
        {
            // Choose a random denomination to add to the inventory
            int randomDenomination = Random.Range(1, 4); // Random number between 1 and 3 inclusive

            switch (randomDenomination)
            {
                case 1: // Sovereign
                    if (valueInSovereigns >= guilderValue)
                    {
                        valueInSovereigns -= guilderValue;
                        guilder++;
                    }
                    break;
                case 2: // Gold crown
                    if (valueInSovereigns >= crownValue)
                    {
                        valueInSovereigns -= crownValue;
                        crown++;
                    }
                    break;
                case 3: // Silver crown
                    valueInSovereigns -= shillingValue;
                    shilling++;
                    break;
            }
        }

        // Add coins to player's inventory
        Player.Add("MIS003-RAR-NN", guilder);
        Player.Add("MIS002-UNC-NN", crown);
        Player.Add("MIS001-COM-NN", shilling);
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
        int hellersInventory = Player.GetCount("MIS000-JUN-NN", "Money Manager, ExchangeCopperToSilver()");
        int shillingsInventory = Player.GetCount("MIS001-COM-NN", "Money Manager, ExchangeSilverToGold()");
        shillings = 0;

        if (hellersInventory < hellers)
        {
            Debug.Log("Too few hellers in inventory.");
            return false;
        }

        if (hellers % 100 == 0)
        {
            var valueInSilver = hellers * hellerValue;
            shillings = (int)valueInSilver;

            if (shillingsInventory + shillings < maxShillings)
            {
                Player.Remove("MIS000-JUN-NN", hellers);
                Player.Add("MIS001-COM-NN", shillings);

                Debug.Log($"{hellers} hellers exchanged for {shillings} shillings.");
                return true;
            }
        }
        Debug.Log($"Hellers ({hellers}) were not divisble by {hellerValue}, or inventory was full.");
        return false;
    }

    public static bool ExchangeShillingsToCrowns(int shillings, out int crowns)
    {
        int shillingsInventory = Player.GetCount("MIS001-COM-NN", "Money Manager, ExchangeSilverToGold()");
        int crownsInventory = Player.GetCount("MIS002-UNC-NN", "Money Manager, ExchangeSilverToGold()");
        crowns = 0;

        if (shillingsInventory < shillings)
        {
            Debug.Log("Too few shillings in inventory.");
            return false;
        }

        if (shillings % 100 == 0)
        {
            crowns = shillings / crownValue;

            if (crowns + crownsInventory < maxCrowns)
            {
                Player.Remove("MIS001-COM-NN", shillings);
                Player.Add("MIS002-UNC-NN", crowns);

                Debug.Log($"{shillings} shillings exchanged for {crowns} crowns.");
                return true;
            }
        }

        Debug.Log($"Shillings ({shillings}) were not divisble by {crownValue}, or inventory was full.");
        return false;
    }

    public static bool ExchangeCrownsToGuilders(int crowns, out int guilders)
    {
        int crownsInventory = Player.GetCount("MIS002-UNC-NN", "Money Manager, ExchangeSilverToGold()");
        int guildersInventory = Player.GetCount("MIS003-RAR-NN", "Money Manager, ExchangeSilverToGold()");
        guilders = 0;

        if (crownsInventory < crowns)
        {
            Debug.Log("Too few crowns in inventory.");
            return false;
        }

        if (crowns % 100 == 0)
        {
            guilders = crowns * 10 / guilderValue;

            if (guildersInventory + guilders < maxGuilders)
            {
                Player.Remove("MIS002-UNC-NN", crowns);
                Player.Add("MIS003-RAR-NN", guilders);
                return true;
            }
        }
        Debug.Log($"Crowns ({crowns}) were not divisble by {guilderValue}, or inventory was full.");
        return false;
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
