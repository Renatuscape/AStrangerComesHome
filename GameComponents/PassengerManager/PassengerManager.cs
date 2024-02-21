using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public DataManagerScript dataManager;
    public GameObject waitingPrefab;
    public GameObject passengerA;
    public GameObject passengerB;

    public List<Sprite> passengerSpriteList;

    public float spawnTimer;
    public float spawnDelay;

    public int waitingMax = 5;
    public int waitingCurrent;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        spawnDelay = 5;

        passengerA.GetComponent<PassengerPrefabScript>().isPassengerA = true;
        passengerA.SetActive(false);
        passengerB.SetActive(false);
    }
    public void ActivateWaitingPassenger(GameObject potentialPassenger)
    {
        if (dataManager.passengerIsActiveA == false || dataManager.passengerIsActiveB == false)
        {
            //GENERATE NAME
            string[] listOfFirstNames = new string[] { "Mauve", "Jeremiah", "Aaron", "Chandler", "Preston", "Winston", "Elliott", "Lliam", "Sterling", "Caine", "Chauncey", "Paige", "Winfrey", "Leslie", "Morgan", "Arthur", "Lindsey", "Quinn", "Corin", "Ava", "Harlan", "Elijah", "Francis", "Colin", "Trevor", "Adrian", "Ida", "Hilda", "Marie", "Willow" };
            var passengerFirstName = listOfFirstNames[Random.Range(0, listOfFirstNames.Length)];

            string[] listOfLastNames = new string[] { "Greene", "Winters", "Fallow", "Graham", "Dale", "Creek", "Shoal", "Carpenter", "Baker", "Forester", "Blake", "River", "Cliff", "Tallow", "Shelligh", "Wyrde", "Crag", "Scree", "Smith", "Cooper", "Mere", "Stahl", "Varde", "Cairn", "Ampersand", "Reed", "Lorn" };
            var passengerLastName = listOfLastNames[Random.Range(0, listOfLastNames.Length)];
            var generatedName = passengerFirstName + " " + passengerLastName;

            //CHOOSE SPRITE
            var passengerSprite = passengerSpriteList[Random.Range(0, passengerSpriteList.Count - 1)];

            var origin = potentialPassenger.GetComponent<WaitingNPC>().origin;
            var destination = potentialPassenger.GetComponent<WaitingNPC>().destination;

            var dialogueList = new List<string>(); //grab a random dialogue string
            dialogueList.Add("Hey!"); //FOR TESTING

            BoardPassenger(generatedName, passengerSprite, origin, destination, dialogueList);
            Destroy(potentialPassenger);
            waitingCurrent -= 1;

            transientData.PushAlert("I picked up " + generatedName + ", who is going to " + destination.name + ".");
        }
        else
            transientData.PushAlert("There are no available seats.");
    }

    public void BoardPassenger(string passengerName, Sprite passengerSprite, Location origin, Location destination, List<string> dialogueList)
    {
        //DEPLOY SPRITE AND UPDATE DATA MANAGER
        if (dataManager.passengerIsActiveA == false)
        {
            dataManager.passengerIsActiveA = true;
            dataManager.passengerNameA = passengerName;
            dataManager.passengerOriginA = origin;
            dataManager.passengerDestinationA = destination;
            dataManager.passengerChatListA = dialogueList;

            passengerA.SetActive(true);
        }
        else if (dataManager.passengerIsActiveB == false)
        {
            dataManager.passengerIsActiveB = true;
            dataManager.passengerNameB = passengerName;
            dataManager.passengerOriginB = origin;
            dataManager.passengerDestinationB = destination;
            dataManager.passengerChatListB = dialogueList;

            passengerB.SetActive(true);
        }
    }
    public void PassengerSpawner()
    {
        var waitingPassenger = Instantiate(waitingPrefab);
        var spawnArea = Random.Range(-15f, -4f);

        waitingPassenger.name = "WaitingPassenger";
        waitingPassenger.transform.position = new Vector3(spawnArea, -4.094f, 0f);
        waitingPassenger.GetComponent<WaitingNPC>().parent = gameObject;

        spawnDelay = Random.Range(2f, 15f);
        spawnTimer = 0;
    }

    private void Update()
    {
        spawnTimer = spawnTimer + Time.deltaTime;

        if (transientData.currentLocation != null && !string.IsNullOrWhiteSpace(transientData.currentLocation.objectID) && spawnTimer >= spawnDelay)
        {
            if (!transientData.currentLocation.noPassengers)
            {
                if (transientData.currentLocation.type == LocationType.City)
                {
                    waitingMax = 7;
                }
                else if (transientData.currentLocation.type == LocationType.Town)
                {
                    waitingMax = 5;
                }
                else if (transientData.currentLocation.type == LocationType.Settlement)
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

                    if (randomRoll >= 90)
                    {
                        PassengerSpawner();
                    }
                }
            }
        }

        if (dataManager.passengerIsActiveA == true && passengerA.activeInHierarchy == false)
            passengerA.SetActive(true);
        if (dataManager.passengerIsActiveB == true && passengerB.activeInHierarchy == false)
            passengerB.SetActive(true);
    }
}