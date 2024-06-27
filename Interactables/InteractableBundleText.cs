using static InteractableNodeText;

public class InteractableBundleText : InteractableBundle
{
    public TextType type;
    public string textTag;
    public string loadedText;
    public bool disableReuse;
    public bool lootClaimed;
    public bool disableAddBookToInventory;
    public bool Initialise()
    {
        bundleID = "WorldNodeLoot_" + textTag + "_" + (disableRespawn ? "disableRespawn" : "allowRespawn") + (exactCooldown ? "_ECD#" : "_CD#") + cooldown; ;

        if (CheckIfLootable())
        {
            lootClaimed = true;
        }

        if (lootClaimed && disableReuse)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ClaimLoot()
    {
        lootClaimed = true;
        SaveNodeToPlayer();

        foreach (var entry in customContent)
        {
            // Allow for RNG rolls at a later time?
            Player.Add(entry);
        }
    }
}