using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachFoodStock : MonoBehaviour
{
    public PassengerSatisfactionMenu satisfactionMenu;
    public GameObject bobber;
    public GameObject displayContainer;
    public GameObject displayPrefab;

    private void OnMouseDown()
    {
        Debug.Log("Detected click on food stock.");
        satisfactionMenu.Initialise();
    }
}
