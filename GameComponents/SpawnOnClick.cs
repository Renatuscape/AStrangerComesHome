using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnClick : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject particlePrefab;
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
        if (transientData.gameState == ifGameStateA || transientData.gameState == ifGameStateB)
        {
            randomNum = Random.Range(minParticles, maxParticles);

            for (int i = 0; i < randomNum; i++)
            {
                Instantiate(particlePrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                particlePrefab.name = "Particle" + name;
            }
        }
    }
}
