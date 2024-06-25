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
    public SpriteRenderer rend;
    public BoxCollider2D col;
    public InteractableBundleText textBundle;
    public bool hideOnLoot;
    bool isReadable;

    void Start()
    {
        isReadable = textBundle.Initialise();

        if (!isReadable )
        {
            col.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.IsTimeFlowing() && isReadable)
        {
            ProcessClick();
        }
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
