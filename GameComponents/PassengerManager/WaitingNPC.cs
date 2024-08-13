using System.Collections;
using UnityEngine;

public class WaitingNPC : MonoBehaviour
{
    TransientDataScript transientData;
    public Location destination;
    public Location origin;
    public GameObject parent;
    public PassengerManager passengerManager;
    public float parallaxMultiplier;
    public float alpha;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        alpha = 0;
        StartCoroutine(SpawnWithFade());
    }
    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        transientData.activePrefabs.Add(gameObject);
        origin = transientData.currentLocation;
        spriteRenderer.color = new Color(1, 1, 1, alpha);
        Invoke("SetUpPassenger", 0.01f);
        RollDestination();
    }

    IEnumerator SpawnWithFade() //Fades in, used at spawn
    {
        yield return new WaitForSeconds(.008f);
        alpha = alpha + 0.01f;
        spriteRenderer.color = new Color(1, 1, 1, alpha);

        if (alpha < 1)
            StartCoroutine(SpawnWithFade());
    }

    void SetUpPassenger()
    {
        if (parent != null)
        {
            passengerManager = parent.GetComponent<PassengerManager>();
            transform.parent = parent.transform;
            ParallaxControllerHelper.AddObjectToParallax(gameObject, "Road");
            passengerManager.waitingCurrent += 1;
        }
        else if (parent == null)
        {
            Debug.Log("No parent found for Waiting NPC. Trying again.");
            parent = GameObject.Find("PassengerManager");
            Invoke("SetUpPassenger", 0.01f);
        }
    }

    void RollDestination()
    {
        int random = Random.Range(0, transientData.currentRegion.locations.Count - 1);
        destination = transientData.currentRegion.locations[random];

        if (destination == null ||
            destination.noPassengers ||
            destination == transientData.currentLocation ||
            destination.type == LocationType.Crossing ||
            CalculateDistance(origin, destination) < 3 ||
            !RequirementChecker.CheckRequirements(destination.requirements))
        {
            RollDestination();
        }
    }

    public static float CalculateDistance(Location pointA, Location pointB)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(pointB.mapX - pointA.mapX, 2) + Mathf.Pow(pointB.mapY - pointA.mapY, 2));
        return distance;
    }

    void LateUpdate()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            if ((transform.position.x <= -19 || transform.position.x >= 19) && transientData.currentLocation == null)
            {
                passengerManager.waitingCurrent -= 1;
                transientData.activePrefabs.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal && TransientDataScript.transientData.currentSpeed == 0)
        {
            if (Player.GetCount(StaticTags.GuildLicense, name) > 0)
            {
                passengerManager.ActivateWaitingPassenger(gameObject);
            }
            else
            {
                TransientDataScript.PushAlert("Potential passengers are asking to see a license. What's that about?");
            }
        }
    }

    public void OnMouseOver()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            if (Player.GetCount(StaticTags.GuildLicense, name) > 0)
            {
                TransientDataScript.PrintFloatText("\'I'd like to go to\n" + destination.name + ", please.\'");
            }
        }
    }

    public void OnMouseExit()
    {
        TransientDataScript.DisableFloatText();
    }
}
