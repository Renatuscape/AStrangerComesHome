using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// OBSOLETE. Replace with new type if found.
public class ParticleSpawner : MonoBehaviour
{
    public List<GameObject> spawnTargets;
    public List<SpriteRenderer> spawnedParticles;
    public GameObject particlePrefab;
    public GameObject particleContainer;
    public string particleName;
    public float spawnTime = 3;
    public float minSpawnTime = 0.2f;
    public bool pingPongSpawnTime;
    public bool affectedByPlayerSpeed;

    public int minParticles = 0;
    public int maxParticles = 3;
    public float spawnTimer = 0f;

    float randomParticles;
    public float calculatedSpawnTime;


    void FixedUpdate()
    {
        spawnTimer += Time.deltaTime;

        if (pingPongSpawnTime)
        {
            calculatedSpawnTime = Random.Range(minSpawnTime, spawnTimer);
        }
        else
        {
            calculatedSpawnTime = spawnTime;
        }

        if (affectedByPlayerSpeed)
        {
            calculatedSpawnTime = calculatedSpawnTime - TransientDataScript.transientData.currentSpeed;
        }

        if (calculatedSpawnTime < 0.2f) //LIMIT MAX SPAWN
            calculatedSpawnTime = 0.2f;

        if (spawnTimer >= calculatedSpawnTime)
        {
            spawnTimer = 0;
            SpawnTick();
        }
    }
    void SpawnTick()
    {
        if (particleContainer == null)
        {
            particleContainer = gameObject;
        }

        if (spawnTargets == null || spawnTargets.Count == 0)
        {
            SpawnParticle(gameObject);
        }
        else
        {
            foreach (GameObject spawnTarget in spawnTargets)
            {
                SpawnParticle(spawnTarget);
            }
        }
    }

    void SpawnParticle(GameObject spawnTarget)
    {
        randomParticles = Random.Range(minParticles * Time.timeScale, maxParticles * Time.timeScale); // Random.Range(minParticles, maxParticles);

        if (affectedByPlayerSpeed)
        {
            randomParticles = randomParticles * TransientDataScript.transientData.currentSpeed;
        }

        for (int i = 0; i < (randomParticles + 1); i++)
        {
            var prefab = Instantiate(particlePrefab);
            prefab.name = particleName;

            prefab.transform.SetParent(particleContainer.transform);

            prefab.transform.position = spawnTarget.transform.position;
        }
    }
}
