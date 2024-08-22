using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankBuilding : MonoBehaviour
{
    public BankManager bankManager;
    public TransientDataScript transientData;
    public BoxCollider2D col;

    void Awake()
    {
        bankManager = TransientDataScript.gameManager.bankManager;

        if (bankManager == null)
        {
            Debug.LogError("GameComponents not found");
        }

        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (TransientDataScript.CameraView == CameraView.Normal)
            col.enabled = true;
        else
            col.enabled = false;
    }

    public void OnMouseDown()
    {
        if (bankManager != null && TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            bankManager.OpenBankMenu();
        }
    }
}
