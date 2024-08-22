using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBobber : MonoBehaviour
{
    public GameObject bobberObject;
    public bool ready = false;
    private void Start()
    {
        bobberObject.SetActive(false);
        StartCoroutine(EnableBobber(Random.Range(1f, 2f)));
    }
    private void Update()
    {
        if (ready && TransientDataScript.CameraView == CameraView.Normal)
        {
            if (!bobberObject.activeInHierarchy)
            {
                bobberObject.SetActive(true);
            }
        }
        else
        {
            if (bobberObject.activeInHierarchy)
            {
                bobberObject.SetActive(false);
            }
        }
    }

    IEnumerator EnableBobber(float delay)
    {
        yield return new WaitForSeconds(delay);
        bobberObject.SetActive(true);
        ready = true;
    }
}
