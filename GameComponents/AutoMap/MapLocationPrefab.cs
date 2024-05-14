using System.Collections;
using UnityEngine;

public class MapLocationPrefab : MonoBehaviour
{
    public AutoMap autoMap;
    public Location location;

    public bool doubleCLickReady;

    private void OnMouseDown()
    {
        Debug.Log($"Tile: {(int)transform.localPosition.x}, {(int)transform.localPosition.y}");

        if (doubleCLickReady)
        {
            autoMap.PlaceMarker(transform.localPosition);
            autoMap.mapCanvas.SetLocation(location);
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
        yield return new WaitForSeconds(0.5f);
        doubleCLickReady = false;
    }

    private void OnMouseOver()
    {
        //Debug.Log("Mouse is hovering over: " + gameObject.name);

        if (!location.isHidden)
        {
            TransientDataScript.PrintFloatText($"{location.name}");
        }

        if (Input.GetMouseButtonDown(1))
        {
            autoMap.GoToCoordinates(transform.localPosition);
        }
    }

    private void OnMouseExit()
    {
        if (!location.isHidden)
        {
            TransientDataScript.DisableFloatText();
        }
    }
}
