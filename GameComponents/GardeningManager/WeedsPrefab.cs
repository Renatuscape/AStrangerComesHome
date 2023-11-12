using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Item weedsObject;
    public planterScript planterParent;
    public TransientDataScript transientData;
    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        weedsObject = Items.all.Find(x => x.objectID == weedsObject.objectID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (transientData.cameraView == CameraView.Garden)
        {
            weedsObject.AddToPlayer();

            planterParent.currentWeeds--;
            Destroy(gameObject);
        }
    }
}
