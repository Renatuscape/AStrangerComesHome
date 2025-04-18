using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repository : MonoBehaviour
{
    public List<Item> items = new();
    public List<Book> books = new();
    public List<Upgrade> upgrades = new();
    public List<Skill> skills = new();
    public List<Recipe> recipes = new();
    public List<Location> locations = new();
    public List<Region> regions = new();
    public List<Memory> memories = new();
    public List<Quest> quests = new();
    public List<Dialogue> dialogues = new();
    public List<Character> characters = new();

    public static Repository instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
