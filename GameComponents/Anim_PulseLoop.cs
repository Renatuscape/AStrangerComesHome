using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_PulseLoop : MonoBehaviour
{
    public float pulseFrequency = 0.05f;
    public float pulseFactor = 0.004f;
    public float pulseMinimum = 1f;
    public float pulseMaximum = 1.1f;
    public float timer;

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= pulseFrequency)
            Pulse();
    }

    void Pulse()
    {
        if (transform.localScale.x < pulseMaximum)
        transform.localScale = new Vector3(transform.localScale.x + pulseFactor, transform.localScale.y + pulseFactor, 1);

        else if (transform.localScale.x > pulseMinimum)
        {
            transform.localScale = new Vector3(pulseMinimum, pulseMinimum, 1);
            timer = 0;
        }
    }
}
