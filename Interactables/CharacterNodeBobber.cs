using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterNodeBobber : MonoBehaviour
{
    public InteractableBobber interactableBobber;
    public bool alwaysHideBobber = false;
    public bool alwaysShowBobber = false;
    GameObject bobber;
    Character character;

    public void HideBobber()
    {
        bobber.SetActive(false);
    }

    public void ShowBobber()
    {
        bobber.SetActive(true);
    }
    void Start()
    {
        bobber = interactableBobber.gameObject;
        bobber.SetActive(false);
    }

    public void EnableBobber(Character incomingCharacter)
    {
        character = incomingCharacter;
        if (!alwaysHideBobber && character != null)
        {
            if (alwaysShowBobber)
            {
                bobber.SetActive(true);
            }
            else if (CharacterHasActiveShops() || CharacterHasActiveDialogue())
            {
                bobber.SetActive(true);
            }
        }
    }

    bool CharacterHasActiveDialogue()
    {
        List<Dialogue> dialogues = Dialogues.GetDialoguesBySpeaker(character.objectID, true);

        foreach (var dialogue in dialogues)
        {
            if (dialogue.CheckRequirements())
            {
                Debug.Log(dialogue.objectID + " returned true when setting up bobber for " + character.objectID);
                return true;
            }
        }
        return false;
    }

    bool CharacterHasActiveShops()
    {
        foreach (var shop in character.shops)
        {
            if (shop.CheckRequirements())
            {
                Debug.Log(shop.objectID + " returned true when setting up bobber for " + character.objectID);
                return true;
            }
        }
        return false;
    }
}
