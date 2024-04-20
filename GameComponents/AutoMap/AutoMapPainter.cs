using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AutoMapPainter : MonoBehaviour
{
    public Sprite defaultSprite;
    public List<Sprite> tileSprites;
    public List<Sprite> mapMarkers;

    public bool FlipTile(Region region, int xCoordinate, int yCoordinate, out Sprite tile)
    {
        try
        {
            string tileID = region.mapLayout[xCoordinate].row[yCoordinate];
            tile = tileSprites.FirstOrDefault((t) => t.name.Contains(tileID));

            return tile != null;
        }
        catch
        {
            Debug.LogError($"Tile at {xCoordinate}, {yCoordinate} is out of bounds.");
            tile = null;
            return false;
        }
    }

    public Sprite FlipMarker(Location location)
    {
        Sprite foundSprite = null;

        if (!string.IsNullOrEmpty(location.customSpriteID))
        {
            foundSprite = mapMarkers.FirstOrDefault(m => m.name.Contains(location.customSpriteID));
        }

        if (foundSprite == null)
        {
            foundSprite = mapMarkers.FirstOrDefault((m) => m.name.Contains(location.type.ToString()));
        }

        if (foundSprite != null)
        {
            return foundSprite;
        }
        else
        {
            return defaultSprite;
        }
    }
}