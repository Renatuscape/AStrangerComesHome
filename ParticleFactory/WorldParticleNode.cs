using System.Collections.Generic;
using UnityEngine;

public class WorldParticleNode : MonoBehaviour
{
    public List<string> particles = new();
    public string customSortingLayer;

    public bool adjustSpawnWithSpeed = false;
    public float maxTick;
    public float minTick;
    public int minParticles;
    public int maxParticles;
    public float tick;
    public float spawnTimer;
    private void Start()
    {
        tick = Random.Range(minTick, maxTick);
    }

    private void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            spawnTimer += Time.deltaTime;

            if (adjustSpawnWithSpeed)
            {
                spawnTimer += TransientDataScript.transientData.currentSpeed * 0.008f;
            }

            if (spawnTimer > tick)
            {
                SpawnEachParticleType(gameObject);
                spawnTimer = 0;
                tick = Random.Range(minTick, maxTick);
            }
        }
    }

    void SpawnEachParticleType(GameObject container)
    {
        foreach (string particle in particles)
        {
            int toSpawn = Random.Range(minParticles, maxParticles + 1);
            // Debug.Log("Creating new particle of type " + particle + ". Attempting to spawn " + toSpawn);
            int spawned = 0;

            while (spawned < toSpawn)
            {
                var newParticle = ParticleFactory.SpawnWorldParticle(particle);

                if (newParticle != null)
                {
                    newParticle.transform.SetParent(container.transform, false);
                }
                spawned++;
            }
        }
    }
}