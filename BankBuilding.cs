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
        var componentContainer = GameObject.Find("GameComponents");
        if (componentContainer != null)
        {
            var bankObject = componentContainer.transform.Find("BankComponent").gameObject;

            if (bankObject != null)
                bankManager = bankObject.GetComponent<BankManager>();
            else
                Debug.LogError("BankManager not found");
        }
        else
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
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            bankManager.OpenBankMenu();
        }
    }
}
