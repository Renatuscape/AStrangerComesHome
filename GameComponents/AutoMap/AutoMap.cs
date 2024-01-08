using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class AutoMap : MonoBehaviour
{
    public Transform TopTarget;
    public Transform BottomTarget;
    public Transform LeftTarget;
    public Transform RightTarget;

    public GameObject mapContainer;
    public GameObject tilePrefab; // Reference to your tile prefab
    public GameObject mapMarker;
    public GameObject locationMarker;
    public MapPlayerToken playerToken;
    public List<Sprite> tileSprites;
    public SerializableDictionary<Vector2Int, GameObject> mapTiles = new();
    public List<GameObject> mapMarkers = new();

    public TransientDataScript transientData;
    public AutoMapEdges edgeCreator;
    public AutoMapBuilder mapBuilder;

    Vector3 enabledPosition;
    Vector3 disabledPosition;
    float scrollSpeed;
    float xCutOff;
    float yCutOff;
    Transform map;

    void Start()
    {
        edgeCreator = new();
        mapBuilder = new(this, mapContainer);
        enabledPosition = new Vector3(0,0,0);
        disabledPosition = new Vector3(200, 0, 0);
        scrollSpeed = 0.05f;

        map = mapContainer.transform;
        playerToken.autoMap = this;
        StartCoroutine(TestRegion());
    }

    IEnumerator TestRegion()
    {
        yield return new WaitForSeconds(1);
        ChangeMap(Regions.FindByID("REGION1"));
    }
    private void Update()
    {
        if (TransientDataScript.GameState == GameState.MapMenu)
        {
            if (transform.position != enabledPosition)
            {
                transform.position = enabledPosition;
            }
            var worldPosition = MouseTracker.GetMouseWorldPosition();

            if (worldPosition.x < LeftTarget.position.x)
            {
                ScrollMapRight();
            }
            else if (worldPosition.x > RightTarget.position.x)
            {
                ScrollMapLeft();
            }
            else if (worldPosition.y > TopTarget.position.y)
            {
                ScrollMapDown();
            }

            else if (worldPosition.y < BottomTarget.position.y)
            {
                ScrollMapUp();
            }
        }
        else if (TransientDataScript.GameState != GameState.MapMenu && transform.position != disabledPosition)
        {
            transform.position = disabledPosition;
        }
    }

    void ScrollMapUp()
    {
        if (map.position.y < yCutOff)
        {
            map.position = new Vector3(
                map.position.x,
                map.position.y + scrollSpeed,
                map.position.z);
        }
    }
    void ScrollMapDown()
    {
        if (map.position.y > yCutOff * -1)
        {
            map.position = new Vector3(
                map.position.x,
                map.position.y - scrollSpeed,
                map.position.z);
        }
    }
    void ScrollMapLeft()
    {
        if (map.position.x > xCutOff * -1)
        {
            map.position = new Vector3(
                map.position.x - scrollSpeed,
                map.position.y,
                map.position.z);
        }
    }
    void ScrollMapRight()
    {
        if (map.position.x < xCutOff)
        {
            map.position = new Vector3(
                map.position.x + scrollSpeed,
               map.position.y,
                map.position.z);
        }
    }

    public void ChangeMap(WorldRegion region, Vector3? startingPosition = null)
    {
        xCutOff = region.rows /2 - 2;
        yCutOff = region.columns / 2 - 6;
        mapBuilder.ChangeMap(region, startingPosition);
    }

    public void GoToCoordinates(Vector3 coordinates)
    {
        StartCoroutine(MoveToCoordinates(coordinates));
    }

    public void GoToPlayerToken()
    {
        StartCoroutine(MoveToCoordinates(playerToken.transform.localPosition));
    }

    public void PlaceMarker(Vector3 coordinates)
    {
        mapMarker.transform.localPosition = coordinates;
    }

    IEnumerator MoveToCoordinates(Vector3 targetCoordinates)
    {
        float transitionDuration = 0.5f;
        Vector3 initialPosition = mapContainer.transform.position;
        Vector3 targetPosition = -targetCoordinates; // Invert the target coordinates to get the offset

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            mapContainer.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Ensure the final position is set to the target coordinates
        mapContainer.transform.position = targetPosition;
    }
}

public class AutoMapBuilder
{
    public AutoMap autoMap;

    float spacing = 1;
    GameObject mapContainer;
    public AutoMapBuilder(AutoMap autoMap, GameObject mapContainer)
    {
        this.autoMap = autoMap;
        this.mapContainer = mapContainer;
    }

    void GenerateMap(int rows, int columns)
    {

        int rowStartValue = rows / 2 * -1;
        int columnStartValue = columns / 2 * -1;

        for (int i = rowStartValue; i <= rows / 2; i++)
        {
            for (int j = columnStartValue; j <= columns / 2; j++)
            {
                Vector3 localPosition = new Vector3(j * spacing, i * spacing, 0);
                var newTile = UnityEngine.Object.Instantiate(autoMap.tilePrefab);
                newTile.transform.parent = mapContainer.transform;
                newTile.transform.localPosition = localPosition;  // Set local position relative to mapContainer
                newTile.GetComponent<MapTilePrefab>().autoMap = autoMap;
                newTile.GetComponent<MapTilePrefab>().sprites = autoMap.tileSprites;
                autoMap.mapTiles.Add(new Vector2Int(i, j), newTile);
            }
        }

        autoMap.edgeCreator.CreateEdge(autoMap.mapTiles, rows, columns);
    }


    void GenerateLocationMarkers(WorldRegion region)
    {
        foreach (WorldLocation location in region.locations)
        {
            Vector3 localPosition = new Vector3(location.mapX, location.mapY, 0);
            var newMarker = UnityEngine.Object.Instantiate(autoMap.locationMarker, localPosition, Quaternion.identity);
            newMarker.transform.SetParent(mapContainer.transform);
            newMarker.transform.localPosition = localPosition;  // Set local position relative to mapContainer
            newMarker.GetComponent<MapLocationPrefab>().location = location;
            newMarker.GetComponent<MapLocationPrefab>().transientData = autoMap.transientData;
            newMarker.GetComponent<MapLocationPrefab>().autoMap = autoMap;
            autoMap.mapMarkers.Add(newMarker);
        }
    }
    public void ChangeMap(WorldRegion region, Vector3? startingPosition = null)
    {
        foreach (var t in autoMap.mapTiles)
        {
            UnityEngine.Object.Destroy(t.Value);
        }

        foreach (var t in autoMap.mapMarkers)
        {
            UnityEngine.Object.Destroy(t);
        }

        autoMap.mapTiles = new();
        autoMap.mapMarkers = new();

        GenerateMap(region.rows, region.columns);
        GenerateLocationMarkers(region);
        autoMap.playerToken.transform.localPosition = startingPosition ?? region.defaultStartingPosition;
        autoMap.GoToPlayerToken();
    }
}