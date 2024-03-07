using UnityEngine;
using UnityEngine.EventSystems;

public class AlchemyContainer : MonoBehaviour, IDropHandler
{
    public int itemLimit = 30;
    public bool animate = true;
    public bool reverseAnimation;
    public float animationTimer;
    public float animationTick = 0.03f;
    RadialLayout radialLayout;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        AlchemyDraggableItem draggableItem = dropped.GetComponent<AlchemyDraggableItem>();
        draggableItem.parentContainer = gameObject.transform;
        Debug.Log("Dropped item");
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && gameObject.activeSelf)
        {
            if (animate)
            {
                animationTimer += Time.deltaTime;

                if (animationTimer >= animationTick)
                {
                    AnimationTick();
                }
            }
        }

    }

    public void AnimationTick()
    {
        animationTimer = 0;
        if (radialLayout == null)
        {
            radialLayout = GetComponent<RadialLayout>();
        }

        if (reverseAnimation)
        {
            if (radialLayout.StartAngle > 0)
            {
                radialLayout.StartAngle += -1;
            }
            else
            {
                radialLayout.StartAngle = 360;
            }
        }
        else
        {
            if (radialLayout.StartAngle < 360)
            {
                radialLayout.StartAngle += 1;
            }
            else
            {
                radialLayout.StartAngle = 0;
            }
        }
        radialLayout.CalculateRadial();
    }
}