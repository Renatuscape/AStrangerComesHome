using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Manages the behaviour a spawned UI particle
public class UiParticleBehaviour : MonoBehaviour
{
    public RectTransform rect;
    public GameObject spriteContainer;
    public Image sprite;
    public UiParticleSettings settings;
    public float timePerFrame = 0.01f;
    float incrementX;
    float incrementY;

    public void Initialise(UiParticleSettings settings)
    {
        Debug.Log("UiParticleBehaviour: Initialising particle behaviour.");
        this.settings = settings;

        StartCoroutine(ParticleLife());
        StartCoroutine(ParticlePhysics());

        if (settings.fadeOut)
        {
            StartCoroutine(FadeOut());
        }
        if (settings.fadeIn)
        {
            StartCoroutine(FadeIn());
        }
        if (settings.rotationSpeed > 0)
        {
            StartCoroutine(Rotate());
        }
    }

    IEnumerator FadeOut()
    {
        int totalFrames = Mathf.FloorToInt(settings.particleLife / timePerFrame);

        float alphaFade = sprite.color.a / totalFrames;

        while (sprite.color.a > 0)
        {
            yield return new WaitForSeconds(timePerFrame);
            float newAlpha = Mathf.Max(0, sprite.color.a - alphaFade); // Ensure alpha doesn't go below 0
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
        }
    }

    IEnumerator FadeIn()
    {
        int totalFrames = Mathf.FloorToInt(settings.particleLife / timePerFrame);

        float alphaFade = sprite.color.a / totalFrames;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);

        while (sprite.color.a < 1)
        {
            yield return new WaitForSeconds(timePerFrame);
            float newAlpha = Mathf.Min(1, sprite.color.a + alphaFade);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
        }
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, 0, settings.rotationSpeed));
            yield return new WaitForSeconds(timePerFrame);
        }
    }

    IEnumerator ParticlePhysics()
    {
        incrementX = CalculateSpeed(settings.speedX, settings.randomDirectionX, settings.pingPongSpeedX, settings.adjustX, out var adjustX);
        incrementY = CalculateSpeed(settings.speedY, settings.randomDirectionY, settings.pingPongSpeedY, settings.adjustY, out var adjustY);

        while (true)
        {
            rect.transform.position = new Vector3(rect.transform.position.x + incrementX, rect.transform.position.y + incrementY);

            incrementX += adjustX;
            incrementY += adjustY;

            yield return new WaitForSeconds(timePerFrame);
        }
    }

    float CalculateSpeed(float speed, bool isRandomDirection, bool isPingPongSpeed, float incrementValue, out float incrementAdjustment)
    {
        float newSpeed = speed;
        incrementAdjustment = incrementValue;

        if (isRandomDirection && Random.Range(0, 100) > 50)
        {
            newSpeed = newSpeed * -1;
            incrementAdjustment = incrementAdjustment * -1;
        }

        if (isPingPongSpeed)
        {
            float minPingPong = settings.minPingPong;

            // Randomize ping pong direction if needed
            if (isRandomDirection && Random.Range(0, 100) > 50)
            {
                minPingPong = minPingPong * -1;
            }

            // Swap values if needed to ensure minPingPong is less than newSpeed
            if (minPingPong > newSpeed)
            {
                float temp = minPingPong;
                minPingPong = newSpeed;
                newSpeed = temp;
            }

            // Set new speed to a random value between minPingPong and newSpeed
            newSpeed = Random.Range(minPingPong, newSpeed);
        }

        return newSpeed;
    }

    IEnumerator ParticleLife()
    {
        yield return new WaitForSeconds(settings.particleLife);
        Debug.Log("UiParticleBehaviour: Particle life ended.");
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}

// Contains setting for a UI particle. Allows quick copy
[Serializable]
public class UiParticleSettings
{
    public float particleLife;
    public float speedX;
    public float speedY;

    public float adjustX;
    public float adjustY;

    public float rotationSpeed;
    public bool fadeOut;
    public bool fadeIn;

    public bool randomDirectionX;
    public bool randomDirectionY;

    public bool pingPongSpeedX;
    public bool pingPongSpeedY;
    public float minPingPong;
}
