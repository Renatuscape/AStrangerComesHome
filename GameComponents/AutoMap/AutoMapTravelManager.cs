using UnityEngine;
using UnityEngine.UIElements;

public class AutoMapTravelManager
{
    TransientDataScript transientData;
    DataManagerScript dataManager;
    AutoMap autoMap;
    GameObject playerToken;
    GameObject mapMarker;

    public AutoMapTravelManager(AutoMap autoMap)
    {
        playerToken = autoMap.playerToken.gameObject;
        mapMarker = autoMap.mapMarker;
        transientData = autoMap.transientData;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        this.autoMap = autoMap;
    }

    bool MapBoundsCheck()
    {
        Transform pos = mapMarker.transform;

        if (pos.localPosition.x > autoMap.mapStartX && pos.localPosition.x < autoMap.mapEndX &&
            pos.localPosition.y > autoMap.mapStartY && pos.localPosition.y < autoMap.mapEndY)
        {
            return true;
        }
            return false;
    }
    public void Travel()
    {
        if (MapBoundsCheck())
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
                if (playerToken.transform.localPosition == mapMarker.transform.localPosition)
                {
                    transientData.engineState = EngineState.Off; //STOP WHEN REACHING DESTINATION
                                                                 //var locationToString = transientData.currentLocation.ToString();
                                                                 //var locationName = Regex.Replace(locationToString, "(\\B[A-Z])", " $1");
                    var location = Locations.FindByCoordinates((int)playerToken.transform.localPosition.x, (int)playerToken.transform.localPosition.y);
                    if (location is not null)//(transientData.currentLocation != Location.None)
                    {
                        transientData.PushAlert("You have arrived at " + location.name);
                        transientData.currentLocation = location;
                        transientData.SpawnLocation(location);
                    }
                    else
                    {
                        transientData.currentLocation = null;
                        transientData.PushAlert("You have arrived at your destination.");
                    }
                }
                else
                {
                    transientData.currentLocation = null;
                }
            }
        }
        else if (transientData.engineState != EngineState.Off)
        {
            transientData.engineState = EngineState.Off; //STOP WHEN REACHING DESTINATION
            transientData.PushAlert("Please choose destination.");
        }
    }
}
