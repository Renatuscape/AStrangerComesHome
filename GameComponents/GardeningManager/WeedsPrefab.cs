using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Plant weedsObject;
    public planterScript planterParent;
    public TransientDataScript transientData;
    void Start()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (transientData.cameraView == CameraView.Garden)
        {
            if (weedsObject.dataValue < weedsObject.maxValue)
                weedsObject.dataValue++;

            planterParent.currentWeeds--;
            Destroy(gameObject);
        }
    }
}
