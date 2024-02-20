using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class StationManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject defaultStation;
    //public GameObject stationCapitalCity;
    public GameObject spawnedStation;
    public float parallaxMultiplier;
    public List<GameObject> customStations;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void Update()
    {
        if (spawnedStation == null && transientData.currentLocation is not null && !string.IsNullOrWhiteSpace(transientData.currentLocation.objectID))
        {
            if (transientData.currentLocation.type == LocationType.Crossing && transientData.currentLocation.gates.Count == 1)
            {

            }
            else
            {
                SetUpStation();
            }
        }

        if (spawnedStation != null && transientData.currentLocation is null)
        {
            if (spawnedStation.transform.position.x < -25 || spawnedStation.transform.position.x > 25)
            {
                transientData.activePrefabs.Remove(spawnedStation);
                Destroy(spawnedStation);
            }
        }
        else if (spawnedStation != null && transientData.currentLocation is not null)
        {
            if (spawnedStation.transform.position.x < -30)
                spawnedStation.transform.position = new Vector3(25, 0, 0);
            else if (spawnedStation.transform.position.x > 25)
                spawnedStation.transform.position = new Vector3(-25, 0, 0);
        }
    }

    void SetUpStation()
    {
        var foundStation = customStations.Where(s => s.name.Contains(transientData.currentLocation.objectID)).FirstOrDefault();
        if (foundStation != null)
        {
            spawnedStation = Instantiate(foundStation);
            spawnedStation.name = "spawnedStation";
            transientData.activePrefabs.Add(spawnedStation);
        }
        else
        {
            spawnedStation = Instantiate(defaultStation);
            spawnedStation.name = "spawnedStation";
            transientData.activePrefabs.Add(spawnedStation);
        }
    }

    private void OnDisable()
    {
        if (spawnedStation is not null)
        {
            transientData.activePrefabs.Remove(spawnedStation);
            Destroy(spawnedStation);
        }
        transientData.currentLocation = null;
    }
}
