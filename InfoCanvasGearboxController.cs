using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoCanvasGearboxController : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject gearStick;
    public TextMeshProUGUI smallGearboxText;
    public GameObject targetPosition;
    public float stickPosition;

    public GameObject targetR;
    public GameObject targetOff;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            PositionCheck();
        }   
    }
    void PositionCheck()
    {
        if (GlobalSettings.uiGearboxLarge)
        {
            if (transientData.engineState == EngineState.Reverse)
            {
                targetPosition = targetR;
            }
            else if (transientData.engineState == EngineState.Off)
            {
                targetPosition = targetOff;
            }
            else if (transientData.engineState == EngineState.FirstGear)
            {
                targetPosition = target1;
            }
            else if (transientData.engineState == EngineState.SecondGear)
            {
                targetPosition = target2;
            }
            else if (transientData.engineState == EngineState.ThirdGear)
            {
                targetPosition = target3;
            }

            while (stickPosition != targetPosition.transform.position.x)
            {
               StartCoroutine(UpdateStickPosition(targetPosition.transform.position));
               stickPosition = targetPosition.transform.position.x;
            }
        }
        else
        {
            if (transientData.engineState == EngineState.Reverse)
            {
                smallGearboxText.text = "R";
            }
            else if (transientData.engineState == EngineState.Off)
            {
                smallGearboxText.text = "OFF";
            }
            else if (transientData.engineState == EngineState.FirstGear)
            {
                smallGearboxText.text = "1";
            }
            else if (transientData.engineState == EngineState.SecondGear)
            {
                smallGearboxText.text = "2";
            }
            else if (transientData.engineState == EngineState.ThirdGear)
            {
                smallGearboxText.text = "3";
            }
        }
    }

    IEnumerator UpdateStickPosition(Vector3 newPos)
    {
        float adjustment = 0.5f;
        while (gearStick.transform.position.x > newPos.x + 0.3f)
        {
            gearStick.transform.position = new Vector3(gearStick.transform.position.x - adjustment, gearStick.transform.position.y, gearStick.transform.position.z);
            yield return new WaitForSeconds(0.001f);
        }
        while (gearStick.transform.position.x < newPos.x + 0.3f)
        {
            gearStick.transform.position = new Vector3(gearStick.transform.position.x + adjustment, gearStick.transform.position.y, gearStick.transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        AudioManager.PlayUISound("metalClick");
    }
}
