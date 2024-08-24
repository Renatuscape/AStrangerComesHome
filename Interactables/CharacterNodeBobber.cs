using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNodeBobber : MonoBehaviour
{
    public InteractableBobber bobber;
    public bool alwaysShopIcon;
    public bool alwaysHideBobber = false;
    public bool alwaysShowBobber = false;
    public bool doNotOverrideLayer = false;
    public Sprite iconShop;
    public SpriteRenderer rend;
    Character character;
    bool ready = false;
    bool isCheckedSinceStateChange = false;
    private void Update()
    {
        if (ready && character != null)
        {
            if (!isCheckedSinceStateChange && TransientDataScript.GameState == GameState.Overworld)
            {
                EnableBobber(character);
            }
            else if (isCheckedSinceStateChange && TransientDataScript.GameState != GameState.Overworld)
            {
                isCheckedSinceStateChange = false;
            }
        }
    }

    public void HideBobber()
    {
        bobber.gameObject.SetActive(false);
    }

    public void ShowBobber()
    {
        bobber.gameObject.SetActive(true);
    }

    public void EnableBobber(Character incomingCharacter, SpriteRenderer rendIn = null)
    {
        isCheckedSinceStateChange = true;
        Debug.Log("Attempting to set up bobber.");
        character = incomingCharacter;

        if (!ready)
        {

            Setup();
        }

        if (!doNotOverrideLayer && rendIn != null)
        {
            rend.sortingLayerName = rendIn.sortingLayerName;
            rend.sortingOrder = rendIn.sortingOrder;
        }

        if (ready)
        {
            Debug.Log("Bobber was ready. Running checks.");
            if (!alwaysHideBobber && character != null)
            {
                if (alwaysShowBobber)
                {
                    ShowBobber();
                }
                else if (CharacterHasActiveShops() || CharacterHasActiveDialogue())
                {
                    Debug.Log(character.objectID + " bobber passed shop or dialogue test.");
                    ShowBobber();
                }
                else
                {
                    HideBobber();
                }
            }
            else
            {
                HideBobber();
            }
        }
    }
    void Setup()
    {
        if (bobber != null)
        {
            bobber.gameObject.SetActive(false);
            bobber.ready = false;

            if (alwaysShopIcon)
            {
                rend.sprite = iconShop;
            }

            ready = true;
        }
    }

    bool CharacterHasActiveDialogue()
    {
        List<Dialogue> dialogues = Dialogues.GetDialoguesBySpeaker(character.objectID, true);

        foreach (var dialogue in dialogues)
        {
            if (dialogue.questStage < 100 && dialogue.stageType == StageType.Dialogue && dialogue.CheckRequirements())
            {
                Debug.Log(dialogue.objectID + " returned true when setting up bobber for " + character.objectID);
                return true;
            }
        }

        Debug.Log("Dialogue returned false when setting up bobber for " + character.objectID);
        return false;
    }

    bool CharacterHasActiveShops()
    {
        foreach (var shop in character.shops)
        {
            if ((character.type == CharacterType.Generic || Player.GetCount(character.objectID, name) > 0) && shop.CheckRequirements())
            {
                Debug.Log(shop.objectID + " returned true when setting up bobber for " + character.objectID);
                rend.sprite = iconShop;
                return true;
            }
        }

        Debug.Log("Shop returned false when setting up bobber for" + character.objectID);
        return false;
    }
}
