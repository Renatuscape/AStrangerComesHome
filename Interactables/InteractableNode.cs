using System.Collections;
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

    internal IEnumerator FadeOutObject()
    {
        float alpha = rend.color.a;
        float fadeAmount = 0.001f;

        while (rend.color.a > 0)
        {
            alpha -= fadeAmount;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, alpha);
            fadeAmount += 0.001f;
            yield return new WaitForSeconds(0.01f);
        }

        gameObject.SetActive(false);
    }
}
