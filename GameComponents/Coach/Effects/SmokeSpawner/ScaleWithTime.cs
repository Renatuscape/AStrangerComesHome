using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithTime : MonoBehaviour
{
    public TransientDataScript transientData;

    private float pingPongScale;
    private float elapsed = 0f;

    public float scaleFrequency;
    public float scaleMultiplier;
    public bool pingPongStart;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    void Start()
    {
        pingPongScale = Random.Range(0.5f, 1.5f);

        if (pingPongStart == true)
        {
            this.transform.localScale = new Vector2(pingPongScale, pingPongScale);
        }
    }

    void FixedUpdate()
    {

        scaleFrequency = 0.8f;
        pingPongScale = Random.Range(0.5f * scaleMultiplier, 1.5f * scaleMultiplier);

        elapsed += Time.deltaTime;
        if (elapsed >= scaleFrequency)
        {
            elapsed = elapsed % scaleFrequency;
            OutputTime();
        }
    }
    void OutputTime()
    {
        this.transform.localScale = new Vector2(this.transform.localScale.x + pingPongScale, this.transform.localScale.y + pingPongScale);

    }
}
