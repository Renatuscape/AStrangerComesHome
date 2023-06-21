using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public TransientDataScript transientData;

    public GameObject barSprite;
    public Transform barMask;
    public Transform barEmptyTarget;

    public Vector3 barPositionFull;
    public Vector3 barPositionEmpty;
    public float manaFraction;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();

        barSprite = GameObject.Find("ManaBarSprite");
        barMask = GameObject.Find("ManaBarMask").GetComponent<Transform>();
        barEmptyTarget = GameObject.Find("ManaBarTarget").GetComponent<Transform>();

        barPositionEmpty = barEmptyTarget.transform.localPosition;
        barPositionFull = barSprite.transform.localPosition;
    }

    void Update()
    {
        manaFraction = transientData.currentMana * 100 / transientData.manapool / 100;

        barMask.localPosition = Vector3.Lerp(barPositionEmpty, barPositionFull, manaFraction);
    }
}
