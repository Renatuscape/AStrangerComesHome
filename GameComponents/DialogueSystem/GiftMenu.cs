using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftMenu : MonoBehaviour
{
    public Character character;
    public PortraitRenderer portraitRenderer;
    public GameObject giftMenu;
    public void Setup(Character character)
    {
        this.character = character;
        portraitRenderer.EnableForGifting(character);
        gameObject.SetActive(true);
        TransientDataCalls.SetGameState(GameState.Dialogue, name, gameObject);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }
}
