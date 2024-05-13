
public static class GameCodex
{
    public static BaseObject GetBaseObject(string objectID)
    {
        var identifier = objectID.Substring(0, 3);

        if (identifier == "REC")
        {
            return Recipes.FindByID(objectID);
        }
        else if (identifier == "ARC" ||
            identifier == "UNI" ||
            identifier == "GEN")
        {
            if (objectID.Contains("-Q") && objectID.Length > 6)
            {
                return Quests.FindByID(objectID);
            }
            else if (objectID.Contains("-NAME"))
            {
                return null;
            }
            else
            {
                return Characters.FindByID(objectID);
            }
        }
        else if (identifier == "CAT" ||
            identifier == "MAT" ||
            identifier == "TRA" ||
            identifier == "PLA" ||
            identifier == "SEE" ||
            identifier == "BOO" ||
            identifier == "LET" ||
            identifier == "SCR" ||
            identifier == "MIS" ||
            identifier == "TRE")
        {
            return Items.FindByID(objectID);
        }
        else if (identifier == "ALC" ||
            identifier == "GAR" ||
            identifier == "MAG" ||
            identifier == "ATT")
        {
            return Skills.FindByID(objectID);
        }
        else if (identifier == "MEC" ||
            identifier == "MAU")
        {
            return Upgrades.FindByID(objectID);
        }

        return null;
    }
}
