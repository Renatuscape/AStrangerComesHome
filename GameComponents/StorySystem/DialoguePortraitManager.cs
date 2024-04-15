using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePortraitManager : MonoBehaviour
{
    // Portraits should be anchored at centre. If anchor changes, these measurements will be off.

    public Image spriteLeft;
    public Image spriteRight;
    public PlayerSprite playerSprite;

    float positionClose = 230;
    float positionNormal = 630;
    float positionMid = 0;
    float positionFar = 860;
    float positionOff = 1600;

    //float portraitWidth = 1228;
    //float portraitHeight = 1736;

    float animationTick;
    float animationTimer;

    bool isAnimating = false;

    private void OnEnable()
    {
        spriteRight.gameObject.SetActive(false);
        spriteRight.gameObject.transform.position = new Vector3(positionNormal, spriteRight.gameObject.transform.position.y, spriteRight.gameObject.transform.position.z);
        spriteLeft.gameObject.SetActive(false);
        spriteLeft.gameObject.transform.position = new Vector3(positionNormal * -1, spriteLeft.gameObject.transform.position.y, spriteLeft.gameObject.transform.position.z);
        playerSprite.gameObject.SetActive(false);
        playerSprite.gameObject.transform.position = new Vector3(positionNormal * -1, playerSprite.gameObject.transform.position.y, playerSprite.gameObject.transform.position.z);
        
        isAnimating = false;
        animationTimer = 0;
    }

    private void Update()
    {
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;

            if (animationTimer > animationTick)
            {
                animationTimer = 0;
            }
        }
    }
    public void StartDialogueEvent(DialogueEvent dEvent)
    {
        gameObject.SetActive(true);

        GameObject spriteObject;

        if (dEvent.speaker.objectID == "ARC000")
        {
            spriteLeft.gameObject.SetActive(false);
            spriteObject = playerSprite.gameObject;
        }
        else if (dEvent.speaker.objectID == "ARC999")
        {
            spriteObject = null;
        }
        else if (dEvent.isLeft)
        {
            spriteLeft.sprite = dEvent.speaker.sprite; // Replace with call to sprite manager later
            playerSprite.gameObject.SetActive(false);
            spriteObject = spriteLeft.gameObject;
        }
        else
        {
            spriteRight.sprite = dEvent.speaker.sprite; // Replace with call to sprite manager later
            spriteObject = spriteRight.gameObject;
        }

        if (spriteObject != null)
        {
            SetStartPosition(dEvent, spriteObject);
        }
    }

    void SetStartPosition(DialogueEvent dEvent, GameObject spriteContainer)
    {
        //OFF-FAR-NOR-CLO-MID
        float startPosition;

        if (dEvent.startingPlacement != null && !string.IsNullOrEmpty(dEvent.startingPlacement))
        {
            if (dEvent.startingPlacement == "OFF")
            {
                startPosition = positionOff;
            }
            else if (dEvent.startingPlacement == "FAR")
            {
                startPosition = positionFar;
            }
            else if (dEvent.startingPlacement == "CLO")
            {
                startPosition = positionClose;
            }
            else if (dEvent.startingPlacement == "MID")
            {
                startPosition = positionMid;
            }
            else
            {
                startPosition = positionNormal;
            }
        }
        else
        {
            startPosition = positionNormal;
        }

        if (dEvent.isLeft || dEvent.speaker.objectID == "ARC000")
        {
            startPosition = startPosition * -1;
        }

        Debug.Log("Start position for event sprite is " + startPosition);
        spriteContainer.transform.localPosition = new Vector3(startPosition, spriteContainer.transform.localPosition.y, spriteContainer.transform.localPosition.z);
        spriteContainer.SetActive(true);
    }
}
