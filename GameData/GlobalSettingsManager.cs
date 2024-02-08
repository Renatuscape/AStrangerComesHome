using UnityEngine;
using TMPro;

public static class GlobalSettingsManager
{
    private const string AlwaysTrueNamePlateKey = "AlwaysTrueNamePlate";
    private const string AlwaysTrueNameEverywhereKey = "AlwaysTrueNameEverywhere";
    private const string AlwaysHideCoachExteriorKey = "AlwaysHideCoachExterior";
    private const string IsScriptEnabledKey = "IsScriptEnabled";
    private const string TextSizeKey = "TextSize";
    private const string HeaderFontKey = "HeaderFont";
    private const string SubtitleFontKey = "SubtitleFont";
    private const string BodyFontKey = "BodyFont";
    private const string ScriptFontKey = "ScriptFont";

    // Call GlobalSettingsManager.SaveSettings() when you want to save the settings(e.g., when they change or when the game exits).
    // Call GlobalSettingsManager.LoadSettings() when the game starts to load the settings.

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt(AlwaysTrueNamePlateKey, GlobalSettings.AlwaysTrueNamePlate ? 1 : 0);
        PlayerPrefs.SetInt(AlwaysTrueNameEverywhereKey, GlobalSettings.AlwaysTrueNameEverywhere ? 1 : 0);
        PlayerPrefs.SetInt(AlwaysHideCoachExteriorKey, GlobalSettings.AlwaysHideCoachExterior ? 1 : 0);
        PlayerPrefs.SetInt(IsScriptEnabledKey, GlobalSettings.IsScriptEnabled ? 1 : 0);
        PlayerPrefs.SetInt(TextSizeKey, GlobalSettings.TextSize);
        PlayerPrefs.SetString(HeaderFontKey, GlobalSettings.HeaderFont != null ? GlobalSettings.HeaderFont : "");
        PlayerPrefs.SetString(SubtitleFontKey, GlobalSettings.SubtitleFont != null ? GlobalSettings.SubtitleFont : "");
        PlayerPrefs.SetString(BodyFontKey, GlobalSettings.BodyFont != null ? GlobalSettings.BodyFont : "");
        PlayerPrefs.SetString(ScriptFontKey, GlobalSettings.ScriptFont != null ? GlobalSettings.ScriptFont : "");
        PlayerPrefs.SetFloat("musicVolume", GlobalSettings.musicVolume);
        PlayerPrefs.SetFloat("ambientVolume", GlobalSettings.ambientVolume);
        PlayerPrefs.SetFloat("uiVolume", GlobalSettings.uiVolume);
        PlayerPrefs.Save();

        Debug.Log("Saved global settings");
    }

    public static void LoadSettings()
    {
        GlobalSettings.AlwaysTrueNamePlate = PlayerPrefs.GetInt(AlwaysTrueNamePlateKey, 0) == 1;
        GlobalSettings.AlwaysTrueNameEverywhere = PlayerPrefs.GetInt(AlwaysTrueNameEverywhereKey, 0) == 1;
        GlobalSettings.AlwaysHideCoachExterior = PlayerPrefs.GetInt(AlwaysHideCoachExteriorKey, 0) == 1;
        GlobalSettings.IsScriptEnabled = PlayerPrefs.GetInt(IsScriptEnabledKey, 1) == 1; // Default value is true
        GlobalSettings.TextSize = PlayerPrefs.GetInt(TextSizeKey, 0); // Default value is 0

        GlobalSettings.HeaderFont = PlayerPrefs.GetString(HeaderFontKey, "");
        GlobalSettings.SubtitleFont = PlayerPrefs.GetString(SubtitleFontKey, "");
        GlobalSettings.BodyFont = PlayerPrefs.GetString(BodyFontKey, "");
        GlobalSettings.ScriptFont = PlayerPrefs.GetString(ScriptFontKey, "");

        GlobalSettings.musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.4f);
        GlobalSettings.ambientVolume = PlayerPrefs.GetFloat("ambientVolume", 0.6f);
        GlobalSettings.uiVolume = PlayerPrefs.GetFloat("uiVolume", 0.6f);

        //Debug.Log("Loaded global settings" +
        //    $"\nTrueName {PlayerPrefs.GetInt(AlwaysTrueNamePlateKey, 0)}" +
        //    $"\nTrueNameEverywhere {PlayerPrefs.GetInt(AlwaysTrueNameEverywhereKey, 0)}" +
        //    $"\nAlwaysHideCoachExterior {PlayerPrefs.GetInt(AlwaysHideCoachExteriorKey, 0)}" +
        //    $"\nIsScriptEnabled {PlayerPrefs.GetInt(IsScriptEnabledKey, 1)}" +
        //    $"\nTextSize {PlayerPrefs.GetInt(TextSizeKey, 0)}" +
        //    $"\nHeaderFont {PlayerPrefs.GetString(HeaderFontKey, "")}" +
        //    $"\nSubtitleFont {PlayerPrefs.GetString(SubtitleFontKey, "")}" +
        //    $"\nBodyFont {PlayerPrefs.GetString(BodyFontKey, "")}" +
        //    $"\nScriptFont {PlayerPrefs.GetString(ScriptFontKey, "")}");
    }
}

public static class GlobalSettings
{
    public static bool AlwaysTrueNamePlate = false; //the traveller's nameplate always uses their True Name
    public static bool AlwaysTrueNameEverywhere = false; //characters will use the traveller's True Name in dialogue
    public static bool AlwaysHideCoachExterior = false; //The coach's exterior wall will never appear
    public static bool IsScriptEnabled = true; //whether the script font is enabled
    public static int TextSize = 0; //increase or decrease text default size by this number
    public static string HeaderFont;
    public static string SubtitleFont;
    public static string BodyFont;
    public static string ScriptFont;

    public static float musicVolume = 0.4f;
    public static float ambientVolume = 0.6f;
    public static float uiVolume = 0.6f;
}