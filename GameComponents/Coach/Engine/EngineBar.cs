using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineBar : MonoBehaviour
{
    public Engine engine;

    public GameObject barSprite;
    public Transform barMask;
    public Transform barEmptyTarget;

    public Vector3 barPositionFull;
    public Vector3 barPositionEmpty;
    public float boostFraction;

    void Awake()
    {
        engine = GameObject.Find("Engine").GetComponent<Engine>();

        barSprite = GameObject.Find("EngineBarSprite");
        barMask = GameObject.Find("EngineBarMask").GetComponent<Transform>();
        barEmptyTarget = GameObject.Find("EngineBarTarget").GetComponent<Transform>();

        barPositionEmpty = barEmptyTarget.transform.localPosition;
        barPositionFull = barSprite.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (engine.isReady)
        {
            boostFraction = engine.currentBoost * 100 / engine.boostMax / 100;

            barMask.localPosition = Vector3.Lerp(barPositionEmpty, barPositionFull, boostFraction);
        }
    }
}
