using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCanvasController : MonoBehaviour
{
    public GameObject largeWallet;
    public GameObject smallWallet;

    public GameObject largeGearbox;
    public GameObject smallGearbox;

    private void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            largeWallet.SetActive(GlobalSettings.uiWalletLarge);
            smallWallet.SetActive(!GlobalSettings.uiWalletLarge);

            largeGearbox.SetActive(GlobalSettings.uiGearboxLarge);
            smallGearbox.SetActive(!GlobalSettings.uiGearboxLarge);
        }
        else
        {
            largeWallet.SetActive(false);
            smallWallet.SetActive(false);
            largeGearbox.SetActive(false);
            smallGearbox.SetActive(false);
        }
    }
    public void ToggleWallet()
    {
        if (largeWallet.activeInHierarchy)
        {
            largeWallet.SetActive(false);
            smallWallet.SetActive(true);
        }
        else
        {
            largeWallet.SetActive(true);
            smallWallet.SetActive(false);
        }

        GlobalSettings.uiWalletLarge = largeWallet.activeInHierarchy;
        GlobalSettingsManager.SaveSettings();
        AudioManager.PlayUISound("metalClick");
    }

    public void ToggleGearbox()
    {
        if (largeGearbox.activeInHierarchy)
        {
            largeGearbox.SetActive(false);
            smallGearbox.SetActive(true);
        }
        else
        {
            largeGearbox.SetActive(true);
            smallGearbox.SetActive(false);
        }

        GlobalSettings.uiGearboxLarge = largeGearbox.activeInHierarchy;
        GlobalSettingsManager.SaveSettings();
        AudioManager.PlayUISound("metalClick");
    }
}
