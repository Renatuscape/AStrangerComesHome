using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleFactory : MonoBehaviour
{
    public static ParticleFactory instance;
    public List<GameObject> particlePrefabs;

    void Start()
    {
        instance = this;

    //    defaultBehaviour = new()
    //{
    //    new()
    //    {
    //        particleID = "smoke",
    //        particleLife = 4f,
    //        velocity = 0.8f,
    //        randomDrag = 0.6f,
    //        verticalAcceleration = 0.7f,
    //        horizontalAcceleration = 0f,
    //        gravity = 0f,
    //        scatterRange = 2f,
    //        adjustForCoachSpeed = true,
    //        isGrowing = true,
    //    }
//};
    }

    public GameObject GetWorldParticle(string particleID)
    {
        var prefab = particlePrefabs.FirstOrDefault(p => p.name.ToLower().Contains(particleID));
        return Instantiate(prefab);
    }

    public static GameObject SpawnWorldParticle(string particleID)
    {
        // Debug.Log("Attempting to spawn " + particleID);
        return instance.GetWorldParticle(particleID);
    }
}
