using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public List<ParallaxRenderPackage> backgroundLayers = new();
    public float tick;
    public float timer;
    public bool isPlaying;
    public List<ParallaxLooseObject> looseObjects = new();
    public StationParallaxData stationToParallax;

    private void Start()
    {
        ParallaxControllerHelper.SetInstance(this);
        tick = 0.01f;
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

        if (TransientDataScript.transientData.currentSpeed != 0)
        {
            MoveLooseObjects();

            if (stationToParallax != null)
            {
                MoveStation();
            }
        }
    }

    void MoveStation()
    {
        Debug.Log("Attempting to move station.");
        if (stationToParallax.parentStation != null && stationToParallax.facade != null)
        {
            float facadeX = stationToParallax.facade.transform.position.x;

            foreach (var layer in stationToParallax.layers)
            {
                MoveStationLayer(layer, facadeX);
            }

            bool moveRight = false;
            bool moveLeft = false;

            foreach (var layer in stationToParallax.layers)
            {
                if (layer.readyToMoveRight)
                {
                    moveRight = true;
                }
                else
                {
                    break;
                }
            }

            foreach (var layer in stationToParallax.layers)
            {
                if (layer.readyToMoveLeft)
                {
                    moveLeft = true;
                }
                else
                {
                    break;
                }
            }

            if (moveRight)
            {
                foreach (var layer in stationToParallax.layers)
                {
                    layer.transform.position = new Vector3(stationToParallax.maxOffsetCentre - (layer.maxOffsetFromFacade * 1.5f) - 1, layer.transform.position.y, 0);
                }
            }
            else if (moveLeft && TransientDataScript.GetCurrentLocation() != null)
            {
                foreach (var layer in stationToParallax.layers)
                {
                    layer.transform.position = new Vector3(-stationToParallax.maxOffsetCentre + (layer.maxOffsetFromFacade * 1.5f) + 1, layer.transform.position.y, 0);
                }
            }
        }
        else
        {
            stationToParallax = null;
            Debug.Log("Attempted to parallax station but parentStation or facade was null. Setting station package to null.");
        }
    }

    void MoveStationLayer(StationParallaxDataLayer dataLayer, float facadeX)
    {
        Debug.Log("Attempting to move station layer " + dataLayer.layer.name);
        var position = dataLayer.layer.transform.position;
        var speedToMove = CalculateSpeed(dataLayer.speedMultiplier) * 0.5f;
        // ^ Though speed calculation and movement comes out as equal, halving it is what makes it LOOK correct. Find out the real issue later. Move called twice somehow?

        if (position.x + speedToMove < dataLayer.maxOffsetFromCentre)
        {
            //Debug.Log($"Station layer position {position.x} + {speedToMove} was less than {dataLayer.maxOffsetFromCentre}. Moving layer.");
            dataLayer.readyToMoveLeft = false;

            //if (CheckPositionAgainstFacade())
            {
                dataLayer.lastMoveAmount = speedToMove;
                ExecuteParallax(dataLayer.layer, speedToMove, position.y);
            }
        }
        else
        {
            // Debug.Log($"Station layer position {position.x} + {speedToMove} was not less than {dataLayer.maxOffsetFromCentre}. Setting ready to move right to true.");
            dataLayer.readyToMoveLeft = true;
        }

        if (position.x + speedToMove > -dataLayer.maxOffsetFromCentre)
        {
            // Debug.Log($"Station layer position {position.x} + {speedToMove} was more than {-dataLayer.maxOffsetFromCentre}. Moving layer.");
            dataLayer.readyToMoveRight = false;

            //if (CheckPositionAgainstFacade())
            {
                dataLayer.lastMoveAmount = speedToMove;
                ExecuteParallax(dataLayer.layer, speedToMove, position.y);
            }
        }
        else
        {
            // Debug.Log($"Station layer position {position.x} + {speedToMove} was not more than {-dataLayer.maxOffsetFromCentre}. Setting ready to move left to true.");
            dataLayer.readyToMoveRight = true;
        }

        //bool CheckPositionAgainstFacade()
        //{
        //    if (speedToMove < 0)
        //    {
        //        if (position.x - facadeX + speedToMove > -dataLayer.maxOffsetFromFacade)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (position.x + facadeX + speedToMove < dataLayer.maxOffsetFromFacade)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
    }

    void MoveLooseObjects()
    {
        List<ParallaxLooseObject> objectsToRemove = new();

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

    void MoveLooseObject(ParallaxLooseObject looseObject)
    {
        if (looseObject != null)
        {
            var speedToMove = CalculateSpeed(looseObject.layerPackage.parallaxMultiplier, looseObject.passiveSpeed);

            looseObject.lastMoveAmount = speedToMove;
            ExecuteParallax(looseObject.parallaxObject, speedToMove, looseObject.yAxis);

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
        var speedToMove = CalculateSpeed(layerData.parallaxMultiplier, layerData.passiveSpeed);

        layerData.lastMoveAmount = speedToMove;
        ExecuteParallax(parallaxObject, speedToMove);

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

    public void AddObjectToParallax(GameObject parallaxObject, string layerName, float minX = -20, float maxX = 20, float passiveSpeed = 0)
    {
        var layerData = backgroundLayers.FirstOrDefault(l => l.rend.sortingLayerName.Contains(layerName));

        if (layerData != null)
        {
            ParallaxLooseObject looseObject = new() { parallaxObject = parallaxObject, layerPackage = layerData, yAxis = parallaxObject.transform.position.y, minX = minX, maxX = maxX, passiveSpeed = passiveSpeed };
            looseObjects.Add(looseObject);
        }
        else
        {
            Debug.LogWarning("Could not find layer data. Layer name " + layerName + " could be wrong.");
        }
    }

    public void AddStationToParallax(StationParallaxData package)
    {
        stationToParallax = package;

        foreach (var layer in package.layers)
        {
            layer.speedMultiplier = ParallaxControllerHelper.GetLayerSpeed(layer.layerName);
        }
    }
}
