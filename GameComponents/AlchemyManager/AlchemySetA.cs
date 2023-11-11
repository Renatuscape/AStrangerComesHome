using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemySetA : MonoBehaviour
{/*
    public DataManagerScript dataManager;
    public TransientDataScript transientData;
    public AlchemyManager alchemyManager;

    public SynthState synthState;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    private void OnMouseDown()
    {
        if (transientData.cameraView == CameraView.Cockpit)
        {
            alchemyManager.EnableSynthMenuA();
        }
    }

    private void Update()
    {
        if (transientData.cameraView != CameraView.Cockpit || transientData.gameState != GameState.Overworld)
        {
            alchemyManager.DisableSynthMenuA();
        }

        if (!dataManager.isSynthActiveA)
            synthState = SynthState.Inactive;
        if (dataManager.isSynthActiveA && dataManager.synthItemA != null)
        {
            if (dataManager.isSynthPausedA)
                synthState = SynthState.Paused;
            else if (dataManager.progressSynthA < dataManager.synthItemA.recipe.maxSynth)
                synthState = SynthState.Active;
            else if (dataManager.progressSynthA >= dataManager.synthItemA.recipe.maxSynth)
                synthState = SynthState.Complete;
        }
    }*/
}
