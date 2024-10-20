using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachFoodStock : MonoBehaviour
{
    public PassengerFoodMenu satisfactionMenu;
    public InteractableBobber bobber;
    public GameObject stockObject;
    public GameObject foodDisplayContainer;
    public GameObject displayPrefab;

    public void Setup()
    {
        var count = Player.GetCount(StaticTags.OnBoardService, name);

        if (count > 0)
        {
            EnableStock();

            if (count == 2)
            {
                bobber.disable = true;
            }
            else
            {
                bobber.disable = false;
            }
        }
        else
        {
            DisableStock();
        }
    }

    public void EnableStock()
    {
        stockObject.SetActive(true);
    }
    public void DisableStock()
    {
        stockObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView != CameraView.Normal)
        {
            satisfactionMenu.Initialise();

            var count = Player.GetCount(StaticTags.OnBoardService, name);

            if (count == 1)
            {
                Player.Add(StaticTags.OnBoardService);
            }

            bobber.disable = true;
        }
    }
}
