using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_BobLoop : MonoBehaviour
{
    public GameObject shadow;
    public int positionIndex;
    public float startPosition;
    public float bobTarget = 6;
    public float bobMagnitude = 1f;
    public float tickMultiplier = 1f;
    public bool isGoingDown = false;
    public float timer = 0;

    void Start()
    {
        startPosition = transform.localPosition.y;
        bobTarget = startPosition + bobTarget;
        transform.localPosition = new Vector3(transform.localPosition.x, startPosition, 0);
    }
    void Update()
    {
        timer += tickMultiplier * Time.deltaTime;

        if (timer >= 0.12f)
        {
            timer = 0;
            Animate();
        }
    }

    void Animate()
    {
        if (isGoingDown && transform.localPosition.y > startPosition)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - bobMagnitude, 0);
        }
        else if (!isGoingDown && transform.localPosition.y < bobTarget)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + bobMagnitude, 0);
        }
        if (transform.localPosition.y >= bobTarget)
        {
            isGoingDown = true;
        }
        else if (transform.localPosition.y <= startPosition)
        {
            isGoingDown = false;
        }

        if (shadow != null)
        {
            shadow.transform.position = new Vector3(transform.position.x + 15, transform.position.y - 15, 0);
        }

    }
}
