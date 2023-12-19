using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public GameObject characterCreator;
    public void OpenCharacterCreation()
    {
        TransientDataScript.SetGameState(GameState.CharacterCreation, "Debug Menu", gameObject);
        characterCreator.SetActive(true);
    }

    public void DebugItems()
    {
        Items.DebugAllItems();
    }
}
