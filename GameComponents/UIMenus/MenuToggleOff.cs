using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggleOff : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameState menuType;

    void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    private void OnEnable()
    {
        transientData.ChangeGameState(name, gameObject, menuType);
    }
    void Update()
    {
        if (transientData.gameState != menuType)
        {
            gameObject.SetActive(false);
        }
    }
}
