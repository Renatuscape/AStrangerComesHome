using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyManager : MonoBehaviour
{/*
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public GameObject synthMenuA;
    public Skill creation; //synth progress speed


    public float synthTimer;
    public float synthTick;

    private void Awake()
    {
        synthTick = 1;
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    private void FixedUpdate()
    {
        synthTimer += Time.fixedDeltaTime;

        if (synthTimer >= synthTick)
        {
            SynthTick();
            synthTimer = 0;
        }

    }
    void SynthTick()
    {
        if (dataManager.isSynthActiveA && dataManager.synthItemA != null && !dataManager.isSynthPausedA) //Synthesis must be active, there must be a synth item, and synth cannot be paused
            UpdateSynthesiser(ref dataManager.progressSynthA, ref dataManager.synthItemA);

        //          !!! add synthB and synthC !!!
    }

    void UpdateSynthesiser(ref float progress, ref Item item)
    {
        if (progress <= item.recipe.maxSynth)
        {
            progress += 1 + (creation.dataValue * 0.2f);
        }
    }

    public void EnableSynthMenuA()
    {
        synthMenuA.SetActive(true);

    }
    public void DisableSynthMenuA()
    {
        synthMenuA.SetActive(false);
    }
    void Update()
    {
        
    }*/
}
