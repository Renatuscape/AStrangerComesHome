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
    public List<GameObject> waitingPassengers = new();

    public int waitingMax = 5;
    public int waitingCurrent;
    string[] listOfLastNames = new string[] { "Greene", "Winters", "Fallow", "Graham", "Dale", "Creek", "Shoal", "Carpenter", "Baker", "Forester", "Blake", "River", "Cliff", "Tallow", "Shelligh", "Wyrde", "Crag", "Scree", "Smith", "Cooper", "Mere", "Stahl", "Varde", "Cairn", "Ampersand", "Reed", "Lorn", "Hale", "Rowan", "Ash", "Moore", "Wood", "Bracken", "Stone", "Fletcher", "Wright", "Mason", "Parker", "Ward", "Hunter", "Marshall", "Webb", "Miller", "Thorne", "Heath", "Grove", "Cobb", "Wardle", "Foster", "Ellis", "Lane", "Barrow", "Caldwell", "Trent", "Hurst", "Blair", "Wren", "Byrne", "Drake", "Keane", "Thorpe", "Fane", "Axton", "Hayes", "Holt", "Kirk", "Saxon", "Ridge", "Harte", "Wade", "Faulkner", "Hendrix", "Brock", "Darcy", "Lyle", "Underwood", "Bolton", "Wainwright", "Sinclair", "Mallory", "Kane", "Granger", "Nash", "Harper", "Elliott", "Sawyer", "Turner", "Spencer", "Draper", "MacGregor", "Grant", "Fitzgerald", "Morrison", "Baldwin", "Hughes", "Lawson", "Osborne", "Quinn", "Shepherd", "Sanders", "Barrett", "Clark", "Harrington", "Jenkins", "Roberts", "Thornton", "Wells", "Young", "Bishop", "Hart", "Nelson", "Porter", "Quincy", "Stevens", "Taylor", "Vincent", "Walker", "Yates", "Blackwood", "Cameron", "Douglas", "Everett", "Fisher", "Gallagher", "Holden", "Irving", "Jefferson", "Kendall", "Lindsay", "Milton", "Norwood", "Orton", "Prescott", "Reynolds", "Shelton", "Thompson", "Vernon", "Whittaker", "York", "Zimmerman", "Aldridge", "Benson", "Crawford", "Duncan", "Ellington", "Fleming", "Garrett", "Hampton", "Ingram", "Jarvis", "Knight", "Lancaster", "Montgomery", "North", "Ogden", "Peters", "Quinlan", "Raleigh", "Sheldon", "Travis", "Ulrich", "Vaughn", "Whitmore", "Yardley", "Zane", "Anderson", "Benson", "Chapman", "Dawson", "Edwards", "Fairchild", "Griffith", "Hamilton", "Iverson", "Jameson", "Kerr", "Lambert", "Merrill", "Norton", "Orchard", "Powell", "Richards", "Stanton", "Truman", "Upton", "Vance", "Warner", "Yardley", "Zephyr", "Pteropus " };
    string[] listOfFirstNames = new string[] { "Phoenix", "Kevin", "Charles", "Mauve", "Jeremiah", "Aaron", "Chandler", "Preston", "Winston", "Elliott", "Lliam", "Sterling", "Caine", "Chauncey", "Paige", "Winfrey", "Leslie", "Morgan", "Arthur", "Lindsey", "Quinn", "Corin", "Ava", "Harlan", "Elijah", "Francis", "Colin", "Trevor", "Adrian", "Ida", "Hilda", "Marie", "Willow", "Gavin", "Brooke", "Fiona", "Dean", "Evangeline", "Sawyer", "Hugo", "Bryce", "Daphne", "Rowan", "Giselle", "August", "Mabel", "Dexter", "Phoebe", "Nolan", "Vivian", "Grant", "Iris", "Felix", "Maeve", "Julian", "Clara", "Desmond", "Audrey", "Tristan", "Blair", "Reid", "Scarlett", "Quentin", "Freya", "Holden", "Hazel", "Garrett", "Dahlia", "Levi", "Florence", "Byron", "Emmeline", "Isaac", "Celeste", "Donovan", "Blythe", "Silas", "Greta", "Ronan", "Ivy", "Everett", "Beatrice", "Milo", "Elsa", "Wyatt", "Lydia", "Jasper", "Nina", "Rhys", "Esme", "Finn", "Cleo", "Maxwell", "Estelle", "Beckett", "Jade", "Ezra", "Rose", "Nathaniel", "Pearl", "Cassian", "Elise", "Lachlan", "Tess", "Harrison", "Juliet", "Asher", "Opal", "Tobias", "Violet", "Griffin", "Seraphina", "Emmett", "Thea", "Hudson", "Cora", "Leander", "Matilda", "Dorian", "Imogen", "Kai", "Helena", "Orson", "Lucille", "Tate", "Mae", "Cyrus", "Annabel", "Jude", "Willa", "Benedict", "Verity", "Soren", "Poppy", "Sullivan", "Luna", "Ansel", "Delilah", "Cormac", "Ada", "Alaric", "Mira", "Gwen", "Rhett", "Bianca", "Bastian", "Camille", "Phineas", "Daisy", "Eamon", "Leonie", "Rory", "Mavis", "Stellan", "Felicity", "Theron", "Aurora", "Evander", "Georgia", "Calvin", "Magnolia", "Cullen", "Rosalind", "Lorcan", "Penelope", "Edmund", "Agnes", "Leland", "Constance", "Amos", "Harriet", "Percy", "Beulah", "Albert", "Maude", "Ephraim", "Myrtle", "Bertram", "Lenore", "Cedric", "Wilhelmina", "Alfred", "Mildred", "Clement", "Dorothea", "Abel", "Eudora", "Howard", "Verona", "Gilbert", "Prudence", "Horace", "Lucretia", "Maurice", "Eleanor", "Rupert", "Clarice", "Reuben", "Harriett", "Theodore", "Winifred", "Virgil", "Minerva", "Ellis", "Ophelia", "Cecil", "Gwendolyn", "Wilfred", "Augusta", "Orville", "Frances", "Wallace", "Etta", "Archibald", "Clyde", "Florine", "Roscoe", "Leona", "Milton", "Amelia", "Norman", "Eloise", "Reginald", "Henrietta", "Lionel", "Josephine", "Basil", "Cornelia", "Frederick", "Lavinia", "Virginia", "Ernest", "Octavia", "Roderick", "Gertrude", "Vernon", "Claudia", "Laurence", "Sybil", "Stanley", "Genevieve", "Malcolm", "Isadora", "Harold", "Blanche", "Wilbur", "Martha", "Eugene", "Clementine", "Sidney", "Nora", "Vincent", "Helene", "Warren", "Minnie", "Eldon", "Marion" };

    public bool isReady;
    public void Initialise()
    {
        instance = this;

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
            dataManager.seatA.satisfaction = 3;
            dataManager.seatA.eatingChance = Random.Range(5f, 15f);

            passengerA.gameObject.SetActive(true);
            passengerA.UpdatePassengerData();
        }
        else if (dataManager.seatB.isActive == false)
        {
            dataManager.seatB.isActive = true;
            dataManager.seatB.passengerName = passengerName;
            dataManager.seatB.origin = origin;
            dataManager.seatB.destination = destination;
            dataManager.seatB.satisfaction = 3;
            dataManager.seatB.eatingChance = Random.Range(5f, 15f);

            while (dataManager.seatA.spriteID == passengerSprite.name)
            {
                passengerSprite = SpriteFactory.GetRandomPassengerSprite();
            }

            dataManager.seatB.spriteID = passengerSprite.name;

            passengerB.gameObject.SetActive(true);
            passengerB.UpdatePassengerData();
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

            waitingPassengers.Add(waitingPassenger);
        }
    }

    public static void GlobalPushPassengerSpawn()
    {
        if (instance != null && !TransientDataScript.transientData.currentRegion.disablePassengers)
        {
            instance.PassengerTick();
        }
    }

    void PassengerTick()
    {
        if (isReady && TransientDataScript.IsTimeFlowing())
        {
            var currentLocation = TransientDataScript.GetCurrentLocation();

            if (currentLocation != null && !string.IsNullOrEmpty(currentLocation.objectID))
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
            else if (!dataManager.seatA.isActive && passengerA.gameObject.activeInHierarchy)
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

    public void ForceRemoveWaiters()
    {
        foreach (var passenger in waitingPassengers)
        {
            Destroy(passenger);
        }

        waitingCurrent = 0;
        waitingPassengers.Clear();
    }
    private void OnDisable()
    {
        passengerA.gameObject.SetActive(false);
        passengerB.gameObject.SetActive(false);

        ForceRemoveWaiters();
    }
}