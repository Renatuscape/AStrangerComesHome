using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocationPrefab : MonoBehaviour
{
    TransientDataScript transientData;
    public WorldLocation location;
    private void OnMouseDown()
    {
        Debug.Log($"{location.name}");
        transientData.PrintFloatText($"{location.name}");
    }
}
