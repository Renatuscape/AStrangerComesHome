using System.Collections;
using UnityEngine;

public static class DialoguePortraitHelper
{
    public static DialoguePortraitManager portraitManager; // Reference to the MonoBehaviour instance

    public static float positionClose = 230;
    public static float positionNormal = 630;
    public static float positionMid = 0;
    public static float positionFar = 860;
    public static float positionOff = 1600;

    public static float speedSlow = 1.5f;
    public static float speedMed = 0.8f;
    public static float speedFast = 0.2f;

    static bool isAnimating = false;

    static SerializableDictionary<DialogueEvent, GameObject> currentlyAnimating = new();

    public static void FinishAnimationsNow()
    {
        portraitManager.StopAllCoroutines();

        foreach (var kvp in currentlyAnimating)
        {
            var dEvent = kvp.Key;
            var container = kvp.Value;

            container.transform.localPosition = new Vector3(ParsePosition(dEvent, dEvent.targetPlacement), container.transform.localPosition.y, container.transform.localPosition.z);
        }

        currentlyAnimating = new();
    }
    public static float ParsePosition(DialogueEvent dEvent, string tag)
    {
        //OFF-FAR-NOR-CLO-MID
        float position;

        if (tag == "OFF")
        {
            position = positionOff;
        }
        else if (tag == "FAR")
        {
            position = positionFar;
        }
        else if (tag == "CLO")
        {
            position = positionClose;
        }
        else if (tag == "MID")
        {
            position = positionMid;
        }
        else
        {
            position = positionNormal;
        }

        if (dEvent.isLeft || dEvent.speaker.objectID == "ARC000")
        {
            position = position * -1;
        }

        return position;
    }

    public static float ParseAnimationSpeed(string tag)
    {
        // NON-SLO-MED-FAS
        float timeToFinish;

        if (tag == "NON")
        {
            timeToFinish = 0f;
        }
        else if (tag == "SLO")
        {
            timeToFinish = speedSlow;
        }
        else if (tag == "FAS")
        {
            timeToFinish = speedFast;
        }
        else
        {
            timeToFinish = speedMed;
        }

        // Debug.Log("Attempting to transition with speed " + timeToFinish);
        return timeToFinish;
    }

    public static void SetStartPosition(DialogueEvent dEvent, GameObject spriteContainer)
    {
        //OFF-FAR-NOR-CLO-MID
        float startPosition;

        if (dEvent.startingPlacement != null && !string.IsNullOrEmpty(dEvent.startingPlacement))
        {
            startPosition = ParsePosition(dEvent, dEvent.startingPlacement);
        }
        else
        {
            startPosition = positionNormal;
        }

        // Debug.Log("Start position for event sprite is " + startPosition);
        spriteContainer.transform.localPosition = new Vector3(startPosition, spriteContainer.transform.localPosition.y, spriteContainer.transform.localPosition.z);
    }

    public static void SetTargetPosition(DialogueEvent dEvent, GameObject spriteContainer)
    {
        var position = ParsePosition(dEvent, dEvent.targetPlacement);

        if (spriteContainer.transform.localPosition.x != position)
        {
            if (isAnimating)
            {
                FinishAnimationsNow();
            }

            var time = ParseAnimationSpeed(dEvent.moveAnimationSpeed);

            if (portraitManager == null)
            {
                Debug.LogError("Portrait manager was null. Make sure dialogueHelper has a monobehaviour to reference.");
            }
            if (dEvent == null)
            {
                Debug.Log("Event was null.");
            }
            if (spriteContainer == null)
            {
                Debug.Log("Sprite container was null.");
            }
            portraitManager.StartCoroutine(TransitionToTarget(dEvent, position, time, spriteContainer));

            // check event for secondary character animation
        }
    }

    static IEnumerator TransitionToTarget(DialogueEvent dEvent, float targetPosition, float transitionTime, GameObject spriteContainer)
    {
        isAnimating = true;
        currentlyAnimating.Add(dEvent, spriteContainer);

        Vector3 startPosition = spriteContainer.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            float t = elapsedTime / transitionTime;
            float newPositionX = Mathf.Lerp(startPosition.x, targetPosition, t);
            Vector3 newPosition = new Vector3(newPositionX, startPosition.y, startPosition.z);
            spriteContainer.transform.localPosition = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the target position is reached exactly
        spriteContainer.transform.localPosition = new Vector3(targetPosition, startPosition.y, startPosition.z);

        currentlyAnimating.Remove(dEvent);
        isAnimating = false;
    }
}