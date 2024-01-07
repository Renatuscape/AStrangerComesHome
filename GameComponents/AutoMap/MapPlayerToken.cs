using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayerToken : MonoBehaviour
{
    public AutoMap autoMap;
    private void OnMouseDown()
    {
        Debug.Log("Registered click on Map Player Token");
        autoMap.GoToCoordinates(this.transform.localPosition);//MouseTracker.GetMouseWorldPosition());
    }
}
