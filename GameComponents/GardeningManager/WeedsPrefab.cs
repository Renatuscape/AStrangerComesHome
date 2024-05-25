using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public PlanterScript planterParent;

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView == CameraView.Garden)
        {

            planterParent.planterData.weeds--;
            Destroy(gameObject);
            int rollForBonus = Random.Range(0, 100);

            if (rollForBonus > 85)
            {
                Player.Add(StaticTags.WeedItem, 2);
            }
            else
            {
                Player.Add(StaticTags.WeedItem);
            }
        }
    }
}
