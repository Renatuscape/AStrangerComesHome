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
                    var particle = pool.objectInstances[0];
                    pool.objectInstances.Remove(particle);
                    particle.SetActive(true);
                    particle.GetComponent<ParticlePhysics>().PlayBehaviour(parentObject);
                    return particle;
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
}
