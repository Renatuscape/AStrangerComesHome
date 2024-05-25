using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayResolutionText : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<TextMeshProUGUI>().text = Screen.width + "x" + Screen.height;
    }
}
