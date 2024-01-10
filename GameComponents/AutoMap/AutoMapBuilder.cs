using UnityEngine;

public class AutoMapBuilder
{
    public AutoMap autoMap;

    float spacing = 1;
    GameObject mapContainer;
    AutoMapEdges edgeCreator;


    public AutoMapBuilder(AutoMap autoMap, GameObject mapContainer)
    {
        this.autoMap = autoMap;
        this.mapContainer = mapContainer;

        edgeCreator = new();
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

        edgeCreator.CreateEdge(autoMap.mapTiles, rows, columns);
    }


    void GenerateLocationMarkers(Region region)
    {
        foreach (Location location in region.locations)
        {
            Vector3 localPosition = new Vector3(location.mapX, location.mapY, 0);
            var newMarker = Object.Instantiate(autoMap.locationMarker, localPosition, Quaternion.identity);
            newMarker.transform.SetParent(mapContainer.transform);
            newMarker.transform.localPosition = localPosition;  // Set local position relative to mapContainer
            newMarker.GetComponent<MapLocationPrefab>().location = location;
            newMarker.GetComponent<MapLocationPrefab>().transientData = autoMap.transientData;
            newMarker.GetComponent<MapLocationPrefab>().autoMap = autoMap;
            autoMap.mapMarkers.Add(newMarker);
        }
    }
    public void ChangeMap(Region region)
    {
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

        GenerateMap(region.rows, region.columns);
        GenerateLocationMarkers(region);
        autoMap.playerToken.transform.localPosition = region.defaultStartingPosition;
    }
}