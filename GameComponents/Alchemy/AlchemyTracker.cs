using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyTracker : MonoBehaviour
{
    DataManagerScript dataManager;
    public float timer;
    public float tick;
    public float progressAmount;
    public int baseManaConsumption;

    private void OnEnable()
    {
        baseManaConsumption = 5;
        Debug.Log("Enabling Alchemy Tracker.");
        dataManager = TransientDataCalls.gameManager.dataManager;
        UpdateValues();
    }

    private void Update()
    {
        if (dataManager.alchemySynthesisers != null && dataManager.alchemySynthesisers.Count > 0)
        {
            if (TransientDataScript.IsTimeFlowing())
            {
                timer += Time.deltaTime;

                if (timer > tick)
                {
                    Progress();
                }
            }
        }
        else
        {
            gameObject.SetActive(false); // Don't use this resource unless it's actually necessary
        }
    }

    void Progress()
    {
        UpdateValues();

        bool anyActiveSynthesiser = false;

        foreach (SynthesiserData synth in dataManager.alchemySynthesisers)
        {
            if (synth.isSynthActive && !synth.isSynthPaused && synth.synthRecipe != null)
            {
                if (synth.progressSynth < synth.synthRecipe.workload)
                {
                    if (synth.consumesMana)
                    {
                        int manaCost = baseManaConsumption * synth.synthRecipe.manaDrainRate;

                        if (TransientDataCalls.transientData.currentMana > manaCost)
                        {
                            TransientDataCalls.transientData.currentMana -= manaCost;
                            anyActiveSynthesiser = true;
                            synth.progressSynth += progressAmount;
                        }
                        else
                        {
                            synth.isSynthPaused = true;
                        }
                    }
                    else
                    {
                        anyActiveSynthesiser = true;
                        synth.progressSynth += progressAmount;
                    }
                }
            }
        }

        if (!anyActiveSynthesiser)
        {
            Debug.Log("No active synthesiser was found. Disabling Alchemy Tracker.");
            gameObject.SetActive(false);
        }
    }

    void UpdateValues()
    {
        timer = 0;
        tick = 10 - (Player.GetCount("ALC004", name) * 0.9f);
        progressAmount = 1 + (Player.GetCount("ALC003", name) * 0.6f);
    }
}
