using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithSpeed : MonoBehaviour
{
    public TransientDataScript transientData;
    private float carriageSpeed;

    public bool reverseRotationDirection = false;
    public float baseRotationSpeed = 0f;
    public float rotationSpeedMultiplier = 5;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    void FixedUpdate()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
        carriageSpeed = transientData.currentSpeed;

        if (!reverseRotationDirection)
        {
            transform.Rotate(new Vector3(0, 0, (baseRotationSpeed + carriageSpeed * rotationSpeedMultiplier)) * Time.timeScale);
        }
        else if (reverseRotationDirection)
        {
            transform.Rotate(new Vector3(0, 0, ((0 - baseRotationSpeed) - (carriageSpeed * rotationSpeedMultiplier))) * Time.timeScale);
        }
        }

    }
}
