using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FontManager : MonoBehaviour
{
    public static FontManager instance;
    public List<TMP_FontAsset> headerFonts;
    public List<TMP_FontAsset> subtitleFonts;
    public List<TMP_FontAsset> bodyFonts;
    public List<TMP_FontAsset> scriptFonts;

    public TMP_FontAsset defaultHeaderFont;
    public TMP_FontAsset defaultSubtitleFont;
    public TMP_FontAsset defaultBodyFont;
    public TMP_FontAsset defaultScriptFont;

    public TextMeshProUGUI header;
    public TextMeshProUGUI subtitle;
    public TextMeshProUGUI body;
    public TextMeshProUGUI script;

    public TMP_Dropdown headerFontDropDown;
    public TMP_Dropdown subtitleFontDropDown;
    public TMP_Dropdown bodyFontDropDown;
    public TMP_Dropdown scriptFontDropDown;
    public Slider fontSizeSlider;
    public int fontSize;

    private void Awake()
    {
        PopulateDropdowns();
        headerFontDropDown.onValueChanged.AddListener(OnSelectHeader);
        subtitleFontDropDown.onValueChanged.AddListener(OnSelectSubtitle);
        bodyFontDropDown.onValueChanged.AddListener(OnSelectBody);
        scriptFontDropDown.onValueChanged.AddListener(OnSelectScript);
        fontSizeSlider.onValueChanged.AddListener(OnFontSizeChange);
        instance = this;
    }

    private void OnEnable()
    {
        LoadFonts();
        //headerFontDropDown.value = headerFonts.IndexOf(header.font);
        //subtitleFontDropDown.value = subtitleFonts.IndexOf(subtitle.font);
        //bodyFontDropDown.value = bodyFonts.IndexOf(body.font);
        //scriptFontDropDown.value = scriptFonts.IndexOf(script.font);
    }

    public void PopulateDropdowns(bool setSliderValues = true)
    {
        headerFontDropDown.ClearOptions();
        subtitleFontDropDown.ClearOptions();
        bodyFontDropDown.ClearOptions();
        scriptFontDropDown.ClearOptions();

        var headerOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var font in headerFonts)
        {
            headerOptions.Add(new TMP_Dropdown.OptionData(font.name));
        }

        // Add options to dropdown
        headerFontDropDown.AddOptions(headerOptions);

        var subtitleOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var font in subtitleFonts)
        {
            subtitleOptions.Add(new TMP_Dropdown.OptionData(font.name));
        }
        subtitleFontDropDown.AddOptions(subtitleOptions);

        var bodyOptions = new List<TMP_Dropdown.OptionData>();
        foreach (var font in bodyFonts)
        {
            bodyOptions.Add(new TMP_Dropdown.OptionData(font.name));
        }
        bodyFontDropDown.AddOptions(bodyOptions);

        var scriptOptions = new List<TMP_Dropdown.OptionData>();

        if (GlobalSettings.IsScriptEnabled)
        {
            foreach (var font in scriptFonts)
            {
                scriptOptions.Add(new TMP_Dropdown.OptionData(font.name));
            }
        }
        else
        {
            foreach (var font in bodyFonts)
            {
                scriptOptions.Add(new TMP_Dropdown.OptionData(font.name));
            }
        }
        scriptFontDropDown.AddOptions(scriptOptions);

        if (setSliderValues)
        {
            headerFontDropDown.value = headerFonts.IndexOf(defaultHeaderFont);
            subtitleFontDropDown.value = subtitleFonts.IndexOf(defaultSubtitleFont);
            bodyFontDropDown.value = bodyFonts.IndexOf(defaultBodyFont);
            scriptFontDropDown.value = scriptFonts.IndexOf(defaultScriptFont);
        }
    }


    public void ToggleScript()
    {
        GlobalSettings.IsScriptEnabled = !GlobalSettings.IsScriptEnabled;
        var bodyFont = body.font;
        var headerFont = header.font;
        var subtitleFont = subtitle.font;

        script.font = GlobalSettings.IsScriptEnabled ? defaultScriptFont : body.font;

        PopulateDropdowns(false);

        headerFontDropDown.value = headerFonts.IndexOf(headerFont);
        subtitleFontDropDown.value = subtitleFonts.IndexOf(subtitleFont);
        bodyFontDropDown.value = bodyFonts.IndexOf(bodyFont);
        scriptFontDropDown.value = scriptFonts.IndexOf(defaultScriptFont);
    }

    public void ResetFonts()
    {
        header.font = defaultHeaderFont;
        subtitle.font = defaultSubtitleFont;
        body.font = defaultBodyFont;
        script.font = defaultScriptFont;
        OnFontSizeChange(0);
        fontSizeSlider.value = 0;
        GlobalSettings.IsScriptEnabled = true;
    }

    public void OnSelectHeader(int index)
    {
        // Handle font selection
        string selectedFontName = headerFontDropDown.options[index].text;

        // You can use selectedFontName to do whatever you need, like applying the font to text components
        Debug.Log("Selected font: " + selectedFontName);

        if (index < headerFonts.Count)
        {
            header.font = headerFonts[index];
        }
        else
        {
            header.font = defaultHeaderFont;
        }
    }

    public void OnSelectSubtitle(int index)
    {
        // Handle font selection
        string selectedFontName = subtitleFontDropDown.options[index].text;

        // You can use selectedFontName to do whatever you need, like applying the font to text components
        Debug.Log("Selected font: " + selectedFontName);

        if (index < subtitleFonts.Count)
        {
            subtitle.font = subtitleFonts[index];
        }
        else
        {
            subtitle.font = defaultSubtitleFont;
        }
    }

    public void OnSelectBody(int index)
    {
        // Handle font selection
        string selectedFontName = bodyFontDropDown.options[index].text;

        // You can use selectedFontName to do whatever you need, like applying the font to text components
        Debug.Log("Selected font: " + selectedFontName);

        if (index < bodyFonts.Count)
        {
            body.font = bodyFonts[index];
        }
        else
        {
            body.font = defaultBodyFont;
        }
    }

    public void OnSelectScript(int index)
    {
        // Handle font selection
        string selectedFontName = scriptFontDropDown.options[index].text;

        // You can use selectedFontName to do whatever you need, like applying the font to text components
        Debug.Log("Selected font: " + selectedFontName);

        List<TMP_FontAsset> fontList;

        if (GlobalSettings.IsScriptEnabled)
        {
            fontList = scriptFonts;
        }
        else
        {
            fontList = bodyFonts;
        }

        if (index < fontList.Count)
        {
            script.font = fontList[index];
        }
        else
        {
            script.font = defaultScriptFont;
        }
    }

    public void OnFontSizeChange(float value)
    {
        fontSize = (int)fontSizeSlider.value;
        body.fontSize = 26 + fontSize;
    }

    public void StoreFonts()
    {
        GlobalSettings.HeaderFont = header.font.name;
        GlobalSettings.SubtitleFont = subtitle.font.name;
        GlobalSettings.BodyFont = body.font.name;
        GlobalSettings.ScriptFont = script.font.name;
        GlobalSettings.TextSize = fontSize;
        GlobalSettingsManager.SaveSettings();

        gameObject.SetActive(false);
    }

    public void LoadFonts()
    {
        if (!string.IsNullOrWhiteSpace(GlobalSettings.HeaderFont))
        {
            header.font = headerFonts.FirstOrDefault((f)=> f.name == GlobalSettings.HeaderFont) ?? defaultHeaderFont;
            headerFontDropDown.value = headerFonts.IndexOf(header.font);
        }
        if (!string.IsNullOrWhiteSpace(GlobalSettings.SubtitleFont))
        {
            subtitle.font = subtitleFonts.FirstOrDefault((f) => f.name == GlobalSettings.SubtitleFont) ?? defaultSubtitleFont;
            subtitleFontDropDown.value = subtitleFonts.IndexOf(subtitle.font);
        }   
        if (!string.IsNullOrWhiteSpace(GlobalSettings.BodyFont))
        {
            body.font = bodyFonts.FirstOrDefault((f) => f.name == GlobalSettings.BodyFont) ?? defaultBodyFont;
            bodyFontDropDown.value = bodyFonts.IndexOf(body.font);
        }
        if (!string.IsNullOrWhiteSpace(GlobalSettings.ScriptFont))
        {
            if (GlobalSettings.IsScriptEnabled)
            {
                script.font = scriptFonts.FirstOrDefault((f) => f.name == GlobalSettings.ScriptFont) ?? defaultScriptFont;
                scriptFontDropDown.value = scriptFonts.IndexOf(script.font);
            }
            else
            {
                script.font = bodyFonts.FirstOrDefault((f) => f.name == GlobalSettings.ScriptFont) ?? defaultScriptFont;
                scriptFontDropDown.value = bodyFonts.IndexOf(script.font);
            }
        }
    }
}
