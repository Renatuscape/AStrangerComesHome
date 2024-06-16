using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleFactory : MonoBehaviour
{
    public static ParticleFactory instance;
    public List<GameObject> particlePrefabs;
    public List<ObjectPool> objectPools = new();
    bool isReady = false;
    void Start()
    {
        instance = this;

        foreach (GameObject obj in particlePrefabs)
        {
            ObjectPool pool = new();
            pool.accessID = obj.name.ToLower();
            pool.objectInstances = new();

            for (int i = 0; i < 100; i++)
            {
                GameObject particle = Instantiate(obj);
                particle.transform.SetParent(gameObject.transform);
                particle.name = pool.accessID + " " + i;
                particle.SetActive(false);
                pool.objectInstances.Add(particle);
            }

            objectPools.Add(pool);
        }

        isReady = true;
    }

    public void ReturnParticleToPool(GameObject particle)
    {
        if (isReady)
        {
            var pool = objectPools.FirstOrDefault(p => particle.name.Contains(p.accessID));
            pool.objectInstances.Add(particle);
            if (particle.activeInHierarchy)
            {
                particle.gameObject.SetActive(false);
            }
        }
    }

    public GameObject GetWorldParticle(GameObject parentObject, string particleID, string caller)
    {
        if (isReady)
        {
            var pool = objectPools.FirstOrDefault(p => p.accessID.Contains(particleID.ToLower()));
            GameObject parent;

            if (parentObject != null)
            {
                parent = parentObject;
            }
            else
            {
                parent = gameObject;
            }

            if (pool != null)
            {
                if (pool.objectInstances == null || pool.objectInstances.Count == 0)
                {
                    Debug.LogWarning(pool.accessID + " had no prefabs of type " + particleID + ". Returning null.");
                    return null;
                }
                else
                {
                    if (pool.objectInstances.Count > 0)
                    {
                        var particle = pool.objectInstances[0];

                        if (particle != null)
                        {

                            pool.objectInstances.Remove(particle);
                            particle.SetActive(true);
                            particle.GetComponent<ParticlePhysics>().PlayBehaviour(parentObject);
                            return particle;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Pool was empty: " + pool.accessID);
                        return null;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No pool found with accessID " + particleID);
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public static GameObject GetParticle(GameObject parentObject, string particleID, string caller)
    {
        return instance.GetWorldParticle(parentObject, particleID, caller);
    }

    public static void ReturnParticle(GameObject particle)
    {
        instance.ReturnParticleToPool(particle);
    }


    // READY-MADE EFFECTS
    public static void ClickEffectWeeds(GameObject parentObject, bool playAudio = true, int min = 3, int max = 6, bool randomCount = true)
    {

        if (playAudio)
        {
            AudioManager.PlaySoundEffect("cloth3", 0.05f);
        }

        ClickEffect("WeedsBurst", parentObject, min, max, randomCount);
    }

    public static void ClickEffectCoins(GameObject parentObject, bool playAudio = true, int min = 3, int max = 6, bool randomCount = true)
    {
        if (playAudio)
        {
            AudioManager.PlaySoundEffect("handleCoins");
        }

        ClickEffect("CoinBurst", parentObject, min, max, randomCount);
    }

    static void ClickEffect(string particleID, GameObject parentObject, int min = 3, int max = 6, bool randomCount = true)
    {
        Debug.Log("Attemtping to spawn particles using ClickEffect with ID " + particleID);

        List<GameObject> spawnedParticles = new();

        int spawnCount = randomCount ? Random.Range(min, max + 1) : min;

        while (spawnCount > 0)
        {
            spawnCount--;
            spawnedParticles.Add(GetParticle(parentObject, particleID, "ClickEffect"));
        }
    }

    //void ClickEffectPhysics(List<GameObject> particles)
    //{

    //}
}
