using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoMapPainter : MonoBehaviour
{
    public List<TileSprite> tilePalette;

    public bool FlipTile(Region region, int xCoordinate, int yCoordinate, out TileSprite tile)
    {
        string tileID = region.mapLayout[xCoordinate].row[yCoordinate];

        tile = tilePalette.FirstOrDefault((t)=> t.TileID == tileID);

        return tile is not null;
    }

    [Serializable]
    public class TileSprite
    {
        public string TileID;
        public bool isUnobstructive;
        public Sprite sprite;
    }
}