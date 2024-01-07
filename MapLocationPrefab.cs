using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocationPrefab : MonoBehaviour
{
    public WorldLocation location;
    private void OnMouseDown()
    {
        Debug.Log($"{location.name}");
    }
}
