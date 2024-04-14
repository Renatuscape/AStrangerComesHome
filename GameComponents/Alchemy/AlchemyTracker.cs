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

    bool isEnabled = false;

    private void Awake()
    {
        if (!isEnabled)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartTracking()
    {
        Debug.Log("Enabling Alchemy Tracker.");
        dataManager = TransientDataCalls.gameManager.dataManager;

        UpdateValues();
        isEnabled = true;
    }

    private void Update()
    {
        if (isEnabled)
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
            //else
            //{
            //    gameObject.SetActive(false); // Don't use this resource unless it's actually necessary
            //}   
        }
    }

    void Progress()
    {
        UpdateValues();

        //bool anyActiveSynthesiser = false;

        foreach (SynthesiserData synth in dataManager.alchemySynthesisers)
        {
            if (synth.isSynthActive && !synth.isSynthPaused && synth.synthRecipe != null && synth.progressSynth < synth.synthRecipe.workload)
            {
                if (synth.consumesMana)
                {
                    ConsumeMana(synth);
                }
                else
                {
                    synth.progressSynth += progressAmount;
                }

                //anyActiveSynthesiser = true;
            }
        }

        //if (!anyActiveSynthesiser)
        //{
        //    Debug.Log("No active synthesiser was found. Disabling Alchemy Tracker.");
        //    gameObject.SetActive(false);
        //}
    }

    void ConsumeMana(SynthesiserData synth)
    {
        int manaCost = baseManaConsumption * synth.synthRecipe.manaDrainRate;

        if (TransientDataCalls.transientData.currentMana >= manaCost + 5)
        {
            TransientDataCalls.transientData.currentMana -= manaCost;
            synth.progressSynth += progressAmount;
        }
        else
        {
            TransientDataCalls.transientData.currentMana = 0;
            synth.progressSynth += 1;
            synth.isSynthPaused = true;
            TransientDataCalls.PushAlert($"Alchemy synthesis was paused due to low mana.");
        }
    }

    void UpdateValues()
    {
        timer = 0;
        tick = 10 - (Player.GetCount("ALC004", name) * 0.9f);
        progressAmount = 1 + (Player.GetCount("ALC003", name) * 0.6f);
        baseManaConsumption = (int)Mathf.Floor(6 - (Player.GetCount("MAG001", name) * 0.5f));
    }
}
