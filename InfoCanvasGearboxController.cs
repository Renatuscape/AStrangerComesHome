using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvasGearboxController : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject gearStick;
    public TextMeshProUGUI smallGearboxText;
    public GameObject targetPosition;

    public GameObject targetR;
    public GameObject targetOff;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;

    public Button btnGear1;
    public Button btnGear2;
    public Button btnGear3;
    public Button btnOff;
    public Button btnGearR;

    bool runningCoroutine = false;
    void Start()
    {
        btnGear1.onClick.AddListener(() => transientData.engineState = EngineState.FirstGear);
        btnGear2.onClick.AddListener(() => transientData.engineState = EngineState.SecondGear);
        btnGear3.onClick.AddListener(() => transientData.engineState = EngineState.ThirdGear);
        btnGearR.onClick.AddListener(() => transientData.engineState = EngineState.Reverse);
        btnOff.onClick.AddListener(() => transientData.engineState = EngineState.Off);
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

            if (!runningCoroutine && (gearStick.transform.localPosition.x > targetPosition.transform.localPosition.x + 0.2f || gearStick.transform.localPosition.x < targetPosition.transform.localPosition.x - 0.2f))
            {
                runningCoroutine = true;
                StartCoroutine(UpdateStickPosition(targetPosition.transform.localPosition));
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
        float duration = 0.2f; // Time taken to reach the target position
        float elapsedTime = 0f;
        Vector3 initialPos = gearStick.transform.localPosition;

        while (elapsedTime < duration)
        {
            gearStick.transform.localPosition = Vector3.Lerp(initialPos, newPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the gear stick reaches exactly the target position
        gearStick.transform.localPosition = newPos;

        AudioManager.PlayUISound("metalClick");
        runningCoroutine = false;
    }
}
