using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoMapPainter : MonoBehaviour
{
    public List<TileSprite> tilePalette;
    public List<Sprite> mapMarkers;

    public bool FlipTile(Region region, int xCoordinate, int yCoordinate, out TileSprite tile)
    {
        try {
            string tileID = region.mapLayout[xCoordinate].row[yCoordinate];

            tile = tilePalette.FirstOrDefault((t) => t.TileID == tileID);

            return tile is not null;
        }
        catch
        {
            Debug.LogError($"Tile at {xCoordinate}, {yCoordinate} is out of bounds.");
            tile = null; return false;
        }
    }

    public Sprite FlipMarker(Location location)
    {
        Sprite foundSprite = null;

        if (location.type == LocationType.Crossing)
        {
            foundSprite = mapMarkers.FirstOrDefault((m) => m.name == "Icon_Crossing");
        }

        if (foundSprite != null)
        {
            return foundSprite;
        }
        else
        {
            return mapMarkers.FirstOrDefault((m) => m.name == "Icon_House");
        }

    }

    [Serializable]
    public class TileSprite
    {
        public string TileID;
        public bool isUnobstructive;
        public Sprite sprite;
    }
}