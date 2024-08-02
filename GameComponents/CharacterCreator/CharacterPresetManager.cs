using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPresetManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public CharacterCreator characterCreator;
    public List<PlayerPreset> presets = new();
    public GameObject presetContainer;
    public Button btnExport;
    public Button btnImport;
    public bool overWritePreset = false;
    public bool isLoading = false;
    string dir;

    private void Start()
    {
        dir = Application.streamingAssetsPath + "/JsonData/PlayerPresets/";
        btnExport.onClick.AddListener(() => SavePreset());
        presetContainer.SetActive(false);
        btnImport.onClick.AddListener(() => LoadPresets());
    }

    public void AttemptToApplyPreset(PlayerPreset preset)
    {
        characterCreator.ApplyPreset(preset, true, false);
    }

    public void SavePreset()
    {
        if (string.IsNullOrEmpty(dataManager.playerName))
        {
            Debug.LogWarning("Cannot save preset without character name.");
        }
        else
        {
            var presetToSave = ExportSavedPreset();
            presetToSave.presetName = presetToSave.playerName + "_Preset";
            string fileName = presetToSave.presetName + ".json";

            if (!CheckIfFileExists(fileName) || overWritePreset)
            {
                string json = JsonUtility.ToJson(presetToSave, prettyPrint: true);
                string fullPath = dir + fileName;
                File.WriteAllText(fullPath, json);
                Debug.Log("Preset saved at " + fullPath);
                overWritePreset = false;
            }
            else
            {
                Debug.Log("Preset already exists.");
            }
        }
    }

    bool CheckIfFileExists(string fileName)
    {
        string filePath = dir + fileName;

        // check if file exists
        if (!File.Exists(filePath))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public PlayerPreset ExportSavedPreset()
    {
        PlayerPreset preset = new PlayerPreset();
        preset.PopulateFromDataManager(dataManager);

        return preset;
    }

    public async void LoadPresets()
    {
        if (!isLoading)
        {
            isLoading = true;
            ClearLoadedItems();

            await StartLoading();

            presets = presets.OrderBy(p => p.presetName).ToList();

            foreach (var preset in presets)
            {
                var presetObj = CreatePresetObject(preset);

                presetObj.transform.SetParent(presetContainer.transform, false);
            }

            presetContainer.SetActive(true);
            isLoading = false;
        }
    }

    void ClearLoadedItems()
    {
        foreach (Transform child in presetContainer.transform)
        {
            Destroy(child.gameObject);
        }

        presets.Clear();
    }

    GameObject CreatePresetObject(PlayerPreset preset)
    {
        GameObject presetObj = new();
        presetObj.AddComponent<RectTransform>();
        presetObj.AddComponent<Image>();
        presetObj.AddComponent<Button>().onClick.AddListener(() => AttemptToApplyPreset(preset));
        presetObj.name = preset.presetName;
        var script = presetObj.AddComponent<PlayerPresetPrefab>();
        script.preset = preset;
        script.presetManager = this;

        CreateTiny(preset.appearance.hairHexColour, 25, -12.5f, 12.5f);
        CreateTiny(preset.appearance.eyesHexColour, 25, -12.5f, -12.5f);
        CreateTiny(preset.appearance.cloakHexColour, 25, 12.5f, -12.5f);
        CreateTiny(preset.appearance.vestHexColour, 25, 12.5f, 12.5f);
        CreateTiny(preset.playerNameColour, 15, 0, 0);

        return presetObj;

        GameObject CreateTiny(string hex, float size, float posX, float posY)
        {
            GameObject tinySquare = new();
            tinySquare.AddComponent<RectTransform>().sizeDelta = new Vector2(size, size);
            var image = tinySquare.AddComponent<Image>();
            image.color = TransientDataScript.GetColourFromHex(hex);
            image.raycastTarget = false;

            tinySquare.transform.SetParent(presetObj.transform);
            tinySquare.transform.localPosition = new Vector3(posX, posY, 0);
            return tinySquare;
        }
    }

    public Task StartLoading()
    {
        List<Task> loadingTasks = new List<Task>();

        Debug.Log("Looking for save files in " + dir);
        var info = new DirectoryInfo(dir);
        var loadedFiles = info.GetFiles();

        foreach (var file in loadedFiles)
        {
            // Debug.Log("Found file named " + file.Name);
            if (file.Extension == ".json")
            {
                Task loadingTask = LoadFromJsonAsync(Path.GetFileName(file.FullName)); // Pass only the file name
                loadingTasks.Add(loadingTask);
            }
        }

        return Task.WhenAll(loadingTasks);
    }

    public async Task LoadFromJsonAsync(string fileName)
    {

        string jsonPath = dir + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            PlayerPreset saveInfo = JsonUtility.FromJson<PlayerPreset>(jsonData);
            presets.Add(saveInfo);
        }
    }
}

public class PlayerPresetPrefab: MonoBehaviour
{
    public CharacterPresetManager presetManager;
    public PlayerPreset preset;
}