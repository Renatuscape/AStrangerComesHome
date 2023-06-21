using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineKnob : MonoBehaviour
{
    public TransientDataScript transientData;

    public Transform posFirstGear;
    public Transform posSecondGear;
    public Transform posThirdGear;
    public Transform knobSprite;
    public Transform targetPosition;

    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();

        posFirstGear = GameObject.Find("KnobTarget1").GetComponent<Transform>();
        posSecondGear = GameObject.Find("KnobTarget2").GetComponent<Transform>();
        posThirdGear = GameObject.Find("KnobTarget3").GetComponent<Transform>();
        knobSprite = GameObject.Find("KnobSprite").GetComponent<Transform>();
        targetPosition = posFirstGear;
    }

    void FixedUpdate()
    {
        if (knobSprite.transform.localPosition.y < targetPosition.localPosition.y - 0.01)
            increaseY();
        else if (knobSprite.transform.localPosition.y > targetPosition.localPosition.y + 0.01f)
            decreaseY();

        if (transientData.engineState != EngineState.Off)
        {
            if (transientData.engineState == EngineState.FirstGear)
                targetPosition = posFirstGear;
            else if (transientData.engineState == EngineState.SecondGear)
                targetPosition = posSecondGear;
            else if (transientData.engineState == EngineState.ThirdGear)
                targetPosition = posThirdGear;
        }

        else if (transientData.engineState == EngineState.Off || transientData.engineState == EngineState.Reverse)
        {
            targetPosition = posFirstGear;
        }
    }
    void increaseY()
    {
        knobSprite.transform.localPosition = new Vector3(knobSprite.transform.localPosition.x, knobSprite.transform.localPosition.y + 0.005f, knobSprite.transform.localPosition.z);
    }
    void decreaseY()
    {
        knobSprite.transform.localPosition = new Vector3(knobSprite.transform.localPosition.x, knobSprite.transform.localPosition.y - 0.01f, knobSprite.transform.localPosition.z);
    }
}
