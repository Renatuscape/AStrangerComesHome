using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerPrefabScript : MonoBehaviour
{
    public GameObject passengerTarget;
    public SpriteRenderer spriteRenderer;
    public Item spiritEssence;
    public PassengerData passengerData;

    public bool isReady;

    public void Initialise(PassengerData passengerData)
    {
        this.passengerData = passengerData;

        if (!isReady)
        {
            passengerTarget = GameObject.Find("PassengerTarget");
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (passengerData.seatID == "A")
            {
                gameObject.name = "PassengerA";
                transform.position = new Vector3(passengerTarget.transform.position.x - 0.031f, passengerTarget.transform.position.y, -2);
            }
            else
            {
                gameObject.name = "PassengerB";
                transform.position = new Vector3(passengerTarget.transform.position.x, passengerTarget.transform.position.y, -2);
                transform.Rotate(new Vector3(0, -180, 0));
            }

            spiritEssence = Items.FindByID("CAT000");
        }

        UpdatePassengerData();

        isReady = true;
    }

    public void UpdatePassengerData()
    {
        if (isReady)
        {
            if (passengerData.isActive)
            {
                spriteRenderer.sprite = SpriteFactory.GetPassengerByID(passengerData.spriteID);

                CalculateFare();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Attempted to set up passenger before it was ready.");
        }
    }

    void CalculateFare()
    {
        float distance = CalculateDistance(passengerData.origin, passengerData.destination);
        Debug.Log("Distance between A and B: " + distance);
        passengerData.fare = (int)(distance * distance) * (int)Mathf.Ceil(Player.GetCount("SCR012", name) * 0.5f);
        Debug.Log("Fare calculated to " + passengerData.fare);
    }

    public static float CalculateDistance(Location pointA, Location pointB)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(pointB.mapX - pointA.mapX, 2) + Mathf.Pow(pointB.mapY - pointA.mapY, 2));
        return distance;
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            if (passengerData.destination.objectID == TransientDataScript.GetCurrentLocation().objectID)
            {
                int fortune = Player.GetCount("ATT000", "PassengerPrefabScript");

                // Actual fare is added by highest denomination
                int added = MoneyExchange.AddHighestDenomination(passengerData.fare);

                AudioManager.PlayAmbientSound("handleCoins", -0.2f);

                // Ensure the player had inventory space for full fare
                if (added < passengerData.fare)
                {
                    TransientDataScript.PushAlert("I was unable to accept the total fare. Better go to the bank!");
                }
                else
                {
                    TransientDataScript.PushAlert($"{passengerData.passengerName} paid {passengerData.fare} shillings.");
                }

                // Tip is paid out in hellers and scripted items are updated to track
                Player.Add("MIS000", Random.Range(0, passengerData.fare), true);
                Player.Add("SCR000");
                Player.Add("SCR001", passengerData.fare);

                // Spirit Essence drop
                if (Random.Range(0, 100) > 80 - (fortune * 4))
                {
                    Player.Add(spiritEssence.objectID);
                    TransientDataScript.PushAlert($"{passengerData.passengerName} dropped some Spirit Essence.");
                    AudioManager.PlayAmbientSound("cloth3");
                }

                passengerData.isActive = false;
                passengerData.characterID = string.Empty;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnMouseOver()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            TransientDataScript.PrintFloatText(passengerData.passengerName + "\n" + passengerData.destination.name);
        }
    }

    public void OnMouseExit()
    {
        TransientDataScript.DisableFloatText();
    }
}
