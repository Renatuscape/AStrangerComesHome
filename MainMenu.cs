using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TransientDataScript transientData;
    public GameObject parallaxManager;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            transientData.currentSpeed = 0.5f;
            parallaxManager.SetActive(true);
        }
    }

    private void OnDisable()
    {
        parallaxManager.SetActive(false);
        transientData.currentSpeed = 0;
    }
}
