﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LooseParallaxObject
{
    public GameObject parallaxObject;
    public ParallaxRenderPackage layerPackage;
    public float yAxis;
    public float minX;
    public float maxX;
    public float passiveSpeed;
}

public class ParallaxController : MonoBehaviour
{
    public static ParallaxController instance;
    public List<ParallaxRenderPackage> backgroundLayers = new();
    public float tick = 0.1f;
    public float timer;
    public bool isPlaying;
    public List<LooseParallaxObject> looseObjects = new();

    private void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if (isPlaying)
        {
            if (TransientDataScript.IsTimeFlowing())
            {
                timer += Time.deltaTime;

                if (timer >= tick)
                {
                    TimerTick();
                    timer = 0;
                }
            }
        }
    }

    void TimerTick()
    {
        foreach (var layer in backgroundLayers)
        {
            if (layer.rend.sprite != null && TransientDataScript.transientData.currentSpeed != 0 || layer.passiveSpeed > 0)
            {
                MoveBackground(layer);
            }
        }

        List<LooseParallaxObject> objectsToRemove = new();

        foreach (var looseObject in looseObjects)
        {
            if (looseObject != null && looseObject.parallaxObject != null)
            {
                MoveLooseObject(looseObject);
            }
            else
            {
                objectsToRemove.Add(looseObject);
            }
        }

        foreach (var looseObject in objectsToRemove)
        {
            looseObjects.Remove(looseObject);
        }
    }

    public static void ParallaxThis(GameObject parallaxObject, string layerName, float minX = -20, float maxX = 20, float passiveSpeed = 0)
    {
        if (instance != null && parallaxObject != null)
        {
            instance.AddParallaxObjectToLayer(parallaxObject, layerName, minX, maxX, passiveSpeed);
        }
    }

    public static void ParallaxThisStation(StationParallaxData package)
    {

    }

    public void AddParallaxObjectToLayer(GameObject parallaxObject, string layerName, float minX = -20, float maxX = 20, float passiveSpeed = 0)
    {
        var layerData = backgroundLayers.FirstOrDefault(l => l.rend.sortingLayerName.Contains(layerName));

        if (layerData != null)
        {
            LooseParallaxObject looseObject = new() { parallaxObject = parallaxObject, layerPackage = layerData, yAxis = parallaxObject.transform.position.y, minX = minX, maxX = maxX, passiveSpeed = passiveSpeed };
            looseObjects.Add(looseObject);
        }
        else
        {
            Debug.LogWarning("Could not find layer data. Layer name " + layerName + " could be wrong.");
        }
    }

    void MoveLooseObject(LooseParallaxObject looseObject)
    {
        if (looseObject != null)
        {
            ExecuteParallax(looseObject.parallaxObject, CalculateSpeed(looseObject.layerPackage.parallaxMultiplier, looseObject.passiveSpeed), looseObject.yAxis);

            if (looseObject.parallaxObject.transform.position.x <= looseObject.minX)
            {
                looseObject.parallaxObject.transform.position = new Vector2(looseObject.maxX - 1, transform.position.y);
            }
            else if (looseObject.parallaxObject.transform.position.x >= looseObject.maxX)
            {
                looseObject.parallaxObject.transform.position = new Vector2(looseObject.minX + 1, transform.position.y);
            }
        }
        else
        {
            looseObjects.Remove(looseObject);
            Debug.Log("Loose object was null. Removing from list.");
        }
    }

    void MoveBackground(ParallaxRenderPackage layerData)
    {
        var parallaxObject = layerData.rend.gameObject;

        ExecuteParallax(parallaxObject, CalculateSpeed(layerData.parallaxMultiplier, layerData.passiveSpeed));

        if (layerData.spriteSize.x == 0)
        {
            layerData.spriteSize = layerData.rend.sprite.bounds.size;
        }

        if (layerData.rend.gameObject.transform.position.x <= 0 - (layerData.spriteSize.x / 3) || layerData.rend.gameObject.transform.position.x >= (layerData.spriteSize.x / 3))
        {
            // Debug.Log($"Attempting to set layer back to zero. Checked current position {layerData.rend.gameObject.transform.position.x} against size {layerData.spriteSize.x}");
            layerData.rend.gameObject.transform.position = new Vector2(0, transform.position.y);
        }
    }

    float CalculateSpeed(float multiplier, float passiveSpeed = 0)
    {
        var parallaxEffect = TransientDataScript.transientData.currentSpeed * multiplier;
        return passiveSpeed + parallaxEffect;
    }
    void ExecuteParallax(GameObject target, float parallaxAmount, float yAxis = 0)
    {
        var currentPosition = target.transform.position;
        target.transform.position = new Vector3(currentPosition.x + parallaxAmount, yAxis, 0);
    }

    public void UpdateSpriteSizes()
    {
        foreach (var layer in backgroundLayers)
        {
            if (layer.rend.sprite != null)
            {
                layer.spriteSize = layer.rend.sprite.bounds.size;
            }
        }
    }
}
