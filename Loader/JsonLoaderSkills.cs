using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class JsonLoaderSkills : JsonLoader
{
    public JsonLoaderSkills()
    {
        displayName = "skills"; // This accesses the inherited field
        path += "/JsonData/Skills/"; // Add to the streaming path
    }

    public override Task StartLoading()
    {
        return LoadFromJsonAsync("Skills.json");
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
                if (dataWrapper.skills != null)
                {
                    foreach (Skill entry in dataWrapper.skills)
                    {
                        SkillManager.Initialise(entry);
                        Repository.instance.skills.Add(entry);
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
                Log.Write("RAW JSON OUTPUT:\n" + jsonData); // Log the JSON data for inspection
            }
        }
        else
        {
            Log.WriteError("JSON file not found: " + jsonPath);
        }
    }
}
