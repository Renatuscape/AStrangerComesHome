using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string name;
    public string printName;
    public bool isGeneric;
    public Color nameColour;
    public string namePlate;
    public GameObject spritePrefab;

    private void Awake()
    {
        NameSetup(); //Needs to be pushed by other scripts at startup to reflect changes
    }
    public void NameSetup()
    {
        var hexColour = ColorUtility.ToHtmlStringRGB(nameColour);
        namePlate = "<color=#" + hexColour + ">" + printName + "</color>";
    }
}
