using UnityEngine;

public class AutoMapTravelManager
{
    AutoMap autoMap;
    GameObject playerToken;
    GameObject mapMarker;
    float playerTokenSpeed;

    public AutoMapTravelManager(AutoMap autoMap)
    {
        playerToken = autoMap.playerToken.gameObject;
        mapMarker = autoMap.mapMarker;
        this.autoMap = autoMap;
    }

    public void Travel()
    {
        if (MapBoundsCheck())
        {
            // ADJUST PLAYER SPEED ON MAP
            playerTokenSpeed = autoMap.transientData.currentSpeed * 0.0015f;

            // RETRIEVE POSITION FROM DATAMANAGER
            var tempPosition = new Vector3(autoMap.dataManager.mapPositionX, autoMap.dataManager.mapPositionY, playerToken.transform.localPosition.z);

            Vector3 newPosition = Vector3.MoveTowards(tempPosition, mapMarker.transform.localPosition, playerTokenSpeed);

            playerToken.transform.localPosition = newPosition;

            // SAVE NEW POSITIONS
            autoMap.dataManager.mapPositionX = playerToken.transform.localPosition.x;
            autoMap.dataManager.mapPositionY = playerToken.transform.localPosition.y;

            if (autoMap.transientData.engineState != EngineState.Off)
            {
                if (playerToken.transform.localPosition == mapMarker.transform.localPosition)
                {
                    autoMap.transientData.engineState = EngineState.Off;

                    var location = Locations.FindByCoordinates(TransientDataScript.gameManager.dataManager.currentRegion, (int)playerToken.transform.localPosition.x, (int)playerToken.transform.localPosition.y, true);
                    
                    if (location != null)
                    {
                        if (location.type != LocationType.Crossing)
                        {
                            TransientDataScript.PushAlert(location.name);
                            TransientDataScript.transientData.currentLocation = location;
                            TransientDataScript.transientData.OnLocationSpawn(location);
                        }
                        else
                        {
                            if (location.type == LocationType.Crossing && location.gates.Count == 1)
                            {
                                Debug.Log("Attempting to travel by gate");

                                Gate gate = location.gates[0];

                                if (gate.AttemptChecks(out var x, out var y))
                                {
                                    TransientDataScript.TravelByGate(location.gates[0]);
                                }
                                else
                                {
                                    TransientDataScript.PushAlert($"{gate.failText}");
                                    autoMap.transientData.currentLocation = null;
                                }
                            }

                            TransientDataScript.PushAlert($"{autoMap.transientData.currentRegion.name} Region.");
                        }
                    }
                    else
                    {
                        autoMap.transientData.currentLocation = null;
                        LogAlert.QueueTextAlert("I have arrived at my destination.");
                    }
                }
                else
                {
                    autoMap.transientData.currentLocation = null;
                }
            }
        }
        else if (autoMap.transientData.engineState != EngineState.Off)
        {
            autoMap.transientData.engineState = EngineState.Off;
            LogAlert.QueueTextAlert("I need to choose a destination.");
        }
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
}
