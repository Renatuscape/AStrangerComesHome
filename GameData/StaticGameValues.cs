
using UnityEngine;

public static class StaticGameValues
{
    public static int maxItemValue = 99;
    public static int maxBookValue = 9;
    public static int maxCharacterValue = 200;
    public static int maxQuestValue = 110; // anything at 100 or above is a completed state
    public static int maxRecipeValue = 1;
    public static int maxAttributeValue = 10;
    public static int maxSkillValue = 10;
    public static int maxUpgradeValue = 10;
    public static int defaultGiftableLevel = 10;
    public static float parallaxFrameRate = 0.001f;

    // Theme colours
    public static Color lightText = new Color(0.8509f, 0.8666f, 0.8980f);
    public static Color darkText = new Color(0.3921f, 0.2509f, 0.1294f);
    public static Color lightThemeBackground = new Color(0.9137f, 0.8705f, 0.8078f);
    public static Color darkThemeBackground = new Color(0.1882f, 0.1686f, 0.1607f);
}
