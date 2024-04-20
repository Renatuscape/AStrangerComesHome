using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_BobLoop : MonoBehaviour
{
    public GameObject shadow;
    public float bobRange = 6;
    public float bobMagnitude = 1f;
    public float animationTick = 0.12f;
    public bool bobUpwards = false;
    public bool paused = false;
    float currentBob = 0;
    bool isGoingDown = false;
    float timer = 0;

    Vector3 origin;

    private void Awake()
    {
        origin = transform.localPosition;
    }

    public void PauseAtOrigin()
    {
        paused = true;
        transform.localPosition = origin;
    }

    void Update()
    {
        if (!paused)
        {
            timer += Time.deltaTime;

            if (timer >= animationTick)
            {
                timer = 0;

                if (isGoingDown)
                {
                    AnimateDown();
                }
                else
                {
                    AnimateUp();
                }

                if (shadow != null)
                {
                    shadow.transform.position = new Vector3(transform.position.x + 15, transform.position.y - 15, 0);
                }
            }
        }
    }

    void AnimateUp()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + bobMagnitude, 0);
        currentBob += bobMagnitude;

        if (currentBob >= bobRange)
        {
            isGoingDown = true;
        }
    }

    void AnimateDown()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - bobMagnitude, 0);
        currentBob -= bobMagnitude;

        if (currentBob <= 0)
        {
            isGoingDown = false;
        }
    }
}
