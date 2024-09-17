using UnityEngine;

public class InteractableNodeMemory : InteractableNode
{
    // Use NodeID to in hierarchy to set the memory ID

    public Memory memory;
    public bool autoPlayMemory;
    bool isReady = false;
    private void OnEnable()
    {
        if (TestConditions())
        {
            Setup();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    bool TestConditions()
    {
        if (Player.GetCount(nodeID, name) > 0)// Additional memories cannot be gained from locations, only quests/events
        {
            return false;
        }

        memory = Memories.FindByID(nodeID);

        if (memory != null)
        {
            if (memory.dialogue.CheckRequirements())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    void Setup()
    {
        animatedSprite = AnimationLibrary.GetAnimatedObject("MemoryShard" + (memory.isUnique ? "_Unique" : ""));

        if (animatedSprite != null && rend != null)
        {
            var idleAnim = animatedSprite.GetAnimationType(AnimationType.idle);

            if (idleAnim != null)
            {
                StartCoroutine(Animate(idleAnim, true));
            }

        }
        else
        {
            Debug.LogWarning(nodeID + " node could not animate. AnimatedSprite or SpriteRenderer was null.");
        }

        isReady = true;
    }

    private void OnMouseDown()
    {
        if (isReady && TransientDataScript.IsTimeFlowing())
        {
            CollectMemory();

            if (autoPlayMemory)
            {
                TransientDataScript.gameManager.storySystem.StartMemory(memory);
            }
        }
    }

    void CollectMemory()
    {
        col.enabled = false;
        Player.Add(nodeID);

        if (animatedSprite != null)
        {
            var useAnim = animatedSprite.GetAnimationType(AnimationType.use);

            if (useAnim != null)
            {
                StartCoroutine(AnimateAndFadeOut(useAnim, true));
            }
            else
            {
                Debug.LogWarning(nodeID + " node found no animation of the USE type. Check animation library or MemoryShard animations.");
                StartCoroutine(FadeOutObject());
            }
        }
        else
        {
            Debug.LogWarning(nodeID + " node had no animated sprite. Check animation library or MemoryShard animations.");
            StartCoroutine(FadeOutObject());
        }
    }
}
