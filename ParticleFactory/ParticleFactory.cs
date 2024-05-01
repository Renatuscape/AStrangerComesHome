using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleFactory : MonoBehaviour
{
    public static ParticleFactory instance;
    public List<Sprite> particleSprites = new();
    public GameObject particlePrefab;
    public List<ParticleData> defaultBehaviour = new()
    {
        new()
        {
            particleID = "smoke",
            particleLife = 4f,
            velocity = 0.8f,
            randomDrag = 0.6f,
            verticalAcceleration = 0.7f,
            horizontalAcceleration = 0f,
            gravity = 0f,
            scatterRange = 2f,
            adjustForCoachSpeed = true,
            isGrowing = true,
        }
};

    void Start()
    {
        instance = this;
    }

    public GameObject CreateWorldParticle(ParticleData particleData)
    {
        GameObject particle = new();

        SpriteRenderer rend = particle.AddComponent<SpriteRenderer>();
        rend.sprite = particleSprites.FirstOrDefault(p => p.name.ToLower().Contains(particleData.particleID.ToLower()));

        if (rend.sprite != null)
        {
            ParticlePhysics pSimulator = particle.AddComponent<ParticlePhysics>();

            rend.sortingLayerName = "WaitingPassengers";
            pSimulator.Initiate(particleData, "");

            return particle;
        }
        else
        {
            Debug.LogWarning("Sprite Factory could not find particle with ID " + particleData.particleID);
            Destroy(particle);
            return null;
        }
    }

    public GameObject GetWorldParticle(string particleID, string customSortingLayer)
    {
        var behaviour = defaultBehaviour.FirstOrDefault(p => p.particleID.ToLower().Contains(particleID.ToLower()));
        GameObject particle = Instantiate(particlePrefab);
        particle.name = particleID;
        var script = particle.GetComponent<ParticlePhysics>();

        script.Initiate(behaviour, customSortingLayer);

        return particle;
    }

    public static GameObject SpawnWorldParticle(ParticleData particleData)
    {
        if (instance != null)
        {
            return instance.CreateWorldParticle(particleData);
        }
        else
        {
            Debug.LogWarning("Particle Factory instance was null.");
            return null;
        }
    }

    public static GameObject SpawnWorldParticle(string particleID, string customSortingLayer = "")
    {
        return instance.GetWorldParticle(particleID, customSortingLayer);
    }
}
