using UnityEngine;
using TMPro;

public static class GlobalSettings
{
    // VALUES
    public static bool AlwaysTrueNamePlate = false; //the traveller's nameplate always uses their True Name
    public static bool AlwaysHideCoachExterior = false; //The coach's exterior wall will never appear
    public static bool IsScriptEnabled = true; //whether the script font is enabled
    public static int TextSize = 0; //increase or decrease text default size by this number
    public static int TextSpeed = 1;

    public static string HeaderFont;
    public static string SubtitleFont;
    public static string BodyFont;
    public static string ScriptFont;

    public static float MusicVolume = 0.4f;
    public static float EffectVolume = 0.4f;

    public static bool UiWalletLarge = true;
    public static bool UiGearboxLarge = true;

    public static bool DarkTheme = false;
    public static float DialogueTransparency = 0.8f;

    // VALUE KEYS
    private const string AlwaysTrueNamePlateKey = "AlwaysTrueNamePlate";
    private const string AlwaysHideCoachExteriorKey = "AlwaysHideCoachExterior";
    private const string IsScriptEnabledKey = "IsScriptEnabled";
    private const string TextSizeKey = "TextSize";
    private const string TextSpeedKey = "TextSpeed";

    private const string HeaderFontKey = "HeaderFont";
    private const string SubtitleFontKey = "SubtitleFont";
    private const string BodyFontKey = "BodyFont";
    private const string ScriptFontKey = "ScriptFont";

    private const string MusicVolumeKey = "MusicVolume";
    private const string EffectVolumeKey = "EffectVolume";

    private const string UiWalletLargeKey = "UiWalletLarge";
    private const string UiGearboxLargeKey = "UiGearboxLarge";

    private const string DarkThemeKey = "DarkTheme";
    private const string DialogueTransparencyKey = "DialogueTransparency";


    // Call GlobalSettings.SaveSettings() when you want to save the settings(e.g., when they change or when the game exits).
    // Call GlobalSettings.LoadSettings() when the game starts to load the settings.

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt(AlwaysTrueNamePlateKey, AlwaysTrueNamePlate ? 1 : 0);
        PlayerPrefs.SetInt(AlwaysHideCoachExteriorKey, AlwaysHideCoachExterior ? 1 : 0);
        PlayerPrefs.SetInt(IsScriptEnabledKey, IsScriptEnabled ? 1 : 0);
        PlayerPrefs.SetInt(TextSizeKey, TextSize);
        PlayerPrefs.SetInt(TextSpeedKey, TextSpeed);

        PlayerPrefs.SetString(HeaderFontKey, HeaderFont != null ? HeaderFont : "");
        PlayerPrefs.SetString(SubtitleFontKey, SubtitleFont != null ? SubtitleFont : "");
        PlayerPrefs.SetString(BodyFontKey, BodyFont != null ? BodyFont : "");
        PlayerPrefs.SetString(ScriptFontKey, ScriptFont != null ? ScriptFont : "");

        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.SetFloat(EffectVolumeKey, EffectVolume);

        PlayerPrefs.SetInt(UiWalletLargeKey, UiWalletLarge ? 1 : 0);
        PlayerPrefs.SetInt(UiGearboxLargeKey, UiGearboxLarge ? 1 : 0);

        PlayerPrefs.SetInt(DarkThemeKey, DarkTheme ? 1 : 0);
        PlayerPrefs.SetFloat(DialogueTransparencyKey, DialogueTransparency);
        PlayerPrefs.Save();

        Debug.Log("Saved global settings");
    }

    public static void LoadSettings()
    {
        AlwaysTrueNamePlate = PlayerPrefs.GetInt(AlwaysTrueNamePlateKey, 0) == 1;
        AlwaysHideCoachExterior = PlayerPrefs.GetInt(AlwaysHideCoachExteriorKey, 0) == 1;
        IsScriptEnabled = PlayerPrefs.GetInt(IsScriptEnabledKey, 1) == 1; // Default value is true
        TextSize = PlayerPrefs.GetInt(TextSizeKey, 0); // Default value is 0
        TextSpeed = PlayerPrefs.GetInt(TextSpeedKey, 1);

        HeaderFont = PlayerPrefs.GetString(HeaderFontKey, "");
        SubtitleFont = PlayerPrefs.GetString(SubtitleFontKey, "");
        BodyFont = PlayerPrefs.GetString(BodyFontKey, "");
        ScriptFont = PlayerPrefs.GetString(ScriptFontKey, "");

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.4f);
        EffectVolume = PlayerPrefs.GetFloat(EffectVolumeKey, 0.6f);

        UiWalletLarge = PlayerPrefs.GetInt(UiWalletLargeKey, 1) == 1;
        UiGearboxLarge = PlayerPrefs.GetInt(UiGearboxLargeKey, 1) == 1;

        DarkTheme = PlayerPrefs.GetInt(DarkThemeKey, 0) == 1;
        DialogueTransparency = PlayerPrefs.GetFloat(DialogueTransparencyKey, 0.8f);
    }
}