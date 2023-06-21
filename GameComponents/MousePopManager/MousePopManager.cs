using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MousePopManager : MonoBehaviour
{
    public TransientDataScript transientData;
    public TextMeshProUGUI mousePop;
    float popTimer;
    Vector3 worldPosition;

    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transientData.gameState == GameState.Overworld && transientData.cameraView == CameraView.Normal)
        {
            Vector3 mousePos = Input.mousePosition;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

            mousePop.transform.position = new Vector3(worldPosition.x + 2f, worldPosition.y + 0.5f, 0);

            if (popTimer > 0)
            {
                popTimer -= Time.deltaTime;
            }

            if (popTimer <= 0 && mousePop.text != " ")
            {
                mousePop.text = " ";
            }
        }
        else
        {
            mousePop.text = " ";
        }
    }

    public void MousePopText(string content)
    {
        popTimer = 0.05f;
        mousePop.text = content;
    }
}
