using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SalvageSpawner : MonoBehaviour
{
    TransientDataScript transientData;
    public GameObject salvageBox;
    public int fortune;

    public int salvageRNG;
    public float waitingTime;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    private void OnEnable()
    {
        Invoke("SpawnRoller", 2f);
    }

    void SyncSkills()
    {
        fortune = Player.GetCount("ATT000");
    }
    void SpawnRoller()
    {
        SyncSkills();
        var spawnDelay = 10f - (1.0f * fortune);
        var salvageChance = 60f + (fortune * 2);

        if (GameObject.Find("spawnedSalvage") == false)
        {
            salvageRNG = Random.Range(0, 100);

            if (salvageRNG < salvageChance)
            {
                SalvageSpawn();
            }
        }
        Invoke("SpawnRoller", spawnDelay);
    }

    void SalvageSpawn()
    {
        var newSalvage = Instantiate(salvageBox);
        newSalvage.transform.localPosition = new Vector3(-20f, -6f, 0);
        newSalvage.name = "spawnedSalvage";
        transientData.activePrefabs.Add(newSalvage);
    }
}
