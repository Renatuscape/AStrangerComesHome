using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxThis : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject spriteObject;
    public Vector3 spriteSize;

    public float parallaxMultiplier;
    public float passiveSpeed = 0;

    private float parallaxEffect;

    float tick = 0.005f;
    float timer;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();

        if (spriteObject == null)
        {
            spriteObject = gameObject;
        }

        spriteSize = spriteObject.GetComponent<SpriteRenderer>().bounds.size;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tick)
        {
            Tick();
            timer = 0;
        }
    }

    void Tick()
    {
        parallaxEffect = transientData.currentSpeed * parallaxMultiplier;

        transform.position = new Vector2((transform.position.x + passiveSpeed + parallaxEffect), transform.position.y);

        if (transform.position.x <= (0 - spriteSize.x) || transform.position.x >= spriteSize.x)
        {
            transform.position = new Vector2(0, transform.position.y);
        }
    }
}
