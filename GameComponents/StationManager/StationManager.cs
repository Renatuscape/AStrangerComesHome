using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class StationManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject stationPrefab;
    public GameObject spawnedStation;
    public float parallaxMultiplier;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void Update()
    {
        if (spawnedStation == null && transientData.currentLocation != Location.None)
        {
            SetUpStation();
        }

        if (spawnedStation != null && transientData.currentLocation == Location.None)
        {
            if (spawnedStation.transform.position.x < -25 || spawnedStation.transform.position.x > 25)
            {
                transientData.activePrefabs.Remove(spawnedStation);
                Destroy(spawnedStation);
            }
        }
        else if (spawnedStation != null && transientData.currentLocation != Location.None)
        {
            if (spawnedStation.transform.position.x < -30)
                spawnedStation.transform.position = new Vector3(25, 0, 0);
            else if (spawnedStation.transform.position.x > 25)
                spawnedStation.transform.position = new Vector3(-25, 0, 0);
        }
    }

    void SetUpStation()
    {
        spawnedStation = Instantiate(stationPrefab);
        spawnedStation.name = "spawnedStation";
        transientData.activePrefabs.Add(spawnedStation);
    }
}
