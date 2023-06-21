using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.UI.Image;

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
        destination = (Location)Random.Range(0, System.Enum.GetValues(typeof(Location)).Length - 1);

        if (destination == Location.None || destination == Location.VerdantEstate || destination == transientData.currentLocation)
        {
            RollDestination();
            //Debug.Log("Destination reroll");
        }
    }

    void LateUpdate()
    {
        var parallaxEffect = transientData.currentSpeed * parallaxMultiplier;

        transform.position = new Vector3(transform.position.x + parallaxEffect, transform.position.y, transform.position.z);

        if ((transform.position.x <= -20 || transform.position.x >= 20) && transientData.currentLocation == Location.None)
        {
            passengerManager.waitingCurrent -= 1;
            transientData.activePrefabs.Remove(gameObject);
            Destroy(gameObject);
        }
        else if (transform.position.x < -20)
            transform.position = new Vector3(18, transform.position.y, transform.position.z);

        else if (transform.position.x > 20)
            transform.position = new Vector3(-18, transform.position.y, transform.position.z);
    }

    private void OnMouseDown()
    {
        if (transientData.gameState == GameState.Overworld && transientData.cameraView == CameraView.Normal)
        {
            passengerManager.ActivateWaitingPassenger(gameObject);
        }
    }

    public void OnMouseOver()
    {
        var locationToString = destination.ToString();
        var locationName = Regex.Replace(locationToString, "(\\B[A-Z])", " $1");
        transientData.MousePop("\'I'd like to go to " + locationName + ", please.\'");
    }
}
