using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MapLocationScript : MonoBehaviour
{
    public GameObject playerToken;
    public string locationName;
    public string locationDescription; //short description for the map
    public Location thisLocation;
    public TextMeshProUGUI mapPlaceName;


    void Awake()
    {
        var locationToString = thisLocation.ToString();
        locationName = Regex.Replace(locationToString, "(\\B[A-Z])", " $1");
        mapPlaceName.text = locationName;
    }

    private void Update()
    {
        if (transform.position.x > 10f || transform.position.x < -10f || transform.position.y > 6f || transform.position.y < -6f)
            mapPlaceName.text = " ";
        else
            mapPlaceName.text = locationName;
    }
}
