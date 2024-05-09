using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuFloatTextScript : MonoBehaviour
{
    public TransientDataScript transientData;
    public TextMeshProUGUI floatText;
    public RectTransform rect;
    public Image backPanel;
    bool disable;
    float disableTimer;
    float disableTick;
    public float mouseWorldPosition;

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
        mouseWorldPosition = MouseTracker.GetMouseWorldPosition().x;
        if (mouseWorldPosition > 12)
        {
            rect.anchorMin = new Vector2(1, rect.anchorMin.y);
            rect.anchorMax = new Vector2(1, rect.anchorMax.y);
            rect.pivot = rect.anchorMin;
            rect.localPosition = new Vector3(-3, rect.localPosition.y, rect.localPosition.z);
        }
        else
        {
            rect.anchorMin = new Vector2(0, rect.anchorMin.y);
            rect.anchorMax = new Vector2(0, rect.anchorMax.y);
            rect.pivot = rect.anchorMin;
            rect.localPosition = new Vector3(3, rect.localPosition.y, rect.localPosition.z);
        }

        disableTick = 5;
        disableTimer = 0;
        disable = true;

        Canvas.ForceUpdateCanvases();
        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;

        backPanel.enabled = true;
        floatText.text = content;

        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();

        Canvas.ForceUpdateCanvases();
        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    public void DisableFloatText()
    {
        disableTimer = 0;
        disableTick = 0.1f;
        disable = true;
    }

    void Disable()
    {
        backPanel.enabled = false;
        floatText.text = " ";
        disable = false;
    }
}
