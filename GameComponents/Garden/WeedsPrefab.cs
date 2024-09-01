using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public GardenPlanter planterParent;

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView == CameraView.Garden)
        {

            planterParent.planterData.weeds--;
            Destroy(gameObject);
            int roll = Random.Range(0, 100);

            if (roll > 85)
            {
                Player.Add(StaticTags.WeedItem, 2);
            }
            else if (roll > 30)
            {
                Player.Add(StaticTags.WeedItem);
            }
        }
    }
}
