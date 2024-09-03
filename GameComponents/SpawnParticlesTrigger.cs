using System.Collections.Generic;
using UnityEngine;

public class SpawnParticlesTrigger : MonoBehaviour
{
    public TransientDataScript transientData;
    public List<string> particleIDs;
    public bool doNotSpawnOnClick;
    public int minParticles = 1;
    public int maxParticles = 4;
    private int randomNum;


    void OnMouseDown()
    {
        if (!doNotSpawnOnClick)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            randomNum = Random.Range(minParticles, maxParticles);

            for (int i = 0; i < randomNum; i++)
            {
                foreach (string id in particleIDs)
                {
                    ParticleFactory.GetParticle(gameObject, id, gameObject.name);
                }
            }
        }
    }
}