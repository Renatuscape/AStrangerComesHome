using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPresetMenu : MonoBehaviour
{
    public CharacterPresetManager presetManager;
    public CharacterCreator creator;
    public GameObject menuCanvas;
    public PlayerPresetPrefab presetPrefab;
    public TextMeshProUGUI presetNameTMP;
    public Button btnApplyAll;
    public Button btnApplyAppearance;
    public Button btnApplyColour;
    public Button btnDelete;
    bool ready = false;

    private void Start()
    {
        if (presetPrefab == null)
        {
            menuCanvas.SetActive(false);
        }
    }

    void CreateNewInstanceAndApplyPreset(PlayerPreset preset, bool applyPersonalia, bool colourOnly)
    {
        PlayerPreset newPreset = new();

        newPreset.presetName = preset.presetName;
        newPreset.presetFilePath = preset.presetFilePath;
        newPreset.playerName = preset.playerName;
        newPreset.playerGender = preset.playerGender;
        newPreset.pronounSub = preset.pronounSub;
        newPreset.pronounObj = preset.pronounObj;
        newPreset.pronounGen = preset.pronounGen;

        newPreset.playerNameColour = preset.playerNameColour;
        newPreset.headIndex = preset.headIndex;
        newPreset.appearance = preset.appearance.Clone();

        creator.ApplyPreset(newPreset, applyPersonalia, colourOnly);
        menuCanvas.SetActive(false);
    }

    void SetUpButtons()
    {
        btnApplyAll.onClick.RemoveAllListeners();
        btnApplyAppearance.onClick.RemoveAllListeners();
        btnApplyColour.onClick.RemoveAllListeners();
        btnDelete.onClick.RemoveAllListeners();

        btnApplyAll.onClick.AddListener(() => CreateNewInstanceAndApplyPreset(presetPrefab.preset, true, false));
        btnApplyAppearance.onClick.AddListener(() => CreateNewInstanceAndApplyPreset(presetPrefab.preset, false, false));
        btnApplyColour.onClick.AddListener(() => CreateNewInstanceAndApplyPreset(presetPrefab.preset, false, true));
        btnDelete.onClick.AddListener(() => { presetManager.DeleteFileAndRefresh(presetPrefab.preset.presetFilePath); menuCanvas.SetActive(false); });
        
        ready = true;
    }
    public void Open(PlayerPresetPrefab prefab)
    {
        presetPrefab = prefab;

        presetNameTMP.text = "<color=#" + presetPrefab.preset.playerNameColour + ">" + presetPrefab.preset.playerName + "</color>";
        menuCanvas.SetActive(true);

        if (!ready)
        {
            SetUpButtons();
        }
    }
}