[System.Serializable]
public class DataWrapper // For wrapping around JSON objects when loading
{
    public Item[] items;
    public Book[] books;
    public Upgrade[] upgrades;
    public Skill[] skills;
    public Recipe[] recipes;
    public Location[] locations;
    public Region[] regions;
    public Memory[] memories;
    public Quest[] quests;
    public Dialogue[] dialogues;
    public Character[] characters;
}
