using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlchemyRecipePrefab : MonoBehaviour
{
    public TextMeshProUGUI recipeTitle;
    public Anim_BobLoop bobAnimation;
    public GameObject recipeContent;
    public float xOffset;

    private void Start()
    {
        xOffset = Random.Range(-7, 7);
        var lastChildIndex = transform.parent.transform.childCount - 1;
        var lastChildObject = transform.parent.transform.GetChild(lastChildIndex).gameObject;

        if (lastChildObject == this)
        {
            OffsetContent(0);
        }
        else
        {
            OffsetContent(xOffset);
        }
    }

    public void OffsetContent(float offset)
    {
        recipeContent.transform.position = new Vector3(offset, recipeContent.transform.position.y, 0);
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
        OffsetContent(0);
    }

    public void BringToBack()
    {
        transform.SetAsFirstSibling();
        OffsetContent(xOffset);
    }
}
