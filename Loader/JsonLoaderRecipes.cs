using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class JsonLoaderRecipes : JsonLoader
{
    public JsonLoaderRecipes()
    {
        displayName = "recipes"; // This accesses the inherited field
        path += "/JsonData/Recipes/"; // Add to the streaming path
    }

    public override Task StartLoading()
    {
        return LoadFromJsonAsync("Recipes.json");
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
                if (dataWrapper.recipes != null)
                {
                    foreach (Recipe entry in dataWrapper.recipes)
                    {
                        RecipeManager.Initialise(entry);
                        Repository.instance.recipes.Add(entry);
                    }

                    Report.Write(nameof(This), $"All {displayName.ToUpper()} successfully loaded from Json file {fileName}.");
                }
                else
                {
                    Report.WriteError(nameof(This), $"Object array for {displayName.ToUpper()} is null in JSON data. Ensure that... \n\t - The JSON data is wrapped in one object\n\t - That the parent JSON object has a name corresponding to the data wrapper\n\t - That the C# class is serializable.");
                }
            }
            else
            {
                Report.WriteError(nameof(This), "JSON data is malformed. No wrapper found?");
                Report.Write(nameof(This), "RAW JSON OUTPUT:\n" + jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Report.WriteError(nameof(This), "JSON file not found: " + jsonPath);
        }
    }
}
