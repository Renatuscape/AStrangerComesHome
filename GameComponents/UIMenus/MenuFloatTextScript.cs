using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuFloatTextScript : MonoBehaviour
{
    public TransientDataScript transientData;
    public TextMeshProUGUI floatText;
    public Image backPanel;
    bool disable;
    float disableTimer;
    float disableTick;

    void Awake()
    {
        Disable();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    // Update is called once per frame
    private void OnEnable()
    {
        DisableFloatText();
    }
    void Update()
    {
        transform.position = Input.mousePosition; //Only works for ScreenSpaceOverlay type of Canvas RenderMode
        if (Input.GetMouseButtonDown(0))
        {
            DisableFloatText();
        }

        if (disable)
        {
            disableTimer += Time.deltaTime;

            if (disableTimer > disableTick)
            {
                Disable();
            }
        }
    }

    public void PrintFloatText(string content)
    {
        disableTick = 5;
        disableTimer = 0;
        disable = true;

        Canvas.ForceUpdateCanvases();
        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;

        backPanel.enabled = true;
        floatText.text = content;

        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void DisableFloatText()
    {
        disableTimer = 0;
        disableTick = 0.2f;
        disable = true;
    }

    void Disable()
    {
        backPanel.enabled = false;
        floatText.text = " ";
        disable = false;
    }
}
