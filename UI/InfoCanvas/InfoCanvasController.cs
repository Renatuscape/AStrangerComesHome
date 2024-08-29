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
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            largeWallet.SetActive(GlobalSettings.UiWalletLarge);
            smallWallet.SetActive(!GlobalSettings.UiWalletLarge);

            largeGearbox.SetActive(GlobalSettings.UiGearboxLarge);
            smallGearbox.SetActive(!GlobalSettings.UiGearboxLarge);
        }
        else if (TransientDataScript.GameState == GameState.ShopMenu)
        {
            largeWallet.SetActive(GlobalSettings.UiWalletLarge);
            smallWallet.SetActive(!GlobalSettings.UiWalletLarge);

            largeGearbox.SetActive(false);
            smallGearbox.SetActive(false);
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

        GlobalSettings.UiWalletLarge = largeWallet.activeInHierarchy;
        GlobalSettings.SaveSettings();
        AudioManager.PlaySoundEffect("metalClick");
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

        GlobalSettings.UiGearboxLarge = largeGearbox.activeInHierarchy;
        GlobalSettings.SaveSettings();
        AudioManager.PlaySoundEffect("metalClick");
    }
}
