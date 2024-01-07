using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTilePrefab : MonoBehaviour
{
    public AutoMap autoMap;
    public float doubleClickWindow;
    public bool doubleCLickReady;
    public List<Sprite> sprites;
    private void OnMouseDown()
    {
        Debug.Log($"Tile: {(int)transform.localPosition.x}, {(int)transform.localPosition.y}");
        autoMap.PlaceMarker(transform.localPosition);

        if (doubleCLickReady)
        {
            autoMap.GoToCoordinates(transform.localPosition);
            doubleCLickReady = false;
        }
        else
        {
            StartCoroutine(DoubleClickTimer());
        }
    }

    IEnumerator DoubleClickTimer()
    {
        doubleCLickReady = true;
        yield return new WaitForSeconds(1);
        doubleCLickReady = false;
    }
}
