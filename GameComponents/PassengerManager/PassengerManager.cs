using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public static PassengerManager instance;
    public GameObject waitingPrefab;
    public PassengerPrefabScript passengerA;
    public PassengerPrefabScript passengerB;

    public int waitingMax = 5;
    public int waitingCurrent;
    string[] listOfLastNames = new string[] { "Greene", "Winters", "Fallow", "Graham", "Dale", "Creek", "Shoal", "Carpenter", "Baker", "Forester", "Blake", "River", "Cliff", "Tallow", "Shelligh", "Wyrde", "Crag", "Scree", "Smith", "Cooper", "Mere", "Stahl", "Varde", "Cairn", "Ampersand", "Reed", "Lorn" };
    string[] listOfFirstNames = new string[] { "Mauve", "Jeremiah", "Aaron", "Chandler", "Preston", "Winston", "Elliott", "Lliam", "Sterling", "Caine", "Chauncey", "Paige", "Winfrey", "Leslie", "Morgan", "Arthur", "Lindsey", "Quinn", "Corin", "Ava", "Harlan", "Elijah", "Francis", "Colin", "Trevor", "Adrian", "Ida", "Hilda", "Marie", "Willow" };

    public bool isReady;
    public void Initialise()
    {
        instance = this;
        waitingCurrent = 0;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();

        dataManager.seatA.seatID = "A";
        passengerA.gameObject.SetActive(false);
        passengerA.Initialise(dataManager.seatA);

        dataManager.seatB.seatID = "B";
        passengerB.gameObject.SetActive(false);
        passengerB.Initialise(dataManager.seatB);

        isReady = true;
    }
    public void ActivateWaitingPassenger(GameObject potentialPassenger)
    {
        if (dataManager.seatA.isActive == false || dataManager.seatB.isActive == false)
        {
            //GENERATE NAME
            var passengerFirstName = listOfFirstNames[Random.Range(0, listOfFirstNames.Length)];

            var passengerLastName = listOfLastNames[Random.Range(0, listOfLastNames.Length)];
            var generatedName = passengerFirstName + " " + passengerLastName;

            //CHOOSE SPRITE
            var passengerSprite = SpriteFactory.GetRandomPassengerSprite();

            var origin = potentialPassenger.GetComponent<WaitingNPC>().origin;
            var destination = potentialPassenger.GetComponent<WaitingNPC>().destination;

            BoardPassenger(generatedName, passengerSprite, origin, destination);
            Destroy(potentialPassenger);
            waitingCurrent -= 1;

            LogAlert.QueueTextAlert("I picked up " + generatedName + ", who is going to " + destination.name + ".");
        }
        else
        {
            LogAlert.QueueTextAlert("There are no available seats.");
        }
    }

    public void BoardPassenger(string passengerName, Sprite passengerSprite, Location origin, Location destination)
    {
        //DEPLOY SPRITE AND UPDATE DATA MANAGER
        if (dataManager.seatA.isActive == false)
        {
            dataManager.seatA.isActive = true;
            dataManager.seatA.passengerName = passengerName;
            dataManager.seatA.origin = origin;
            dataManager.seatA.destination = destination;
            dataManager.seatA.spriteID = passengerSprite.name;

            passengerA.gameObject.SetActive(true);
            passengerA.UpdatePassengerData();
        }
        else if (dataManager.seatB.isActive == false)
        {
            dataManager.seatB.isActive = true;
            dataManager.seatB.passengerName = passengerName;
            dataManager.seatB.origin = origin;
            dataManager.seatB.destination = destination;

            while (dataManager.seatA.spriteID == passengerSprite.name)
            {
                passengerSprite = SpriteFactory.GetRandomPassengerSprite();
            }

            dataManager.seatB.spriteID = passengerSprite.name;

            passengerB.gameObject.SetActive(true);
            passengerA.UpdatePassengerData();
        }
    }
    public void PassengerSpawner()
    {
        if (isReady)
        {
            var waitingPassenger = Instantiate(waitingPrefab);
            var spawnArea = Random.Range(-15f, -4f);

            waitingPassenger.name = "WaitingPassenger";
            waitingPassenger.transform.position = new Vector3(spawnArea, -4.094f, 0f);
            waitingPassenger.GetComponent<WaitingNPC>().parent = gameObject;
        }
    }

    public static void GlobalPushPassengerSpawn()
    {
        if (instance != null)
        {
            instance.PassengerTick();
        }
    }
    void PassengerTick()
    {
        if (isReady && TransientDataScript.IsTimeFlowing())
        {
            var currentLocation = TransientDataScript.GetCurrentLocation();

            if (currentLocation != null)
            {
                if (!currentLocation.noPassengers)
                {
                    if (currentLocation.type == LocationType.City)
                    {
                        waitingMax = 7;
                    }
                    else if (currentLocation.type == LocationType.Town)
                    {
                        waitingMax = 5;
                    }
                    else if (currentLocation.type == LocationType.Settlement)
                    {
                        waitingMax = 3;
                    }
                    else
                    {
                        waitingMax = 2;
                    }

                    if (waitingCurrent < waitingMax)
                    {
                        var randomRoll = Random.Range(0, 100);

                        if (randomRoll >= 80)
                        {
                            PassengerSpawner();
                        }
                    }
                }
            }

            if (dataManager.seatA.isActive && passengerA.gameObject.activeInHierarchy == false)
            {
                passengerA.UpdatePassengerData();
                passengerA.gameObject.SetActive(true);
            }
            else if(!dataManager.seatA.isActive && passengerA.gameObject.activeInHierarchy)
            {
                passengerA.gameObject.SetActive(false);
            }

            if (dataManager.seatB.isActive && passengerB.gameObject.activeInHierarchy == false)
            {
                passengerB.UpdatePassengerData();
                passengerB.gameObject.SetActive(true);
            }
            else if (!dataManager.seatB.isActive && passengerB.gameObject.activeInHierarchy)
            {
                passengerB.gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        passengerA.gameObject.SetActive(false);
        passengerB.gameObject.SetActive(false);
    }
}