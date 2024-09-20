using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionsMemoryEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Memory memory;
    public SimpleAnimationController animController;
    public AnimatedSprite animatedSprite;
    public AnimationData idleAnimation;
    public GameObject shadow;
    public string strengthText;
    public string hoverText = "Forgotten Memory";

    public void EnableMemory(Memory memory, int strength)
    {
        this.memory = memory;
        shadow.SetActive(true);

        animatedSprite = AnimationLibrary.GetAnimatedObject("MemoryShard");

        if (animatedSprite != null)
        {
            idleAnimation = animatedSprite.GetAnimationType(AnimationType.idle);

            if (idleAnimation != null)
            {
                animController.PlayAnimation(idleAnimation, true);
            }
        }

        if (!string.IsNullOrEmpty(memory.name))
        {
            hoverText = memory.name;
        }
        else
        {
            hoverText = "???";
        }

        if (memory.isUnique)
        {
            strengthText = "";
        }
        else if (strength == 1)
        {
            strengthText = " (Dim)";
        }
        else if (strength == 2)
        {
            strengthText = " (Bright)";
        }
        else if (strength == 3)
        {
            strengthText = " (Brilliant)";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (memory != null && !string.IsNullOrEmpty(memory.objectID))
        {
            MemoryMenu.PlayMemory(memory, true);
        }
        else
        {
            Debug.LogWarning("Memory was null, but click was registered.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TransientDataScript.PrintFloatText(hoverText + strengthText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TransientDataScript.DisableFloatText();
    }
}
