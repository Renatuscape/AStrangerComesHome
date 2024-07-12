using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePortraitManager : MonoBehaviour
{
    // Portraits should be anchored at centre. If anchor changes, these measurements will be off.

    public DialoguePortraitHelper portraitAnimator;
    public Image spriteLeft;
    public Image spriteRight;
    public DialoguePlayerSprite playerSprite;

    private void OnEnable()
    {
        var starterPosition = portraitAnimator.positionOff;

        spriteRight.gameObject.SetActive(false);
        spriteRight.gameObject.transform.localPosition = new Vector3(starterPosition, spriteRight.gameObject.transform.localPosition.y, spriteRight.gameObject.transform.localPosition.z);
        spriteLeft.gameObject.SetActive(false);
        spriteLeft.gameObject.transform.localPosition = new Vector3(starterPosition * -1, spriteLeft.gameObject.transform.localPosition.y, spriteLeft.gameObject.transform.localPosition.z);
        playerSprite.gameObject.SetActive(false);
        playerSprite.gameObject.transform.localPosition = new Vector3(starterPosition * -1, playerSprite.gameObject.transform.localPosition.y, playerSprite.gameObject.transform.localPosition.z);
    }

    public void SetRightPortrait(string speakerID)
    {
        spriteRight.sprite = SpriteFactory.GetUiSprite(speakerID).GetDefaultFrame();
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
            spriteLeft.sprite = dEvent.speaker.spriteCollection.GetFirstFrameFromEvent(dEvent.spriteID);
            playerSprite.gameObject.SetActive(false);
            spriteObject = spriteLeft.gameObject;
        }
        else
        {
            spriteRight.sprite = dEvent.speaker.spriteCollection.GetFirstFrameFromEvent(dEvent.spriteID); // dEvent.speaker.sprite; // Replace with call to sprite manager later
            spriteObject = spriteRight.gameObject;
        }

        if (spriteObject != null)
        {
            spriteObject.SetActive(true);

            if (portraitAnimator.isAnimating && !string.IsNullOrEmpty(dEvent.targetPlacement))
            {
                portraitAnimator.CompleteAnimationsNow();
            }

            if (!string.IsNullOrEmpty(dEvent.startingPlacement))
            {
                portraitAnimator.SetStartPosition(dEvent, spriteObject);
            }

            if (!string.IsNullOrEmpty(dEvent.targetPlacement))
            {
                portraitAnimator.SetTargetPosition(dEvent, spriteObject);
            }
        }
    }
}
