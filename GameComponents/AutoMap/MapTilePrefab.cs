using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class MapTilePrefab : MonoBehaviour
{
    public AutoMap autoMap;
    public float doubleClickWindow;
    public bool doubleClickReady;
    public Vector3 mouseOffset;

    void OnMouseDrag()
    {
        autoMap.mapContainer.transform.position = MouseTracker.GetMouseWorldPosition() + mouseOffset;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            autoMap.GoToCoordinates(transform.localPosition);
        }
    }
    private void OnMouseDown()
    {
        mouseOffset = autoMap.mapContainer.transform.position - MouseTracker.GetMouseWorldPosition();

        if (doubleClickReady)
        {
            // Debug.Log($"Tile: {(int)transform.localPosition.x}, {(int)transform.localPosition.y}");
            autoMap.PlaceMarker(transform.localPosition);
            autoMap.mapCanvas.SetLocation(null);
            doubleClickReady = false;
        }
        else
        {
            StartCoroutine(DoubleClickTimer());
        }
    }

    IEnumerator DoubleClickTimer()
    {
        doubleClickReady = true;
        yield return new WaitForSeconds(0.35f);
        doubleClickReady = false;
    }
}
