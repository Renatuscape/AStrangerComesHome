using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PassengerPrefabScript : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public GameObject passengerTarget;
    public SpriteRenderer spriteRenderer;
    public Item spiritEssence;
    public bool isPassengerA;
    public int fare;

    public string passengerName;
    public Location origin;
    public Location destination;

    public List<string> dialogueList;

    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        passengerTarget = GameObject.Find("PassengerTarget");
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (isPassengerA)
        {
            name = "PassengerA";
            transform.position = new Vector3(passengerTarget.transform.position.x - 0.031f, passengerTarget.transform.position.y, -2);
        }
        else
        {
            name = "PassengerB";
            transform.position = new Vector3(passengerTarget.transform.position.x, passengerTarget.transform.position.y, -2);
            transform.Rotate(new Vector3(0, -180, 0));
        }
    }
    private void OnEnable()
    {
        spiritEssence = Items.FindByID("CAT000");
        if (isPassengerA)
        {
            passengerName = dataManager.passengerNameA;
            origin = dataManager.passengerOriginA;
            destination = dataManager.passengerDestinationA;
            dialogueList = dataManager.passengerChatListA;
        }
        if (!isPassengerA)
        {
            passengerName = dataManager.passengerNameB;
            origin = dataManager.passengerOriginB;
            destination = dataManager.passengerDestinationB;
            dialogueList = dataManager.passengerChatListB;
        }

        var passengerSpriteList = GameObject.Find("PassengerSpawner").GetComponent<PassengerManager>().passengerSpriteList;
        spriteRenderer.sprite = passengerSpriteList[Random.Range(0, passengerSpriteList.Count - 1)];

        CalculateFare();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPassengerA && dataManager.passengerIsActiveA == false)
            gameObject.SetActive(false);
        if (!isPassengerA && dataManager.passengerIsActiveB == false)
            gameObject.SetActive(false);

        if (origin is null)
        {
            origin = transientData.currentRegion.locations[0]; //Check for closest place
            CalculateFare();
        }
    }

    void CalculateFare()
    {
        float distance = CalculateDistance(origin, destination);
        Debug.Log("Distance between A and B: " + distance);
        fare = (int) distance * 10;
    }

    public static float CalculateDistance(Location pointA, Location pointB)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(pointB.mapX - pointA.mapY, 2) + Mathf.Pow(pointB.mapX - pointA.mapY, 2));
        return distance;
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            if (destination == transientData.currentLocation)
            {
                int fortune = Player.GetCount("ATT000", "PassengerPrefabScript");
                int added = MoneyExchange.AddHighestDenomination(fare);

                AudioManager.PlayAmbientSound("handleCoins");

                // Ensure the player had inventory space for full fare
                if (added < fare)
                {
                    transientData.PushAlert("I was unable to accept the total fare. Better go to the bank!");
                }
                else
                {
                    transientData.PushAlert($"{passengerName} paid {fare} shillings.");
                }

                Player.Add("MIS000-JUN-NN", Random.Range(0, fare));


                //Spirit Essence drop
                if (Random.Range(0, 100) > 80 - (fortune * 4))
                {
                    Player.Add(spiritEssence.objectID);
                    transientData.PushAlert($"{passengerName} dropped some Spirit Essence.");
                    AudioManager.PlayAmbientSound("cloth3");
                }

                if (isPassengerA)
                    dataManager.passengerIsActiveA = false;
                else
                    dataManager.passengerIsActiveB = false;

                gameObject.SetActive(false);
            }
        }
    }

    public void OnMouseOver()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            //var locationToString = destination.ToString();
            //var locationName = Regex.Replace(locationToString, "(\\B[A-Z])", " $1");

            transientData.PrintFloatText(passengerName + "\n" + destination.name);
        }
    }

    public void OnMouseExit()
    {
        transientData.DisableFloatText();
    }
}
