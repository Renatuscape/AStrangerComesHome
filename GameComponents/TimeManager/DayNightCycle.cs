using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    public Volume volume;
    public DataManagerScript dataManager;
    public SpriteRenderer nightSkySprite;
    //public SpriteRenderer starSprite; MAKE A SINGLE STAR SPRITE
    private float alphaSetting;


    void Start()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        volume.weight = 1;
        alphaSetting = 0;
    }

    void LateUpdate()
    {
        alphaSetting = volume.weight;

        //nightSkySprite.color = new Color(0.03364186f, 0.07704131f, 0.1132075f, alphaSetting);
        //starSprite.color = new Color(0.03364186f, 0.07704131f, 0.1132075f, alphaSetting);

        if (dataManager != null)
        {
            if (dataManager.timeOfDay < 0.5)
            {
                volume.weight = 1 - (dataManager.timeOfDay * 2); //works only for 1 day. Day must be reset to 0
            }
            else if (dataManager.timeOfDay > 0.5)
            {
                volume.weight = -1 + (dataManager.timeOfDay * 2); //works only for 1 day. Day must be reset to 0
            }
        }
    }
}
