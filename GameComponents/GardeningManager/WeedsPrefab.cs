using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Plant weedsObject;
    public planterScript planterParent;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (weedsObject.dataValue < weedsObject.maxValue)
            weedsObject.dataValue++;

        planterParent.currentWeeds--;
        Destroy(gameObject);
    }
}
