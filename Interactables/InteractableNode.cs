using UnityEngine;

public class InteractableNode : MonoBehaviour
{
    public string nodeID;
    public string animationID;
    public bool hideOnLoot;
    public bool hideBobber;
    public InteractableBobber interactBobber;
    public SpriteRenderer rend;
    public BoxCollider2D col;
    public AnimatedSprite animatedSprite;
}
