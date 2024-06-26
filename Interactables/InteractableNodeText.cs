using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InteractableNodeText : MonoBehaviour
{
    public enum TextType
    {
        LogAlert,
        PushAlert,
        Book
    }

    public string animationID;
    public bool hideOnLoot;
    public bool hideBobber;
    public InteractableBobber interactBobber;
    public SpriteRenderer rend;
    public BoxCollider2D col;
    public AnimatedSprite animatedSprite;
    public InteractableBundleText textBundle;
    bool isReadable;

    void Start()
    {
        isReadable = textBundle.Initialise();

        if (!isReadable )
        {
            col.enabled = false;
        }
        else
        {
            if (!hideBobber && interactBobber != null)
            {
                StartCoroutine(PlayBobber());
            }

            if (!string.IsNullOrEmpty(animationID))
            {
                animatedSprite = AnimationLibrary.GetAnimatedObject(animationID);
                StartCoroutine(Animate(animatedSprite.GetAnimationType(AnimationType.idle), true));
            }

        }
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.IsTimeFlowing() && isReadable)
        {
            ProcessClick();
        }
    }

    IEnumerator Animate(AnimationData animationData, bool loop, bool startFromCustom = false)
    {
        if (animationData != null)
        {
            int index = 0;

            if (startFromCustom)
            {
                index = animationData.customLoopStart;
            }

            int maxIndex = animationData.frames.Count - 1;

            while (index < maxIndex)
            {
                rend.sprite = animationData.frames[index];
                index++;
                yield return new WaitForSeconds(animationData.frameRate);
            }

            if (loop && !animationData.disallowLooping)
            {
                StartCoroutine(Animate(animationData, true));
            }
        }
    }

    IEnumerator PlayBobber()
    {
        yield return null;
    }
    void ProcessClick()
    {
        if (textBundle.type == TextType.LogAlert)
        {
            LogAlert.QueueTextAlert(textBundle.loadedText);
        }
        else if (textBundle.type == TextType.PushAlert)
        {
            TransientDataScript.PushAlert(textBundle.loadedText);
        }
        else if (textBundle.type == TextType.Book)
        {
            if (!textBundle.disableAddBookToInventory)
            {
                Book book = Books.FindByID(textBundle.textTag);
                if (book != null)
                {
                    if (textBundle.CheckIfLootable())
                    {
                        Player.Add(book.objectID);
                    }

                    JournalController.ForceReadBook(book);
                }
            }
        }

        if (!textBundle.lootClaimed)
        {
            textBundle.ClaimLoot();

            if (textBundle.disableReuse)
            {
                DisableNode();
            }
        }
        else if (textBundle.disableReuse)
        {
            textBundle.SaveNodeToPlayer();
            DisableNode();
        }
    }

    void DisableNode()
    {
        isReadable = false;
        col.enabled = false;

        if (hideOnLoot)
        {
            gameObject.SetActive(false);
        }
    }
}
