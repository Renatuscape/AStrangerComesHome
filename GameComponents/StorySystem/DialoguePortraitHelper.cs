using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePortraitHelper : MonoBehaviour
{
    public const float positionClose = 230;
    public const float positionNormal = 630;
    public const float positionMid = 0;
    public const float positionFar = 860;
    public float positionOff = 1600;

    const float speedSlow = 1.3f;
    const float speedMed = 0.7f;
    const float speedFast = 0.2f;

    public bool isAnimating = false;
    public bool completeAllTransitionsNow = false;
    public float animationTimer;
    float animationTick = 0.015f;

    public List<PortraitTransitionData> portraitTransitions = new();
    public List<PortraitTransitionData> completedAnimations = new();

    private void Update()
    {
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;

            if (animationTimer >= animationTick)
            {
                animationTimer = 0;
                Animate();
            }
        }
    }

    public void CompleteAnimationsNow()
    {
        foreach (var transition in portraitTransitions)
        {
            transition.portrait.transform.localPosition = new Vector3(transition.targetX, transition.portrait.transform.localPosition.y, 0);
            completedAnimations.Add(transition);
        }

        completeAllTransitionsNow = false;
    }

    public void Animate()
    {
        if (isAnimating && portraitTransitions.Count > 0)
        {
            if (completeAllTransitionsNow)
            {
                CompleteAnimationsNow();
            }
            else
            {
                foreach (var transition in portraitTransitions)
                {
                    if (!transition.isComplete)
                    {
                        if (transition.isMovingRight)
                        {
                            transition.portrait.transform.localPosition = new Vector3(transition.portrait.transform.localPosition.x + transition.distancePerTick, transition.portrait.transform.localPosition.y, 0);

                            if (transition.portrait.transform.localPosition.x >= transition.targetX)
                            {
                                transition.portrait.transform.localPosition = new Vector3(transition.targetX, transition.portrait.transform.localPosition.y, 0);
                                transition.isComplete = true;
                            }
                        }
                        else
                        {
                            transition.portrait.transform.localPosition = new Vector3(transition.portrait.transform.localPosition.x - transition.distancePerTick, transition.portrait.transform.localPosition.y, 0);
                            
                            if (transition.portrait.transform.localPosition.x <= transition.targetX)
                            {
                                transition.portrait.transform.localPosition = new Vector3(transition.targetX, transition.portrait.transform.localPosition.y, 0);
                                transition.isComplete = true;
                            }
                        }
                    }
                    else
                    {
                        completedAnimations.Add(transition);
                    }
                }
            }
        }

        if (completedAnimations.Count > 0)
        {
            foreach (var transition in completedAnimations)
            {
                portraitTransitions.Remove(transition);
            }

            completedAnimations.Clear();
        }

        if (portraitTransitions.Count == 0)
        {
            isAnimating = false;
        }
    }

    public float ParsePosition(DialogueEvent dEvent, string tag)
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

    public float ParseAnimationSpeed(string tag)
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

        return timeToFinish;
    }

    public void SetStartPosition(DialogueEvent dEvent, GameObject spriteContainer)
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

        spriteContainer.transform.localPosition = new Vector3(startPosition, spriteContainer.transform.localPosition.y, spriteContainer.transform.localPosition.z);
    }

    public void SetTargetPosition(DialogueEvent dEvent, GameObject spriteContainer)
    {
        var targetPosition = ParsePosition(dEvent, dEvent.targetPlacement);
        var startPosition = spriteContainer.transform.localPosition.x;

        if (startPosition != targetPosition)
        {
            if (isAnimating)
            {
                completeAllTransitionsNow = true;
            }

            var time = ParseAnimationSpeed(dEvent.moveAnimationSpeed);
            var travelDistance = startPosition - targetPosition;
            int totalFrames = Mathf.CeilToInt(time / animationTick); // Calculate total frames based on animation time and tick interval
            float distancePerFrame = travelDistance / totalFrames;
            bool isMovingRight = false;

            if (startPosition < targetPosition)
            {
                isMovingRight = true;
            }

            if (distancePerFrame < 0)
            {
                distancePerFrame = distancePerFrame * -1;
            }

            portraitTransitions.Add(new()
            {
                portrait = spriteContainer,
                startX = startPosition,
                targetX = targetPosition,
                time = time,
                isMovingRight = isMovingRight,
                distancePerTick = distancePerFrame
            });

            isAnimating = true;
        }
    }

    [System.Serializable]
    public class PortraitTransitionData
    {
        public GameObject portrait;
        public float startX;
        public float targetX;
        public float distancePerTick;
        public float time;
        public bool isMovingRight;
        public bool isComplete;
    }
}