using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Item weedsObject;
    public planterScript planterParent;
    public TransientDataScript transientData;
    public int weedsPicked = 0;
    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        weedsObject = Items.allItems.Find(item => item.objectID.Contains("PLA000"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (transientData.cameraView == CameraView.Garden)
        {
            Player.AddItem(weedsObject.objectID, 1);
            planterParent.currentWeeds--;
            Destroy(gameObject);
            int rollForBonus = Random.Range(0, 100);

            if (rollForBonus > 85)
            {
                Player.AddItem(weedsObject.objectID, 1);
            }
        }
    }
}
