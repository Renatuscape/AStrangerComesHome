using UnityEngine;
using UnityEngine.UIElements;

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
            playerTokenSpeed = autoMap.transientData.currentSpeed * 0.0006f;

            // RETRIEVE POSITION FROM DATAMANAGER
            var tempPosition = new Vector3(autoMap.dataManager.mapPositionX, autoMap.dataManager.mapPositionY, playerToken.transform.localPosition.z);

            // Calculate the new position without moving the player yet
            Vector3 newPosition = Vector3.MoveTowards(tempPosition, mapMarker.transform.localPosition, playerTokenSpeed);

            // Check if the new position is over a basic tile
            if (IsOverUnobstructedTile(newPosition))
            {
                // Move the player token to the new position
                playerToken.transform.localPosition = newPosition;

                // SAVE NEW POSITIONS
                autoMap.dataManager.mapPositionX = playerToken.transform.localPosition.x;
                autoMap.dataManager.mapPositionY = playerToken.transform.localPosition.y;

                if (autoMap.transientData.engineState != EngineState.Off)
                {
                    if (playerToken.transform.localPosition == mapMarker.transform.localPosition)
                    {
                        autoMap.transientData.engineState = EngineState.Off; // STOP WHEN REACHING DESTINATION

                        var location = Locations.FindByCoordinates((int)playerToken.transform.localPosition.x, (int)playerToken.transform.localPosition.y);
                        if (location is not null)
                        {
                            if (location.type is not LocationType.Crossing)
                            {
                                TransientDataScript.PushAlert("I have arrived at " + location.name);
                                TransientDataScript.transientData.currentLocation = location;
                                TransientDataScript.transientData.SpawnLocation(location);
                            }
                            else
                            {
                                if (location.type == LocationType.Crossing && location.gates.Count == 1)
                                {
                                    //add checks here
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

                                TransientDataScript.PushAlert($"I have arrived at {autoMap.transientData.currentRegion.name}.");
                            }
                        }
                        else
                        {
                            autoMap.transientData.currentLocation = null;
                            TransientDataScript.PushAlert("I have arrived at my destination.");
                        }
                    }
                    else
                    {
                        autoMap.transientData.currentLocation = null;
                    }
                }

            }
            else
            {
                TransientDataScript.PushAlert("The road ahead is obstructed. I should check my map.");
                autoMap.transientData.engineState = EngineState.Off; // STOP WHEN REACHING DESTINATION
            }
            // If not over a basic tile, you might want to handle it differently (e.g., stop movement or perform some other action)
        }
        else if (autoMap.transientData.engineState != EngineState.Off)
        {
            autoMap.transientData.engineState = EngineState.Off; // STOP WHEN REACHING DESTINATION
            TransientDataScript.PushAlert("I need to choose a destination.");
        }
    }

    bool IsOverUnobstructedTile(Vector3 position)
    {
        //// Cast a ray to check for the "UnobstructedTile" tag at the new position
        //int layerMask = LayerMask.GetMask("MapTile");
        //RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity, layerMask);  // Using a downward direction

        //// Check if the ray hit an object
        //if (hit.collider != null)
        //{
        //    Debug.Log($"Hit object: {hit.collider.gameObject.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Tag: {hit.collider.tag}");

        //    // Check if the hit object has the "UnobstructedTile" tag
        //    if (hit.collider.CompareTag("UnobstructedTile"))
        //    {
        //        return true;
        //    }
        //}

        //return false;
        return true;
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
