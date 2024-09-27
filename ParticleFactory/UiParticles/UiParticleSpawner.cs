using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiParticleSpawner : MonoBehaviour
{
    public List<GameObject> spawningNodes;
    public GameObject particlePrefab;
    public UiParticleSettings particleSettings;

    public bool isTriggerOnly;
    public float autoTriggerInterval;
    public bool ready;

    private void OnEnable()
    {
        if (!ready)
        {
            CheckValues();
        }

        if (!isTriggerOnly)
        {
            Debug.Log("UiParticle: Starting spawn coroutine.");
            StartCoroutine(SpawnRoutine());
        }
    }

    void CheckValues()
    {
        Debug.Log("UiParticle: Checking values for spawner.");
        if (autoTriggerInterval <= 0)
        {
            autoTriggerInterval = 1;
        }

        if (particleSettings.minPingPong == 0)
        {
            particleSettings.minPingPong = 0.1f;
        }

        if (spawningNodes == null || spawningNodes.Count == 0)
        {
            spawningNodes = new() { gameObject };
        }

        if (particleSettings.particleLife <= 0)
        {
            particleSettings.particleLife = 3;
        }

        if (particleSettings.speedX == 0 && particleSettings.speedY == 0)
        {
            particleSettings.speedY = 3;
        }

        if (particleSettings.fadeIn && particleSettings.fadeOut)
        {
            particleSettings.fadeIn = false;
        }

        ready = true;
    }

    IEnumerator SpawnRoutine()
    {
        Debug.Log("UiParticle: SpawnRoutine attempting to spawn.");
        while (true)
        {
            yield return new WaitForSeconds(autoTriggerInterval);
            SpawnParticle();
        }
    }

    public void TriggerParticleSpawn()
    {
        if (!ready)
        {
            CheckValues();
        }

        SpawnParticle();
    }
    void SpawnParticle()
    {
        Debug.Log("UiParticle: SpawnParticle called.");

        foreach (GameObject node in spawningNodes)
        {
            var particle = CreateParticle();

            particle.transform.SetParent(node.transform, false);
        }
    }

    GameObject CreateParticle()
    {
        Debug.Log("UiParticle: Attempting to create particle.");

        var newParticle = Instantiate(particlePrefab);
        var script = newParticle.GetComponent<UiParticleBehaviour>();

        if (script != null)
        {
            script.Initialise(particleSettings);
        }
        else
        {
            Debug.Log("UiParticle: Ui Particle Behaviour could not be found.");
        }

        return newParticle;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
