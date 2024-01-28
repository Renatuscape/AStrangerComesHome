using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
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
    public SerializableDictionary<Vector2Int, GameObject> mapTiles = new();
    public List<GameObject> mapMarkers = new();
    public List<Sprite> baseTiles;
    public List<Sprite> edgeTiles;
    public List<Sprite> terrainTiles;
    public List<Sprite> markerTiles;

    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public AutoMapBuilder mapBuilder;
    public AutoMapScroller mapScroller;
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
        mapScroller = new(this);
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
            GoToPlayerToken();
        }

        else
        {
            transform.position = disabledPosition;
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

        //SCROLL LOGIC
        if (TransientDataScript.GameState == GameState.MapMenu)
        {
            if (transform.position != enabledPosition)
            {
                transform.position = enabledPosition;
            }
            var worldPosition = MouseTracker.GetMouseWorldPosition();

            if (worldPosition.x < LeftTarget.position.x)
            {
                mapScroller.ScrollMapRight();
            }
            else if (worldPosition.x > RightTarget.position.x)
            {
                mapScroller.ScrollMapLeft();
            }
            else if (worldPosition.y > TopTarget.position.y)
            {
                mapScroller.ScrollMapDown();
            }

            else if (worldPosition.y < BottomTarget.position.y)
            {
                mapScroller.ScrollMapUp();
            }
        }
        else //Force this bitch to hide if she hasn't
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

        if (mapScroller is not null)
        {
        mapScroller.xCutOff = mapEndX /2;
        mapScroller.yCutOff = mapEndY / 2;
        }
        else
        {
            Debug.Log("Map Scroller has not been initialised yet.");
        }


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
