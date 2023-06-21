using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTokenScript : MonoBehaviour
{
    public TransientDataScript transientData;
    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void OnTriggerEnter2D(Collider2D x)
    {
        if (x.gameObject.GetComponent<MapLocationScript>() != null)
        {
            var locationScript = x.gameObject.GetComponent<MapLocationScript>();
            transientData.currentLocation = locationScript.thisLocation;
        }
    }

    void OnTriggerExit2D(Collider2D x)
    {
        if (x.gameObject.GetComponent<MapLocationScript>() != null)
        {
            transientData.currentLocation = Location.None;
            var locationScript = x.gameObject.GetComponent<MapLocationScript>();
        }
    }
}
