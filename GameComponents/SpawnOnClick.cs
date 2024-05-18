using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnClick : MonoBehaviour
{
    public TransientDataScript transientData;
    public List<string> particleIDs;
    public GameState ifGameStateA;
    public GameState ifGameStateB; //if the particles spawn in multiple game state. Otherwise use the same for both
    public int minParticles = 1;
    public int maxParticles = 4;
    private int randomNum;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    void OnMouseDown()
    {
        if (TransientDataScript.GameState == ifGameStateA || TransientDataScript.GameState == ifGameStateB)
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
