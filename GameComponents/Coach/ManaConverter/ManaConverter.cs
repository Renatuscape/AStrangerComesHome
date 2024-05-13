using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManaConverter : MonoBehaviour
{
    public TransientDataScript transientData;

    public int manapool;
    public int manaPassiveGeneration;
    public int manaClickPotency;

    private void Awake()
    {
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        transientData.currentMana = 0;
        transientData.manapool = 100;
    }
    void Start()
    {
        Invoke("PassiveManaRegeneration", 0);
    }

    private void OnEnable()
    {
        SyncUpgrades();
    }

    void SyncUpgrades()
    {
        manaClickPotency = Player.GetCount("MAU000", "ManaConverter");
        manaPassiveGeneration = Player.GetCount("MAU001", "ManaConverter");
        manapool = Player.GetCount("MAU002", "ManaConverter");
    }
    void UpdateManaPool()
    {
        transientData.manapool = 100 + (manapool * 30);
    }

    void PassiveManaRegeneration()
    {
        //SyncUpgrades();

        //REGENERATION AMOUNT
        var manaRecovery = 0.3f + (0.1f * manaPassiveGeneration);
        UpdateManaPool();


        //PASSIVE MANA ONLY REGENERATES DURING THESE STATES
        if (TransientDataScript.GameState == GameState.Overworld
            || TransientDataScript.GameState == GameState.ShopMenu
            || TransientDataScript.GameState == GameState.Dialogue
            || TransientDataScript.GameState == GameState.PlayerHome
            || TransientDataScript.GameState == GameState.MapMenu)
        {
            if (transientData.currentMana < transientData.manapool)
            {
                transientData.currentMana = transientData.currentMana + manaRecovery;

            }
            else
                transientData.currentMana = transientData.manapool;
        }

        //FREQUENCY
        var regenFrequency = 0.2f - (0.01f * manaPassiveGeneration);
        if (regenFrequency < 0.01f)
            regenFrequency = 0.01f;

        Invoke("PassiveManaRegeneration", regenFrequency);
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld || TransientDataScript.GameState == GameState.ShopMenu)
        {
            var clickRecovery = 1 + (2 * manaClickPotency) / 10;  //can be expressed as 0.2 * level, but wrong variable type. Allows for +3 at level 10

            if (transientData.currentMana < transientData.manapool)
            {
                transientData.currentMana = transientData.currentMana + clickRecovery;
            }
            else
                transientData.currentMana = transientData.manapool;
        }
    }
}
