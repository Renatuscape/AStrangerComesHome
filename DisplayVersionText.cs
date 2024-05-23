using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayVersionText : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = "Version " + Application.version;
    }
}
