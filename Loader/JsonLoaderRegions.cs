using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class JsonLoaderRegions : JsonLoader
{
    public JsonLoaderRegions()
    {
        displayName = "regions"; // This accesses the inherited field
        path += "/JsonData/Locations/"; // Add to the streaming path
    }

    public override Task StartLoading()
    {
        return LoadFromJsonAsync("Regions.json");
    }

    public async Task LoadFromJsonAsync(string fileName)
    {
        string jsonPath = path + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            DataWrapper dataWrapper = JsonUtility.FromJson<DataWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.regions != null)
                {
                    foreach (Region entry in dataWrapper.regions)
                    {
                        RegionManager.Initialise(entry);
                        Repository.instance.regions.Add(entry);
                    }

                    Report.Write($"All {displayName.ToUpper()} successfully loaded from Json file {fileName}.");
                }
                else
                {
                    Report.WriteError($"Object array for {displayName.ToUpper()} is null in JSON data. Ensure that... \n\t - The JSON data is wrapped in one object\n\t - That the parent JSON object has a name corresponding to the data wrapper\n\t - That the C# class is serializable.");
                }
            }
            else
            {
                Report.WriteError("JSON data is malformed. No wrapper found?");
                Report.Write("RAW JSON OUTPUT:\n" + jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Report.WriteError("JSON file not found: " + jsonPath);
        }
    }
}