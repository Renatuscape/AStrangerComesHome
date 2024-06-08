using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationParallax : MonoBehaviour
{
    public List<ParallaxCluster> clusters = new();
    public List<WorldParticleNode> particleNodes = new();
    TransientDataScript transientData;
    Vector3 spawnArea;

    public float parallaxMultiplier;
    public float spawnAdjustment = 3;
    public bool readyToDestroy = false;
    public int totalClusters;
    public int clustersReady;

    public bool movingLeft;
    public bool movingRight;

    float tick = StaticGameValues.parallaxFrameRate;
    float timer;
    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        spawnArea = new Vector3(-10 - (transientData.currentSpeed * spawnAdjustment), 0, 0);
        transform.position = spawnArea;
        GetAllClusters();
        totalClusters = clusters.Count;
    }

    void LateUpdate()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            timer += Time.deltaTime;
            if (timer >= tick)
            {
                timer = 0;
                Tick();
            }
        }
    }

    void Tick()
    {
        var parallaxEffect = transientData.currentSpeed * parallaxMultiplier;

        transform.position = new Vector3(transform.position.x + parallaxEffect, transform.position.y, transform.position.z);

        if (movingLeft && clustersReady == totalClusters)
        {
            transform.position = new Vector3(-28, 0, 0);
            movingLeft = false;
            clustersReady = 0;

            foreach (var cluster in clusters)
            {
                cluster.StartFadeIn(false);
            }
        }
        else if (movingRight && clustersReady == totalClusters)
        {
            transform.position = new Vector3(25, 0, 0);
            movingRight = false;
            clustersReady = 0;

            foreach (var cluster in clusters)
            {
                cluster.StartFadeIn(false);
            }
        }
        else if (readyToDestroy && clustersReady == totalClusters)
        {
            Destroy(gameObject);
        }
    }

    void GetAllClusters()
    {
        foreach (var child in transform.GetComponentsInChildren<ParallaxCluster>())
        {
            child.GetComponent<ParallaxCluster>().parentStation = this;
            clusters.Add(child);
        }
    }
    public void MoveLeft()
    {
        clustersReady = 0;
        movingLeft = true;
        movingRight = false;

        foreach (var cluster in clusters)
        {
            cluster.StartFadeOut(true);
        }
    }

    public void MoveRight()
    {
        clustersReady = 0;
        movingRight = true;
        movingLeft = false;

        foreach (var cluster in clusters)
        {
            cluster.StartFadeOut(true);
        }
    }

    public void RemoveStation()
    {
        clustersReady = 0;
        readyToDestroy = true;

        foreach (var particleNode in particleNodes)
        {
            if (particleNode != null)
            {
                particleNode.DestroySafely();
            }
        }

        foreach (var cluster in clusters)
        {
            cluster.StartFadeOut(true);
        }
    }
}
