using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuFloatTextScript : MonoBehaviour
{
    public TransientDataScript transientData;
    public TextMeshProUGUI floatText;
    public Image backPanel;

    void Awake()
    {
        DisableFloatText();
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
    }

    public void PrintFloatText(string content)
    {
        backPanel.enabled = true;
        floatText.text = content;
        Canvas.ForceUpdateCanvases();
        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false;
        floatText.transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    public void DisableFloatText()
    {
        backPanel.enabled = false;
        floatText.text = " ";
    }
}
