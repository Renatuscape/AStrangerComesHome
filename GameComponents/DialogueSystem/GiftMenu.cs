using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftMenu : MonoBehaviour
{
    public Character character;
    public PortraitRenderer portraitRenderer;
    public void Setup(Character character)
    {
        this.character = character;
        portraitRenderer.EnableForGifting(character);
        TransientDataCalls.SetGameState(GameState.AlchemyMenu, name, gameObject);
    }
}
