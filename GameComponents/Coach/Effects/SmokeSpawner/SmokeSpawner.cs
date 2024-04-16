using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeSpawner : MonoBehaviour
{

    public TransientDataScript transientData;

    public GameObject chimneySmokePrefab;
    public GameObject groundDustPrefab;
    public Transform chimneyTarget;
    public Transform groundTarget;
    public int minParticles = 0;
    public int maxParticles = 3;
    private float elapsed = 0f;

    public float randomNum;
    public float popTime;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    void FixedUpdate()
    {
        popTime = 3 - (transientData.currentSpeed);

        elapsed += Time.deltaTime;

        if (popTime < 0.2f) //LIMIT MAX SPAWN
            popTime = 0.2f;

        if (elapsed >= popTime)
        {
            elapsed = elapsed % popTime;
            OutputTime();
        }
    }
    void OutputTime()
    {
        randomNum = Random.Range(minParticles * Time.timeScale, maxParticles * Time.timeScale) * (transientData.currentSpeed); // Random.Range(minParticles, maxParticles);

        for (int i = 0; i < (randomNum + 1); i++)
        {
            Instantiate(chimneySmokePrefab, chimneyTarget.position, Quaternion.identity);
            chimneySmokePrefab.name = "Smoke";
        }

        if (groundTarget != null)
        {
            for (int i = 0; i < randomNum; i++)
            {
                Instantiate(groundDustPrefab, groundTarget.position, Quaternion.identity);
                groundDustPrefab.name = "Dust";
            }
        }
    }
}
