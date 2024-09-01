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
        Debug.Log("CBob: Attempting to set up bobber: " + incomingCharacter.objectID);
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
            Debug.Log("CBob: Bobber was ready. Running checks on : " + character.objectID);
            if (!alwaysHideBobber && character != null)
            {
                if (alwaysShowBobber)
                {
                    ShowBobber();
                }
                else if (CharacterHasActiveShops() || CharacterHasActiveDialogue())
                {
                    Debug.Log("Cbob: " + character.objectID + " bobber passed shop or dialogue test.");
                    ShowBobber();
                }
                else
                {
                    Debug.Log("Cbob: " + character.objectID + " bobber failed shop or dialogue test.");
                    HideBobber();
                }
            }
            else
            {
                Debug.Log("Cbob: Character was null or .");
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
                Debug.Log("CBob: " + dialogue.objectID + " returned true when setting up bobber for " + character.objectID);
                return true;
            }
        }

        Debug.Log("CBob: Dialogue returned false when setting up bobber for " + character.objectID);
        return false;
    }

    bool CharacterHasActiveShops()
    {
        foreach (var shop in character.shops)
        {
            if ((character.type == CharacterType.Generic || Player.GetCount(character.objectID, name) > 0) && shop.CheckRequirements())
            {
                Debug.Log("CBob: " + shop.objectID + " returned true when setting up bobber for " + character.objectID);
                rend.sprite = iconShop;
                return true;
            }
        }

        Debug.Log("CBob: Shop returned false when setting up bobber for" + character.objectID);
        return false;
    }
}
