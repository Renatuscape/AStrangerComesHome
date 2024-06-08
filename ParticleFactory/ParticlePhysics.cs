using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticlePhysics : MonoBehaviour
{
    public ParticleData settings;
    public SpriteRenderer rend;
    public float currentParticleLife;
    public bool isActivated = false;
    public float velocity = 0;
    public float scatterForce = 0;
    public float updateFrequency = 0.01f;
    public float updateTimer = 0;

    float magnitudeAdjustment = 0.2f;

    private void Start()
    {
        if (settings.randomDrag != 0)
        {
            velocity = Random.Range(settings.velocity - settings.randomDrag, settings.velocity);
        }
        else
        {
            velocity = settings.velocity;
        }

        currentParticleLife = settings.particleLife;

        if (settings.isGrowing)
        {
            if (settings.minScale == 0)
            {
                settings.minScale = 0.5f;
            }

            if (settings.maxScale == 0)
            {
                settings.maxScale = 1.5f;
            }

            if (settings.randomiseScale)
            {
                settings.minScale = Random.Range(settings.minScale, settings.minScale + 0.5f);
                settings.maxScale = Random.Range(settings.maxScale, settings.minScale + 1f);
            }

            rend.transform.localScale = new Vector3(settings.minScale, settings.minScale, 1);
        }
        else if (settings.isShrinking)
        {
            if (settings.minScale < settings.maxScale)
            {
                var minScale = settings.minScale;
                var maxScale = settings.maxScale;

                settings.minScale = maxScale;
                settings.maxScale = minScale;
            }

            if (settings.minScale == 0)
            {
                settings.minScale = 1.5f;
            }

            if (settings.maxScale == 0)
            {
                settings.maxScale = 0.5f;
            }

            if (settings.randomiseScale)
            {
                settings.minScale = Random.Range(settings.minScale, settings.minScale + 1f);
                settings.maxScale = Random.Range(settings.maxScale, settings.maxScale + 0.5f);
            }

            rend.transform.localScale = new Vector3(settings.minScale, settings.minScale, 1);
        }
        else if (!settings.isGrowing && !settings.isShrinking)
        {
            settings.minScale = rend.transform.localScale.x;
            settings.maxScale = rend.transform.localScale.y;
        }

        if (settings.scatterRange != 0)
        {
            scatterForce = Random.Range(settings.scatterRange * -1, settings.scatterRange) * magnitudeAdjustment;

        }
        if (settings.adjustForCoachSpeed && settings.coachSpeedMultiplier == 0)
        {
            settings.coachSpeedMultiplier = 0.9f;
        }
    }
    public void PlayBehaviour(GameObject parentObject)
    {
        transform.SetParent(parentObject.transform);
        transform.localPosition = new Vector3(0, 0, 0);
        rend.transform.localScale = new Vector3(settings.minScale, settings.minScale, 1);
        rend.color = new Color(1, 1, 1, 1);

        currentParticleLife = settings.particleLife;

        if (settings.randomDrag != 0)
        {
            velocity = Random.Range(settings.velocity - settings.randomDrag, settings.velocity);
        }
        else
        {
            velocity = settings.velocity;
        }

        if (settings.scatterRange != 0)
        {
            scatterForce = Random.Range(settings.scatterRange * -1, settings.scatterRange) * magnitudeAdjustment;

        }

        isActivated = true;
    }

    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            // VERTICAL MOVEMENT
            float verticalForce = velocity;
            velocity -= settings.gravity; // reduce applied force over time

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
                horizontalForce += TransientDataScript.transientData.currentSpeed * settings.coachSpeedMultiplier;
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
                    float scale = Mathf.Lerp(settings.maxScale, settings.minScale, lifeRatio);
                    transform.localScale = new Vector3(scale, scale, 1);
                }
            }

            // CHECK FOR DEATH
            if (currentParticleLife <= 0)
            {
                Return();
            }
        }
    }

    public void Return()
    {
        gameObject.transform.SetParent(ParticleFactory.instance.gameObject.transform);
        gameObject.SetActive(false);
        ParticleFactory.ReturnParticle(gameObject);
    }
}
