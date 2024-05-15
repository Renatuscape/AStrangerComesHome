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

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void Update()
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

        if (spawnedStation != null && spawnedStationScript != null && transientData.currentLocation == null)
        {
            if (spawnedStation.transform.position.x < -20 || spawnedStation.transform.position.x > 20)
            {
                transientData.activePrefabs.Remove(spawnedStation);

                if (spawnedStationScript != null && !spawnedStationScript.readyToDestroy)
                {
                    spawnedStationScript.RemoveStation();
                    spawnedStationScript = null;
                    spawnedStation = null;
                }
            }
        }
        else if (spawnedStation != null && spawnedStationScript != null && transientData.currentLocation != null)
        {
            if (spawnedStation.transform.position.x < -30 && !spawnedStationScript.movingRight)
            {
                spawnedStationScript.MoveRight();
                // spawnedStation.transform.position = new Vector3(25, 0, 0);
            }

            else if (spawnedStation.transform.position.x > 30 && !spawnedStationScript.movingLeft)
            {
                spawnedStationScript.MoveLeft();
                //spawnedStation.transform.position = new Vector3(-25, 0, 0);
            }
        }
    }

    void SetUpStation()
    {
        var foundStation = customStations.FirstOrDefault(s => s.name.Contains(transientData.currentLocation.objectID));

        if (foundStation == null)
        {
            foundStation = defaultStations.FirstOrDefault(s => s.name.ToLower().Contains(transientData.currentLocation.type.ToString().ToLower()));

            if (foundStation == null)
            {
                foundStation = defaultStation;
            }
        }

        spawnedStation = Instantiate(foundStation);
        spawnedStation.name = "spawnedStation";
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
        transientData.currentLocation = null;
    }
}
