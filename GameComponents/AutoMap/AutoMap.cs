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
    public MapEdgeCreator edgeCreator;

    int startingRows;
    int startingColumns;
    float scrollSpeed;
    float spacing;


    float xCutOff;
    float yCutOff;
    Transform map;

    void Start()
    {
        edgeCreator = new();

        startingRows = 35;//10;
        startingColumns = 50;// 20;

        yCutOff = startingRows / 3;
        xCutOff = startingColumns / 3;
        spacing = 1.0f;
        scrollSpeed = 0.05f;

        map = mapContainer.transform;
        playerToken.autoMap = this;
        GenerateMap(startingRows, startingColumns);
        StartCoroutine(TestRegion());
    }

    IEnumerator TestRegion()
    {
        yield return new WaitForSeconds(1);
        ChangeMap(Regions.FindByID("REGION1"));
    }
    private void Update()
    {
        //if (TransientDataScript.GameState == GameState.MapMenu)
        {
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

    void GenerateMap(int rows, int columns)
    {
        yCutOff = rows / 2;
        xCutOff = columns / 2;

        int rowStartValue = (rows / 2 * -1);
        int columnStartValue = (columns / 2 * -1);

        for (int i = rowStartValue; i <= rows / 2; i++)
        {
            for (int j = columnStartValue; j <= columns / 2; j++)
            {
                Vector3 position = new Vector3(j * spacing, i * spacing, 0);
                var newTile = Instantiate(tilePrefab, position, Quaternion.identity);
                newTile.transform.parent = mapContainer.transform;
                newTile.GetComponent<MapTilePrefab>().autoMap = this;
                newTile.GetComponent<MapTilePrefab>().sprites = tileSprites;
                mapTiles.Add(new Vector2Int(i, j), newTile);
            }
        }

        edgeCreator.CreateEdge(mapTiles, rows, columns);
    }


    void GenerateLocationMarkers(WorldRegion region)
    {
        foreach (WorldLocation location in region.locations)
        {
            Vector3 position = new Vector3(location.mapX, location.mapY, 0);
            var newMarker = Instantiate(locationMarker, position, Quaternion.identity);
            newMarker.transform.SetParent(mapContainer.transform);
            newMarker.GetComponent<MapLocationPrefab>().location = location;
            newMarker.GetComponent<MapLocationPrefab>().transientData = transientData;
            mapMarkers.Add(newMarker);
        }
    }
    public void ChangeMap(WorldRegion region, Vector3? startingPosition = null)
    {
        foreach (var t in mapTiles)
        {
            Destroy(t.Value);
        }

        foreach (var t in mapMarkers)
        {
            Destroy(t);
        }

        mapTiles = new();
        mapMarkers = new();

        GenerateMap(region.rows, region.columns);
        GenerateLocationMarkers(region);
        playerToken.transform.localPosition = startingPosition ?? region.defaultStartingPosition;
        GoToPlayerToken();
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
