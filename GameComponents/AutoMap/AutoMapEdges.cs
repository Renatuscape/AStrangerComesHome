using UnityEngine;

public class AutoMapEdges
{

    // Call this function to create the edge tiles
    public void CreateEdge(SerializableDictionary<Vector2Int, GameObject> tileMap, int rows, int columns)
    {
        int rowsStartValue = rows / -2;
        int columnsStartValue = columns / -2;
        int rowsEndValue = rows / 2;
        int columnsEndValue = columns / 2;

        foreach (var kvp in tileMap)
        {
            Vector2Int coordinates = kvp.Key;

            bool isTopLeftCorner = coordinates.x == rowsStartValue && coordinates.y == columnsStartValue;
            bool isTopRightCorner = coordinates.x == rowsStartValue && coordinates.y == columnsEndValue;
            bool isBottomLeftCorner = coordinates.x == rowsEndValue && coordinates.y == columnsStartValue;
            bool isBottomRightCorner = coordinates.x == rowsEndValue && coordinates.y == columnsEndValue;

            bool isTopEdge = coordinates.x == rowsStartValue && !isTopLeftCorner && !isTopRightCorner;
            bool isBottomEdge = coordinates.x == rowsEndValue && !isBottomLeftCorner && !isBottomRightCorner;
            bool isLeftEdge = coordinates.y == columnsStartValue && !isTopLeftCorner && !isBottomLeftCorner;
            bool isRightEdge = coordinates.y == columnsEndValue && !isTopRightCorner && !isBottomRightCorner;

            // Change the sprite color based on the specific corner or edge
            if (isTopLeftCorner)
            {
                ChangeTileColor(kvp.Value, Color.red);
            }
            else if (isTopRightCorner)
            {
                ChangeTileColor(kvp.Value, Color.green);
            }
            else if (isBottomLeftCorner)
            {
                ChangeTileColor(kvp.Value, Color.blue);
            }
            else if (isBottomRightCorner)
            {
                ChangeTileColor(kvp.Value, Color.yellow);
            }
            else if (isTopEdge || isBottomEdge || isLeftEdge || isRightEdge)
            {
                ChangeTileColor(kvp.Value, Color.cyan);
            }
        }
    }

    void ChangeTileColor(GameObject tile, Color color)
    {
        // Change the sprite color or material of the tile here
        tile.GetComponent<SpriteRenderer>().color = color;
        tile.GetComponent<BoxCollider2D>().isTrigger = false;
    }
}