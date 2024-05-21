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
        dataManager = TransientDataScript.gameManager.dataManager;

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
        }
    }

    void Progress()
    {
        UpdateValues();

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
            }
        }
    }

    void ConsumeMana(SynthesiserData synth)
    {
        int manaCost = baseManaConsumption * synth.synthRecipe.manaDrainRate;

        if (TransientDataScript.transientData.currentMana >= manaCost + 5)
        {
            TransientDataScript.transientData.currentMana -= manaCost;
            synth.progressSynth += progressAmount;
        }
        else
        {
            TransientDataScript.transientData.currentMana = 0;
            synth.progressSynth += 1;
            synth.isSynthPaused = true;
            TransientDataScript.PushAlert($"Alchemy synthesis was paused due to low mana.");
        }
    }

    void UpdateValues()
    {
        timer = 0;
        tick = 10 - (Player.GetCount(StaticTags.Hermeneutics, name) * 0.9f);
        progressAmount = 1 + (Player.GetCount(StaticTags.Chemistry, name) * 0.6f);
        baseManaConsumption = (int)Mathf.Floor(6 - (Player.GetCount(StaticTags.Metaphysics, name) * 0.5f));
    }
}
