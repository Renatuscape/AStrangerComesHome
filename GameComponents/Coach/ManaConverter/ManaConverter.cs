using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaConverter : MonoBehaviour
{
    public SpawnParticlesTrigger spawnParticlesTrigger;
    public static ManaConverter instance;

    public int manapool;
    public int manaPassiveGeneration;
    public int manaClickPotency;
    public int skillMagic;
    public int attMysticism;

    public Upgrade MAU000;
    public Upgrade MAU001;
    public Upgrade MAU002;
    public bool isReady = false;
    bool clickDelay = false;
    public void Enable()
    {
        TransientDataScript.transientData.currentMana = 0;
        TransientDataScript.transientData.manapool = 100;

        MAU000 = Upgrades.FindByID("MAU000");
        MAU001 = Upgrades.FindByID("MAU001");
        MAU002 = Upgrades.FindByID("MAU002");

        instance = this;

        SyncUpgrades();
        isReady = true;
    }

    public static void SyncUpgrades()
    {
        if (instance != null && instance.isReady)
        {
            instance.manaClickPotency = Player.GetCount("MAU000", "ManaConverter");
            instance.manaPassiveGeneration = Player.GetCount("MAU001", "ManaConverter");
            instance.manapool = Player.GetCount("MAU002", "ManaConverter");
            instance.skillMagic = Player.GetCount(StaticTags.Magic, "ManaConverter");
            instance.attMysticism = Player.GetCount(StaticTags.Mysticism, "ManaConverter");

            instance.UpdateManaPool();
        }
    }
    void UpdateManaPool()
    {
        if (MAU002.isBroken)
        {
            TransientDataScript.transientData.manapool = 50 + (manapool * 5) + (skillMagic * 5);
        }
        else
        {
            TransientDataScript.transientData.manapool = 100 + (manapool * 30) + (skillMagic * 30);
        }
    }

    public static void GlobalPushManaRegen()
    {
        if (instance != null && TransientDataScript.IsTimeFlowing() &&  instance.isReady)
        {
            //REGENERATION AMOUNT
            var manaRecovery = 1.5f + (0.2f * instance.manaPassiveGeneration) + (0.15f * instance.attMysticism);

            if (TransientDataScript.transientData.engineState == EngineState.Off)
            {
                manaRecovery = manaRecovery * 2f;
            }

            if (instance.MAU001.isBroken)
            {
                manaRecovery = manaRecovery * 0.5f;
            }

            if (TransientDataScript.transientData.currentMana < TransientDataScript.transientData.manapool)
            {
                TransientDataScript.transientData.currentMana = TransientDataScript.transientData.currentMana + manaRecovery;
            }
            else
            {
                TransientDataScript.transientData.currentMana = TransientDataScript.transientData.manapool;
            }
        }
    }

    private void OnMouseDown()
    {
        ManaClick();
    }

    public void ManaClick()
    {
        if (TransientDataScript.GameState == GameState.Overworld && !clickDelay)
        {
            clickDelay =  true;
            spawnParticlesTrigger.Spawn();

            float clickRecovery;

            if (MAU000.isBroken)
            {
                clickRecovery = 0.5f + (manaClickPotency * 0.1f) + (attMysticism * 0.1f);
            }
            else
            {
                clickRecovery = 1.5f + (manaClickPotency * 0.7f + (attMysticism * 0.5f));
            }

            if (TransientDataScript.transientData.currentMana + clickRecovery <= TransientDataScript.transientData.manapool)
            {
                TransientDataScript.transientData.currentMana = TransientDataScript.transientData.currentMana + clickRecovery;
            }
            else
            {
                TransientDataScript.transientData.currentMana = TransientDataScript.transientData.manapool;
            }

            StartCoroutine(ClickDelay());
        }
    }

    IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(0.2f);
        clickDelay = false;
    }
}
