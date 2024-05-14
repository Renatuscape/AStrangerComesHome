using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoMap : MonoBehaviour
{
    public GameObject mapContainer;
    public GameObject tilePrefab;
    public GameObject mapMarker;
    public GameObject locationMarker;
    public AutoMapCanvas mapCanvas;

    public MapPlayerToken playerToken;

    public List<GameObject> mapMarkers = new();
    public List<Sprite> baseTiles;
    public List<Sprite> edgeTiles;
    public List<Sprite> terrainTiles;
    public List<Sprite> markerTiles;

    public SerializableDictionary<Vector2Int, GameObject> mapTiles = new();

    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public AutoMapBuilder mapBuilder;
    public AutoMapTravelManager travelManager;
    public AutoMapPainter tilePainter;

    Vector3 enabledPosition = new Vector3(0, 0, 0);
    Vector3 disabledPosition = new Vector3(200, 0, 0);

    public int mapStartX;
    public int mapEndX;
    public int mapStartY;
    public int mapEndY;

    private void Start()
    {
        mapBuilder = new(this, mapContainer);
        travelManager = new(this);

        playerToken.autoMap = this;
        ToggleEnable(false);
    }

    public void ToggleEnable(bool isTrue)
    {
        if (isTrue)
        {
            transform.position = enabledPosition;
            mapCanvas.ToggleEnable(true);
            GoToPlayerToken();
            mapBuilder.CheckLockedLocations();
        }

        else
        {
            transform.position = disabledPosition;
            mapCanvas.ToggleEnable(false);
            mapContainer.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void Update()
    {
        //TRAVEL LOGIC
        if (TransientDataScript.GameState == GameState.MapMenu || TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.JournalMenu)
        {
            if (transientData.engineState != EngineState.Off && playerToken.transform.localPosition != mapMarker.transform.localPosition && travelManager is not null)
            {
                travelManager.Travel();
            }
        }

        // ENSURE DISABLE
        if (TransientDataScript.GameState != GameState.MapMenu)
        {
            if (transform.position != disabledPosition)
            {
                transform.position = disabledPosition;
            }
        }
    }

    public void TravelByGate(Gate gate)
    {
        Debug.Log("Travel by gate initiated at AutoMap");

        Region destinationRegion = Regions.FindByID(gate.destinationRegion);

        if (destinationRegion is not null)
        {
            Debug.Log($"Travel by gate calling ChangeMap for {destinationRegion.objectID}");
            ChangeMap(destinationRegion, gate.xCoordinate, gate.yCoordinate);
            transientData.currentLocation = null;
            mapMarker.transform.position = playerToken.transform.position;
        }
        else
        {
            Debug.Log($"{gate.objectID}'s region could not be found by ID {gate.destinationRegion}");
        }
    }

    public void ChangeMap(Region region, float x = 9999, float y = 9999)
    {
        Debug.Log($"Change map method received {region.objectID}.");
        transientData.currentRegion = region;
        dataManager.currentRegion = region.objectID;

        mapStartY = region.rows / 2 * -1;
        mapEndY = region.rows / 2;
        mapStartX = region.columns / 2 * -1;
        mapEndX = region.columns / 2;

        mapBuilder.ChangeMap(region);
        mapContainer.transform.localPosition = new Vector3(0, 0, 0);

        if (x == 9999 || y == 9999)
        {
            dataManager.mapPositionX = region.defaultStartingPosition.x;
            dataManager.mapPositionY = region.defaultStartingPosition.y;
        }
        else
        {
            dataManager.mapPositionX = x;
            dataManager.mapPositionY = y;
        }

        playerToken.transform.localPosition = new Vector3(dataManager.mapPositionX, dataManager.mapPositionY, 0);
        CheckCurrentLocation();
        mapCanvas.ChangeRegion();
    }

    public Location CheckCurrentLocation()
    {
        Vector2Int playerPosition = new Vector2Int((int)playerToken.transform.localPosition.x, (int)playerToken.transform.localPosition.y);

        foreach (Location location in transientData.currentRegion.locations)
        {
            if (location.mapX == playerPosition.x && location.mapY == playerPosition.y)
            {
                transientData.currentLocation = location;
                return location;
            }
        }

        return null;
    }

    public void GoToCoordinates(Vector3 coordinates)
    {
        StartCoroutine(MoveToCoordinates(coordinates));
    }

    public void GoToPlayerToken()
    {
        mapContainer.transform.position = playerToken.transform.localPosition * -1;
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

        mapContainer.transform.position = targetPosition;
    }
}
