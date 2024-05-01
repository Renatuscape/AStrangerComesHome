using Unity.VisualScripting;
using UnityEngine;

public class ParticlePhysics : MonoBehaviour
{
    public ParticleData settings;
    public SpriteRenderer rend;
    public float currentParticleLife;
    public bool isActivated = false;
    public float scatterForce = 0;
    public float updateFrequency = 0.01f;
    public float updateTimer = 0;

    float magnitudeAdjustment = 0.2f;
    float scaleMin = 0.5f;
    float scaleMax = 2f;
    public void Initiate(ParticleData particleData, string customSortingLayer)
    {
        if (rend == null)
        {
            rend = GetComponent<SpriteRenderer>();
        }

        if (customSortingLayer != null )
        {
            rend.sortingLayerName = customSortingLayer;
        }

        settings = new()
        {
            particleLife = particleData.particleLife,
            verticalAcceleration = particleData.verticalAcceleration,
            horizontalAcceleration = particleData.horizontalAcceleration * magnitudeAdjustment,
            gravity = particleData.gravity * magnitudeAdjustment,
            adjustForCoachSpeed = particleData.adjustForCoachSpeed,
            isSpinning = particleData.isSpinning,
            isSpinningLeft = particleData.isSpinningLeft,
            isGrowing = particleData.isGrowing,
            isShrinking = particleData.isShrinking,
            isFadeDisabled = particleData.isFadeDisabled
        };

        if (particleData.randomDrag != 0 )
        {
            settings.velocity = Random.Range(particleData.velocity - particleData.randomDrag, particleData.velocity);
        }
        else
        {
            settings.velocity = particleData.velocity;
        }

        currentParticleLife = settings.particleLife;

        if (settings.isGrowing)
        {
            scaleMin = Random.Range(0.8f, 1f);
            scaleMax = Random.Range(1.5f, 2f);
        }

        if (settings.scatterRange != 0)
        {
            scatterForce = Random.Range(particleData.scatterRange * -1, particleData.scatterRange) * magnitudeAdjustment;

        }

        isActivated = true;
    }

    void Update()
    {
        if (TransientDataScript.IsTimeFlowing() && isActivated)
        {
            // VERTICAL MOVEMENT
            float verticalForce = settings.velocity;
            settings.velocity -= settings.gravity; // reduce applied force over time

            // HORIZONTAL MOVEMENT
            float horizontalForce = scatterForce;

            if (scatterForce != 0)
            {
                if (scatterForce > 0)
                {
                    scatterForce -= settings.horizontalAcceleration * 0.05f;
                }
                else
                {
                    scatterForce += settings.horizontalAcceleration * 0.05f;
                }
            }

            if (settings.adjustForCoachSpeed)
            {
                horizontalForce += TransientDataScript.transientData.currentSpeed * 0.9f;
            }

            horizontalForce = horizontalForce * Time.deltaTime;
            verticalForce = verticalForce * Time.deltaTime;

            currentParticleLife -= Time.deltaTime;

            // SET POSITION
            transform.position = new Vector3(transform.position.x + horizontalForce, transform.position.y + verticalForce, 0);


            // ADJUST APPEARANCE
            if (!settings.isFadeDisabled || settings.isGrowing)
            {
                float lifeRatio = currentParticleLife / settings.particleLife;

                if (!settings.isFadeDisabled)
                {
                    float alpha = Mathf.Lerp(0, 1, lifeRatio);
                    rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
                }

                if (settings.isGrowing)
                {
                    float scale = Mathf.Lerp(scaleMax, scaleMin, lifeRatio);
                    transform.localScale = new Vector3(scale, scale, 1);
                }
            }

            // CHECK FOR DEATH
            if (currentParticleLife <= 0)
            {
                Destroy(gameObject); // Destroy the particle object
            }
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
