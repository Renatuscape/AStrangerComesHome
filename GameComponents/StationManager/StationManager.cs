using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject defaultStation;
    public GameObject spawnedStation;
    public StationParallax spawnedStationScript;
    public float parallaxMultiplier;
    public List<GameObject> customStations;
    public List<GameObject> defaultStations;
    public List<GameObject> defaultStationsByRegion;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void Update()
    {
        if ( TransientDataScript.IsTimeFlowing())
        {
            CheckIfReadyToSpawn();

            CheckIfReadyToRemove();

            CheckIfOutOfBounds();
        }
    }

    void CheckIfReadyToSpawn()
    {
        if (transientData.currentLocation != null && !string.IsNullOrWhiteSpace(transientData.currentLocation.objectID))
        {
            if (spawnedStation == null)
            {
                if (transientData.currentLocation.type == LocationType.Crossing && transientData.currentLocation.gates.Count == 1)
                {

                }
                else if (transientData.currentSpeed < 5)
                {
                    SetUpStation();
                }
            }
        }
    }

    void CheckIfReadyToRemove()
    {
        if (spawnedStation != null && spawnedStationScript != null && transientData.currentLocation == null)
        {
            if (spawnedStation.transform.position.x < -20 || spawnedStation.transform.position.x > 20 || TransientDataScript.transientData.engineState == EngineState.Off)
            {
                RemoveStation();
            }
        }

        if (spawnedStation != null && transientData.currentLocation != null && !spawnedStation.name.Contains(transientData.currentLocation.objectID))
        {
            RemoveStation();
        }
    }

    public void RemoveStation()
    {
        if (spawnedStationScript != null && !spawnedStationScript.readyToDestroy)
        {
            transientData.activePrefabs.Remove(spawnedStation);

            spawnedStationScript.RemoveStation();
            spawnedStationScript = null;
            spawnedStation = null;
        }
    }

    void CheckIfOutOfBounds()
    {
        if (spawnedStation != null && spawnedStationScript != null && transientData.currentLocation != null)
        {
            if (spawnedStation.transform.position.x < -30 && !spawnedStationScript.movingRight)
            {
                spawnedStationScript.MoveRight();
            }

            else if (spawnedStation.transform.position.x > 30 && !spawnedStationScript.movingLeft)
            {
                spawnedStationScript.MoveLeft();
            }
        }
    }

    void SetUpStation()
    {
        var foundStation = customStations.FirstOrDefault(s => s.name.Contains(transientData.currentLocation.objectID));

        if (foundStation == null)
        {
            var searchType = transientData.currentLocation.type.ToString().ToLower();
            var searchRegion = transientData.currentRegion.ToString().ToLower();

            // CHECK FOR LOCATION TYPE AND REGION
            if (foundStation == null && searchRegion != "region0") // REGION0 uses the defaultStations list
            {
                foundStation = defaultStationsByRegion.FirstOrDefault(s => s.name.ToLower().Contains(searchType) && s.name.ToLower().Contains(searchRegion));
            }

            // CHECK FOR LOCATION TYPE ONLY
            if (foundStation == null)
            {
                foundStation = defaultStations.FirstOrDefault(s => s.name.ToLower().Contains(searchType));
            }

            // USE FALLBACK
            if (foundStation == null)
            {
                foundStation = defaultStation;
            }
        }

        spawnedStation = Instantiate(foundStation);
        spawnedStation.name = "spawnedStation-" + TransientDataScript.GetCurrentLocation().objectID;
        transientData.activePrefabs.Add(spawnedStation);
        spawnedStationScript = spawnedStation.GetComponent<StationParallax>();
    }

    private void OnDisable()
    {
        if (spawnedStation is not null)
        {
            transientData.activePrefabs.Remove(spawnedStation);
            Destroy(spawnedStation);
        }

        spawnedStation = null;
        spawnedStationScript = null;
        transientData.currentLocation = null;
    }
}
