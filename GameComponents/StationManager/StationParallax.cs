using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationParallax : MonoBehaviour
{
    TransientDataScript transientData;
    Vector3 spawnArea;

    public float parallaxMultiplier;
    public float spawnAdjustment = 3;
    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        spawnArea = new Vector3(-10 - (transientData.currentSpeed * spawnAdjustment), 0, 0);
        transform.position = spawnArea;
    }

    // Update is called once per frame
    void Update()
    {
        var parallaxEffect = transientData.currentSpeed * parallaxMultiplier;

        transform.position = new Vector3(transform.position.x + parallaxEffect, transform.position.y, transform.position.z);
    }
}
