using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class JsonLoaderItems : JsonLoader
{
    public JsonLoaderItems()
    {
        displayName = "items"; // This accesses the inherited field
        path += "/JsonData/Items/"; // Add to the streaming path
    }

    public override Task StartLoading()
    {
        // Multi-file loader setup

        List<Task> loadingTasks = new List<Task>();

        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();

        foreach (var file in fileInfo)
        {
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
        string jsonPath = path + fileName;

        if (File.Exists(jsonPath))
        {
            string jsonData = await Task.Run(() => File.ReadAllText(jsonPath));
            DataWrapper dataWrapper = JsonUtility.FromJson<DataWrapper>(jsonData);

            if (dataWrapper != null)
            {
                if (dataWrapper.items != null)
                {
                    foreach (Item entry in dataWrapper.items)
                    {
                        ItemManager.Initialise(entry);
                        Repository.instance.items.Add(entry);
                    }

                    Log.Write($"All {displayName.ToUpper()} successfully loaded from Json file {fileName}.");
                }
                else
                {
                    Log.WriteError($"Object array for {displayName.ToUpper()} is null in JSON data. Ensure that... \n\t - The JSON data is wrapped in one object\n\t - That the parent JSON object has a name corresponding to the data wrapper\n\t - That the C# class is serializable.");
                }
            }
            else
            {
                Log.WriteError("JSON data is malformed. No wrapper found?");
                Log.Write("RAW JSON OUTPUT:\n" +jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Log.WriteError("JSON file not found: " + jsonPath);
        }
    }
}
