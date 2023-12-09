using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggleOff : MonoBehaviour
{
    public GameState menuType;

    private void OnEnable()
    {
        TransientDataScript.SetGameState(menuType,name, gameObject);
    }
    void Update()
    {
        if (TransientDataScript.GameState != menuType)
        {
            gameObject.SetActive(false);
        }
    }
}
