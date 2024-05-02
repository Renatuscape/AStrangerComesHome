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
    void Start()
    {

        if (settings.randomDrag != 0 )
        {
            settings.velocity = Random.Range(settings.velocity - settings.randomDrag, settings.velocity);
        }

        currentParticleLife = settings.particleLife;

        if (settings.isGrowing)
        {
            if (settings.minScale == 0)
            {
                settings.minScale = Random.Range(0.5f, 1f); ;
            }
            if (settings.maxScale == 0)
            {
                settings.maxScale = Random.Range(1.5f, 2.5f);
            }

            rend.transform.localScale = new Vector3(settings.minScale, settings.minScale, 1);
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
            if (!settings.disableWithoutSpeed || settings.disableWithoutSpeed && TransientDataScript.transientData.currentSpeed != 0)
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
                        float scale = Mathf.Lerp(settings.maxScale, settings.minScale, lifeRatio);
                        transform.localScale = new Vector3(scale, scale, 1);
                    }
                }

                // CHECK FOR DEATH
                if (currentParticleLife <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
