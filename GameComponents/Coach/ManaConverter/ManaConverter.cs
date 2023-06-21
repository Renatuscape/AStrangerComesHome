using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManaConverter : MonoBehaviour
{
    public TransientDataScript transientData;

    public MotherObject manapool;
    public MotherObject manaPassiveGeneration;
    public MotherObject manaClickPotency;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        transientData.currentMana = 0;
        transientData.manapool = 100;
    }
    void Start()
    {
        for (int i=0; i < transientData.objectIndex.Count; i++) //FIND THOSE SCRIPTABLE BOYS
        {
            if (transientData.objectIndex[i].name == "Manapool")
            {
                manapool = transientData.objectIndex[i];
            }
            if (transientData.objectIndex[i].name == "ManaPassiveGeneration")
            {
                manaPassiveGeneration = transientData.objectIndex[i];
            }
            if (transientData.objectIndex[i].name == "ManaClickPotency")
            {
                manaClickPotency = transientData.objectIndex[i];
            }
        }

        Invoke("PassiveManaRegeneration", 0);
    }

    void UpdateManaPool()
    {
        transientData.manapool = 100 + (manapool.dataValue * 30);
    }
    
    void PassiveManaRegeneration()
    {
        //REGENERATION AMOUNT
        var manaRecovery = 0.3f + (0.1f * manaPassiveGeneration.dataValue);
        UpdateManaPool();


        //PASSIVE MANA ONLY REGENERATES DURING THESE STATES
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.ShopMenu || transientData.gameState == GameState.Dialogue || transientData.gameState == GameState.PlayerHome || transientData.gameState == GameState.MapMenu)
        {
            if (transientData.currentMana < transientData.manapool)
            {
                transientData.currentMana = transientData.currentMana + manaRecovery;

            }
            else
                transientData.currentMana = transientData.manapool;
        }

        //FREQUENCY
        var regenFrequency = 0.2f - (0.01f * manaPassiveGeneration.dataValue);
        if (regenFrequency < 0.01f)
            regenFrequency = 0.01f;

        Invoke("PassiveManaRegeneration", regenFrequency);
    }

    private void OnMouseDown()
    {
        if (transientData.gameState == GameState.Overworld || transientData.gameState == GameState.ShopMenu)
        {
            var clickRecovery = 1 + (2 * manaClickPotency.dataValue) / 10;  //can be expressed as 0.2 * level, but wrong variable type. Allows for +3 at level 10

            if (transientData.currentMana < transientData.manapool)
            {
                transientData.currentMana = transientData.currentMana + clickRecovery;
            }
            else
                transientData.currentMana = transientData.manapool;
        }
    }
}
