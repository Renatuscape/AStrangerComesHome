using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    private SpriteRenderer sprite; //essential for alpha fade
    private Rigidbody2D rigbod; //for gravity controls

    public TransientDataScript transientData;

    //MOVEMENT AND FADE
    private float randomX;
    private float randomY;
    private float carriageSpeed;
    private float alphaSetting = 1;

    //PARTICLE ADJUSTMENTS
    public float carriageSpeedMultiplier = 0.3f;
    public float particleLife = 1;
    public float particleGravity = 0.1f;
    public float spreadMultiplier = 1;
    public float riseMultiplier = 1;

    void Start()
    {
        StartCoroutine(SelfDestruct());
        StartCoroutine(AlphaFade());

        sprite = GetComponent<SpriteRenderer>();
        rigbod = GetComponent<Rigidbody2D>();

        randomX = Random.Range(spreadMultiplier * -0.01f, spreadMultiplier * 0.01f);
        randomY = Random.Range(riseMultiplier * 0.006f, riseMultiplier * 0.018f);

        rigbod.gravityScale = particleGravity;
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void FixedUpdate()
    {
        carriageSpeed = transientData.currentSpeed / 500;
        sprite.color = new Color(1, 1, 1, alphaSetting);

        if (Time.timeScale == 1) // !uiController.pauseMenuEnabled)
        {
            transform.position = new Vector2(transform.position.x + randomX + (carriageSpeedMultiplier * carriageSpeed), transform.position.y + randomY);
        }
        else if (Time.timeScale == 0) //(uiController.pauseMenuEnabled)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y);
        }

        if (transform.position.y > 8) //destroy if particle floats above game window
            Destroy(gameObject);
    }

    IEnumerator AlphaFade()
    {
        yield return new WaitForSeconds(particleLife * 0.1f);
        alphaSetting = alphaSetting - (0.1f);
        StartCoroutine(AlphaFade());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(particleLife * 1f);
        Destroy(gameObject);
    }
}
