using System.Collections.Generic;

public static class TextManager
{
    public static Dictionary<string, string> loadedText = new() // Load content from Json in future
    {
        {"STORY0000", ""},
        {"INTER0000", "New Game"},
        {"INTER0001", "Continue"},
    };
}

public static class Tag
{
    public static string Parse(string tag)
    {
        return TextManager.loadedText[tag];
    }
}