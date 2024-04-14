using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePortraitManager : MonoBehaviour
{
    // Portraits should be anchored at centre. If anchor changes, these measurements will be off.

    public PlayerSprite playerSprite;

    float defaultPositionY = -350;

    float positionClose = 230;
    float positionNormal = 560;
    float positionMid = 0;
    float positionFar = 860;
    float positionOff = 1600;

    float portraitWidth = 1228;
    float portraitHeight = 1736;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
