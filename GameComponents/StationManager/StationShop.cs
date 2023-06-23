using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StationShop : MonoBehaviour
{
    public Shop shopType;
    public TransientDataScript transientData;
    public BoxCollider2D col;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (transientData.cameraView == CameraView.Normal)
            col.enabled = true;
        else
            col.enabled = false;
    }
    public void OnMouseDown()
    {
        if (transientData.gameState == GameState.Overworld && transientData.cameraView == CameraView.Normal)
        {
            transientData.currentShop = shopType;
            transientData.ChangeGameState(name, gameObject, GameState.ShopMenu);
            //transientData.gameState = GameState.ShopMenu;
            //Debug.Log(name + " changed GameState to " + GameState.ShopMenu);
        }
    }

    public void OnMouseOver()
    {
        if (transientData.gameState == GameState.Overworld && transientData.cameraView == CameraView.Normal)
        {
            transientData.PrintFloatText($"{shopType.shopName}");
        }
    }

    public void OnMouseExit()
    {
        transientData.DisableFloatText();
    }
}
