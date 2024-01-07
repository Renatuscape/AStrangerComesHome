using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMap : MonoBehaviour
{
    public Transform TopTarget;
    public Transform BottomTarget;
    public Transform LeftTarget;
    public Transform RightTarget;

    public GameObject mapContainer;
    public GameObject tilePrefab; // Reference to your tile prefab
    public int rows;
    public int columns;
    public float scrollSpeed;
    public float spacing;

    void Start()
    {
        rows = 10;
        columns = 20;
        spacing = 1.0f;
        scrollSpeed = 0.01f;
        GenerateMap();
    }

    private void Update()
    {
        //if (TransientDataScript.GameState == GameState.MapMenu)
        {
            var worldPosition = MouseTracker.GetMouseWorldPosition();

            if (worldPosition.x < LeftTarget.position.x)
            {
                ScrollMapRight();
            }
            else if (worldPosition.x > RightTarget.position.x)
            {
                ScrollMapLeft();
            }
            else if (worldPosition.y > TopTarget.position.y)
            {
                ScrollMapDown();
            }

            else if (worldPosition.y < BottomTarget.position.y)
            {
                ScrollMapUp();
            }
        }
    }

    void ScrollMapUp()
    {
        mapContainer.transform.position = new Vector3(mapContainer.transform.position.x, mapContainer.transform.position.y + scrollSpeed, mapContainer.transform.position.z);
    }
    void ScrollMapDown()
    {
        mapContainer.transform.position = new Vector3(mapContainer.transform.position.x, mapContainer.transform.position.y - scrollSpeed, mapContainer.transform.position.z);
    }
    void ScrollMapLeft()
    {
        mapContainer.transform.position = new Vector3(mapContainer.transform.position.x - scrollSpeed, mapContainer.transform.position.y, mapContainer.transform.position.z);
    }
    void ScrollMapRight()
    {
        mapContainer.transform.position = new Vector3(mapContainer.transform.position.x + scrollSpeed, mapContainer.transform.position.y, mapContainer.transform.position.z);
    }

    void GenerateMap()
    {
        int rowStartValue = (rows / 2 * -1);
        int columnStartValue = (columns / 2 * -1);

        for (int i = rowStartValue; i < rows / 2; i++)
        {
            for (int j = columnStartValue; j < columns / 2; j++)
            {
                Debug.Log($"Instantiating tile (r{j} c{i})");
                Vector3 position = new Vector3(j * spacing, i * spacing, 0);
                var newTile = Instantiate(tilePrefab, position, Quaternion.identity);
                newTile.transform.parent = mapContainer.transform;
            }
        }
    }
}
