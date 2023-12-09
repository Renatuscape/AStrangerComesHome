using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    TransientDataScript transientData;
    DataManagerScript dataManager;
    public GameObject gridContainer;
    public GameObject mapMarker;
    public GameObject playerToken;
    public Grid gridObject;
    public Tilemap tileMap;

    public Vector3 gridPositionBefore;
    public Vector3 gridPositionAfter;
    bool positionSet;
    bool positionErrorInvoked;

    public float speed = 0.15f;

    void Awake()
    {
        transform.position = new Vector3(transform.position.x, -50, transform.position.z);

        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();

        gridPositionBefore = gridObject.transform.position;
        gridPositionAfter = gridObject.transform.position;

        playerToken.transform.localPosition = new Vector3(dataManager.mapPositionX, dataManager.mapPositionY, playerToken.transform.localPosition.z);
        mapMarker.transform.localPosition = new Vector3(0.5f, 0.5f, mapMarker.transform.localPosition.z);
    }

    public void NoPositionError()
    {
        positionErrorInvoked = true;

        if (playerToken.transform.localPosition == mapMarker.transform.localPosition && positionSet == false && transientData.engineState != EngineState.Off)
        {
            transientData.PushAlert("Driving in circles. Choose destination (M).");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                TransientDataScript.SetGameState(GameState.MapMenu, "Map Manager", gameObject);

                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                gridContainer.transform.localPosition = new Vector3(0, 0, gridContainer.transform.localPosition.z);
            }
            else if (TransientDataScript.GameState == GameState.MapMenu)
            {
                TransientDataScript.SetGameState(GameState.Overworld, "Map Manager", gameObject);
            }
        }

        if (TransientDataScript.GameState != GameState.MapMenu && transform.position.y != -50) //SHOULD NOT BE NECESSARY TO DO ON UPDATE. FIGURE OUT HOW TO DO IT BETTER
        {
            transform.position = new Vector3(transform.position.x, -50, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        //PLAYER TOKEN CONTROLLERS
        if (TransientDataScript.GameState == GameState.Overworld
            || TransientDataScript.GameState == GameState.ShopMenu
            || TransientDataScript.GameState == GameState.MapMenu
            || TransientDataScript.GameState == GameState.Dialogue)
        {
            //ADJUST PLAYER SPEED ON MAP
            var playerTokenSpeed = transientData.currentSpeed * 0.0006f;

            //RETRIEVE POSITION FROM DATAMANAGER
            var tempPosition = new Vector3(dataManager.mapPositionX, dataManager.mapPositionY, playerToken.transform.localPosition.z);

            //MOVE PLAYER TOKEN
            playerToken.transform.localPosition = Vector3.MoveTowards(tempPosition, mapMarker.transform.localPosition, playerTokenSpeed);

            //SAVE NEW POSITIONS
            dataManager.mapPositionX = playerToken.transform.localPosition.x;
            dataManager.mapPositionY = playerToken.transform.localPosition.y;

            if (transientData.engineState != EngineState.Off)
            {
                if (playerToken.transform.localPosition == mapMarker.transform.localPosition && positionSet == true)
                {
                    transientData.engineState = EngineState.Off; //STOP WHEN REACHING DESTINATION
                    var locationToString = transientData.currentLocation.ToString();
                    var locationName = Regex.Replace(locationToString, "(\\B[A-Z])", " $1");
                    positionSet = false;
                    if (transientData.currentLocation != Location.None)
                        transientData.PushAlert("You have arrived at " + locationName);
                    else
                        transientData.PushAlert("You have arrived at your destination.");
                }
                else if (playerToken.transform.localPosition == mapMarker.transform.localPosition && positionSet == false)
                {
                    if (positionErrorInvoked == false)
                        InvokeRepeating("NoPositionError", 0, 30f);
                }
            }
        }
    }

    public IEnumerator OnMouseDrag()
    {
        yield return new WaitForSeconds(0.075f);
        var dX = Input.GetAxis("Mouse X") * speed; var newXPos = gridContainer.transform.localPosition.x + dX;
        var dY = Input.GetAxis("Mouse Y") * speed; var newYPos = gridContainer.transform.localPosition.y + dY;
        gridContainer.transform.localPosition = new Vector3(newXPos, newYPos, gridContainer.transform.localPosition.z);

        yield return null;
    }
    public void OnMouseDown()
    {
        gridPositionBefore = gridContainer.transform.position;
    }

    public void OnMouseUp()
    {
        gridPositionAfter = gridContainer.transform.position;

        if (gridPositionAfter == gridPositionBefore)
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var tpos = tileMap.WorldToCell(worldPoint);

            // Try to get a tile from cell position
            var tile = tileMap.GetTile(tpos);

            if (tile)
            {
                Debug.Log(tpos);
                Vector3 markerTarget = tileMap.GetCellCenterWorld(tpos);
                mapMarker.transform.position = markerTarget;
                positionSet = true;
                CancelInvoke("NoPositionError");
                positionErrorInvoked = false;
            }
        }
    }
}
