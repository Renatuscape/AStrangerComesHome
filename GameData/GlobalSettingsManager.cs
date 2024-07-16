using UnityEngine;
using TMPro;

public static class GlobalSettingsManager
{
    private const string AlwaysTrueNamePlateKey = "AlwaysTrueNamePlate";
    private const string AlwaysHideCoachExteriorKey = "AlwaysHideCoachExterior";
    private const string IsScriptEnabledKey = "IsScriptEnabled";
    private const string TextSizeKey = "TextSize";
    private const string HeaderFontKey = "HeaderFont";
    private const string SubtitleFontKey = "SubtitleFont";
    private const string BodyFontKey = "BodyFont";
    private const string ScriptFontKey = "ScriptFont";
    private const string MusicVolumeKey = "musicVolume";
    private const string EffectVolume = "effectVolume";
    private const string UiWalletLarge = "uiWalletLarge";
    private const string UiGearboxLarge = "uiGearboxLarge";
    private const string DarkTheme = "darkTheme";
    private const string DialogueTransparency = "dialogueTransparency";


    // Call GlobalSettingsManager.SaveSettings() when you want to save the settings(e.g., when they change or when the game exits).
    // Call GlobalSettingsManager.LoadSettings() when the game starts to load the settings.

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt(AlwaysTrueNamePlateKey, GlobalSettings.AlwaysTrueNamePlate ? 1 : 0);
        PlayerPrefs.SetInt(AlwaysHideCoachExteriorKey, GlobalSettings.AlwaysHideCoachExterior ? 1 : 0);
        PlayerPrefs.SetInt(IsScriptEnabledKey, GlobalSettings.IsScriptEnabled ? 1 : 0);
        PlayerPrefs.SetInt(TextSizeKey, GlobalSettings.TextSize);
        PlayerPrefs.SetString(HeaderFontKey, GlobalSettings.HeaderFont != null ? GlobalSettings.HeaderFont : "");
        PlayerPrefs.SetString(SubtitleFontKey, GlobalSettings.SubtitleFont != null ? GlobalSettings.SubtitleFont : "");
        PlayerPrefs.SetString(BodyFontKey, GlobalSettings.BodyFont != null ? GlobalSettings.BodyFont : "");
        PlayerPrefs.SetString(ScriptFontKey, GlobalSettings.ScriptFont != null ? GlobalSettings.ScriptFont : "");
        PlayerPrefs.SetFloat(MusicVolumeKey, GlobalSettings.musicVolume);
        PlayerPrefs.SetFloat(EffectVolume, GlobalSettings.effectVolume);
        PlayerPrefs.SetInt(UiWalletLarge, GlobalSettings.uiWalletLarge ? 1 : 0);
        PlayerPrefs.SetInt(UiGearboxLarge, GlobalSettings.uiGearboxLarge ? 1 : 0);
        PlayerPrefs.SetInt(DarkTheme, GlobalSettings.darkTheme ? 1 : 0);
        PlayerPrefs.SetFloat(DialogueTransparency, GlobalSettings.dialogueTransparency);
        PlayerPrefs.Save();

        Debug.Log("Saved global settings");
    }

    public static void LoadSettings()
    {
        GlobalSettings.AlwaysTrueNamePlate = PlayerPrefs.GetInt(AlwaysTrueNamePlateKey, 0) == 1;
        GlobalSettings.AlwaysHideCoachExterior = PlayerPrefs.GetInt(AlwaysHideCoachExteriorKey, 0) == 1;
        GlobalSettings.IsScriptEnabled = PlayerPrefs.GetInt(IsScriptEnabledKey, 1) == 1; // Default value is true
        GlobalSettings.TextSize = PlayerPrefs.GetInt(TextSizeKey, 0); // Default value is 0

        GlobalSettings.HeaderFont = PlayerPrefs.GetString(HeaderFontKey, "");
        GlobalSettings.SubtitleFont = PlayerPrefs.GetString(SubtitleFontKey, "");
        GlobalSettings.BodyFont = PlayerPrefs.GetString(BodyFontKey, "");
        GlobalSettings.ScriptFont = PlayerPrefs.GetString(ScriptFontKey, "");

        GlobalSettings.musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.4f);
        GlobalSettings.effectVolume = PlayerPrefs.GetFloat(EffectVolume, 0.6f);

        GlobalSettings.uiWalletLarge = PlayerPrefs.GetInt(UiWalletLarge, 1) == 1;
        GlobalSettings.uiGearboxLarge = PlayerPrefs.GetInt(UiGearboxLarge, 1) == 1;

        GlobalSettings.darkTheme = PlayerPrefs.GetInt(DarkTheme, 0) == 1;
        GlobalSettings.dialogueTransparency = PlayerPrefs.GetFloat(DialogueTransparency, 0.8f);
    }
}

public static class GlobalSettings
{
    public static bool AlwaysTrueNamePlate = false; //the traveller's nameplate always uses their True Name
    public static bool AlwaysHideCoachExterior = false; //The coach's exterior wall will never appear
    public static bool IsScriptEnabled = true; //whether the script font is enabled
    public static int TextSize = 0; //increase or decrease text default size by this number
    public static string HeaderFont;
    public static string SubtitleFont;
    public static string BodyFont;
    public static string ScriptFont;

    public static float musicVolume = 0.4f;
    public static float effectVolume = 0.4f;

    public static bool uiWalletLarge = true;
    public static bool uiGearboxLarge = true;

    public static bool darkTheme = false;
    public static float dialogueTransparency = 0.8f;
}