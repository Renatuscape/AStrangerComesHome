using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Item weedsObject;
    public planterScript planterParent;
    void Start()
    {
        weedsObject = Items.FindByID("PLA000");
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView == CameraView.Garden)
        {
            Player.Add(weedsObject.objectID);

            planterParent.currentWeeds--;
            Destroy(gameObject);
            int rollForBonus = Random.Range(0, 100);

            if (rollForBonus > 85)
            {
                Player.Add(weedsObject.objectID, 1);
            }
        }
    }
}
