using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Item weedsObject;
    public PlanterScript planterParent;
    void Start()
    {
        weedsObject = Items.FindByID(StaticTags.WeedItem);
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView == CameraView.Garden)
        {
            Player.Add(weedsObject.objectID);

            planterParent.planterData.weeds--;
            Destroy(gameObject);
            int rollForBonus = Random.Range(0, 100);

            if (rollForBonus > 85)
            {
                Player.Add(weedsObject.objectID, 1);
            }
        }
    }
}
