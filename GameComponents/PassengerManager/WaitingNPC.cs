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
        int random = Random.Range(0, transientData.currentRegion.locations.Count - 1);
        destination = transientData.currentRegion.locations[random];

        if (destination == null || destination.noPassengers || destination == transientData.currentLocation || destination.type == LocationType.Crossing)
        {
            RollDestination();
        }
    }

    void LateUpdate()
    {
        var parallaxEffect = transientData.currentSpeed * parallaxMultiplier;

        transform.position = new Vector3(transform.position.x + parallaxEffect, transform.position.y, transform.position.z);

        if ((transform.position.x <= -20 || transform.position.x >= 20) && transientData.currentLocation is null)
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
        if (Player.GetCount("SCR013-SCR-NN", name) > 0)
        {
            if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
            {
                passengerManager.ActivateWaitingPassenger(gameObject);
            }
        }
        else
        {
            TransientDataScript.PushAlert("What's all this talk about a guild license? I should ask around.");
        }
    }

    public void OnMouseOver()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            if (Player.GetCount("SCR013-SCR-NN", name) > 0)
            {
                TransientDataScript.PrintFloatText("\'I'd like to go to\n" + destination.name + ", please.\'");
            }
            else
            {
                TransientDataScript.PrintFloatText("\'Isn't there a guild licensed coach arriving soon?\'");
            }
        }
    }

    public void OnMouseExit()
    {
        TransientDataScript.DisableFloatText();
    }
}
