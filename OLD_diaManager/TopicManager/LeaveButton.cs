using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveButton : MonoBehaviour
{
    public GameObject dialogueManager;
    public void ButtonDown()
    {
        dialogueManager.SetActive(false);
    }
}
