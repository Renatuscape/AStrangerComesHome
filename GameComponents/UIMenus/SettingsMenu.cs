using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public CanvasScaler canvasScaler;
    public Dropdown canvasScaleDropDown;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DropDownScaleUI(int index)
    {
        switch (index)
        {
            case 0: canvasScaler.scaleFactor = 1; break;
            case 1: canvasScaler.scaleFactor = 2; break;
            case 2: canvasScaler.scaleFactor = 3; break;
            case 3: canvasScaler.scaleFactor = 4; break;
            case 4: canvasScaler.scaleFactor = 5; break;
            default: canvasScaler.scaleFactor = 2; break;
        }
    }
}
