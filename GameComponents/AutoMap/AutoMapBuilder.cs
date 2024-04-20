using System.Collections.Generic;
using UnityEngine;

public class AutoMapBuilder
{
    public AutoMap autoMap;
    public List<MapLocationPrefab> lockedLocations = new();

    float spacing = 1;
    GameObject mapContainer;


    public AutoMapBuilder(AutoMap autoMap, GameObject mapContainer)
    {
        this.autoMap = autoMap;
        this.mapContainer = mapContainer;
    }

    void GenerateMap(Region region)
    {

        int rowCounter = region.rows - 1;

        for (int i = 0; i < region.rows; i++)
        {
            for (int j = 0; j < region.columns; j++)
            {
                Vector3 localPosition = new Vector3(j * spacing, i * spacing, 0);
                var newTile = Object.Instantiate(autoMap.tilePrefab);
                newTile.transform.parent = mapContainer.transform;
                newTile.transform.localPosition = localPosition;  // Set local position relative to mapContainer
                newTile.transform.localPosition = new Vector3(newTile.transform.localPosition.x - (region.columns / 2), newTile.transform.localPosition.y - (region.rows / 2));
                newTile.GetComponent<MapTilePrefab>().autoMap = autoMap;
                autoMap.mapTiles.Add(new Vector2Int(i, j), newTile);

                if (autoMap.tilePainter.FlipTile(region, rowCounter, j, out var tile))
                {
                    var renderer = newTile.GetComponent<SpriteRenderer>();
                    renderer.sprite = tile;
                    newTile.name = $"{tile.name} ({rowCounter}, {j})";
                }
            }
            rowCounter--;
        }
    }

    void GenerateLocationMarkers(Region region)
    {
        lockedLocations.Clear();

        foreach (Location location in region.locations)
        {
            Vector3 localPosition = new Vector3(location.mapX, location.mapY, 0);
            var newMarker = Object.Instantiate(autoMap.locationMarker, localPosition, Quaternion.identity);
            newMarker.transform.SetParent(mapContainer.transform);
            newMarker.transform.localPosition = localPosition;  // Set local position relative to mapContainer
            newMarker.name = location.name;
            autoMap.mapMarkers.Add(newMarker);


            var script = newMarker.GetComponent<MapLocationPrefab>();
            script.location = location;
            script.autoMap = autoMap;

            if (location.isHidden)
            {
                newMarker.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                lockedLocations.Add(script);
            }
            else
            {
                newMarker.GetComponent<SpriteRenderer>().sprite = autoMap.tilePainter.FlipMarker(location);
            }

            if (location.requirements.Count > 0 || location.restrictions.Count > 0)
            {
                newMarker.SetActive(false);
                lockedLocations.Add(script);
            }
        }

        CheckLockedLocations();
    }

    public void CheckLockedLocations()
    {
        var locationsToCheck = new List<MapLocationPrefab>(lockedLocations);

        foreach (var mapMarker in locationsToCheck)
        {
            if (mapMarker.location.CheckIfUnlocked())
            {
                mapMarker.gameObject.SetActive(true);
                Debug.Log($"Location {mapMarker.location.name} was unlocked after passing requirement checks.");
                lockedLocations.Remove(mapMarker);
            }
        }
    }
    public void ChangeMap(Region region)
    {
        Debug.Log($"AutoMapBuilder attempting to build {region.objectID}");
        foreach (var t in autoMap.mapTiles)
        {
            Object.Destroy(t.Value);
        }

        foreach (var t in autoMap.mapMarkers)
        {
            Object.Destroy(t);
        }

        autoMap.mapTiles = new();
        autoMap.mapMarkers = new();

        GenerateMap(region);
        GenerateLocationMarkers(region);
    }
}