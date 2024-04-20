using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFactory : MonoBehaviour
{
    public static ParticleFactory instance;
    public List<Sprite> particleSprites = new();
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameObject Spawn()
    {
        GameObject particle = new();

        return particle;
    }
}
