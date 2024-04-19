using UnityEngine;

public static class MoneyExchange
{
    public static Character teller;
    public static DayOfWeek freeExchangeDay = DayOfWeek.Solden;
    public static float hellerValue = 0.01f;
    public static int shillingValue = 1;
    public static int crownValue = 100;
    public static int guilderValue = 10000;

    public static int playerHellers;
    public static int playerShillings;
    public static int playerCrowns;
    public static int playerGuilders;

    public static int maxHellers = 9999;
    public static int maxShillings = 9999;
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
        total += playerCrowns * crownValue;
        total += playerGuilders * guilderValue;

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
                int shillingsInInventory = Player.GetCount("MIS001-COM-NN", "Money Manager, Purchase()");
                int crownsInInventory = Player.GetCount("MIS002-UNC-NN", "Money Manager, Purchase()");
                int guildersInInventory = Player.GetCount("MIS003-RAR-NN", "Money Manager, Purchase()");

                if (shillingsInInventory >= costInShillings - totalPaid)
                {
                    Player.Remove("MIS001-COM-NN", costInShillings - totalPaid, true);
                    totalPaid += costInShillings - totalPaid;
                }
                else // Exchange higher denominations
                {
                    // If not enough shillings, try to exchange higher denominations
                    if (crownsInInventory > 0)
                    {
                        var crownExchange = ExchangeCrownsToShillings(1, out int shillings);
                        Debug.Log($"Crown exchange was {crownExchange}.");
                    }
                    else if (guildersInInventory > 0)
                    {
                        Debug.Log($"No crowns in inventory. Getting guilders..");
                        var guilderExchange = ExchangeGuildersToCrowns(1, out int crowns);
                        Debug.Log($"Guilder exchange was {guilderExchange}.");

                        var crownExchange = ExchangeCrownsToShillings(1, out int shillings);
                        Debug.Log($"Crown exchange was {crownExchange}.");
                    }
                    else
                    {
                        Debug.Log($"All exchanges failed.");
                        return false; // Unable to exchange
                    }

                    shillingsInInventory = Player.GetCount("MIS001-COM-NN", "Money Manager, Purchase()");

                    if (shillingsInInventory >= costInShillings - totalPaid)
                    {
                        Player.Remove("MIS001-COM-NN", costInShillings - totalPaid, true);
                        totalPaid += costInShillings - totalPaid;
                    }
                    else
                    {
                        Player.Remove("MIS001-COM-NN", shillingsInInventory, true);
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
        evaluation = evaluation * (int)item.type + 1;

        Mathf.FloorToInt(price * evaluation);

        if (price < 1)
        {
            price = 1;
        }
        //Debug.Log($"Item({item.objectID}) base price: {item.basePrice}. Evaluation: {evaluation}. Returned price: {price}.");
        return price;
    }

    public static int AddHighestDenomination(int valueToAddInShillings) {
        int crowns = 0;
        int shillings = 0;
        int guilders = 0;

        while (valueToAddInShillings > 0)
        {
            if (valueToAddInShillings >= guilderValue)
            {
                valueToAddInShillings -= guilderValue;
                guilders++;
            }
            else if (valueToAddInShillings >= crownValue)
            {
                valueToAddInShillings -= crownValue;
                crowns++;
            }
            else
            {
                valueToAddInShillings -= 1;
                shillings++;
            }
        }

        var guildersAdded = Player.Add("MIS003-RAR-NN", guilders, true);
        var crownsAdded = Player.Add("MIS002-UNC-NN", crowns, true);
        var shillingsAdded = Player.Add("MIS001-COM-NN", shillings, true);
        var totalAdded = (guildersAdded * 10000) + (crownsAdded * 100) + shillingsAdded;
        Debug.Log($"Attempted to add {valueToAddInShillings} using highest denomination. The equivalent of {totalAdded} shillings was added total.");
        return totalAdded;
    }

    public static void AddRandomDenomination(int valueToAddInShillings)
    {
        int crown = 0;
        int shilling = 0;
        int guilder = 0;

        while (valueToAddInShillings > 0)
        {
            // Choose a random denomination to add to the inventory
            int randomDenomination = Random.Range(1, 4); // Random number between 1 and 3 inclusive

            switch (randomDenomination)
            {
                case 1: // Sovereign
                    if (valueToAddInShillings >= guilderValue)
                    {
                        valueToAddInShillings -= guilderValue;
                        guilder++;
                    }
                    break;
                case 2: // Gold crown
                    if (valueToAddInShillings >= crownValue)
                    {
                        valueToAddInShillings -= crownValue;
                        crown++;
                    }
                    break;
                case 3: // Silver crown
                    valueToAddInShillings -= shillingValue;
                    shilling++;
                    break;
            }
        }

        // Add coins to player's inventory
        Player.Add("MIS003-RAR-NN", guilder, true);
        Player.Add("MIS002-UNC-NN", crown, true);
        Player.Add("MIS001-COM-NN", shilling, true);
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

        Player.Remove("MIS002-UNC-NN", crowns, true);
        shillings = crowns * 100;
        Player.Add("MIS001-COM-NN", shillings, true);
        Debug.Log($"Exchanged {crowns} crowns for {shillings} shillings.");
        return true;
    }

    public static bool ExchangeGuildersToCrowns(int guilders, out int crowns)
    {
        if (Player.GetCount("MIS003-RAR-NN", "Money Manager, ExchangeSovereignToGold()") < guilders)
        {
            crowns = 0;
            return false;
        }

        Debug.Log($"Attempting to exchange {guilders} guilders for crowns. Current total: {GetPlayerMoney()}");

        Player.Remove("MIS003-RAR-NN", guilders, true);
        crowns = guilders * 100;
        Player.Add("MIS002-UNC-NN", crowns, true);
        Debug.Log($"Exchanged {guilders} guilders for {crowns} crowns. Current total: {GetPlayerMoney()}");
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
                Player.Remove("MIS000-JUN-NN", hellers, true);
                Player.Add("MIS001-COM-NN", shillings, true);

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
                Player.Remove("MIS001-COM-NN", shillings, true);
                Player.Add("MIS002-UNC-NN", crowns, true);

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
                Player.Remove("MIS002-UNC-NN", crowns, true);
                Player.Add("MIS003-RAR-NN", guilders, true);
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
