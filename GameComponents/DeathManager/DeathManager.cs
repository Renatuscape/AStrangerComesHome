using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public DataManagerScript dataManager;
    public SaveLoadManager saveLoadManager;
    public CharacterCreator characterCreator;
    public AutoMap map;

    public GameObject killScreen;
    public Button btnContinue;

    private void Start()
    {
        killScreen.SetActive(false);
        btnContinue.onClick.AddListener(() => Continue());
    }
    public void InitiateDeath(Choice choice, bool isSuccess)
    {
        Debug.Log("Death Manager's InitiateDeath was called.");
        TransientDataScript.SetGameState(GameState.Death, "DeathManager", gameObject);
        if (Player.GetCount(StaticTags.Destruction, name) < 10)
        {
            Player.Add(StaticTags.Destruction);
        }

        Player.Add(StaticTags.PlayerDeath);

        btnContinue.interactable = false;
        var colourBlock = btnContinue.colors;
        var btnDisabledColour = colourBlock.normalColor;
        colourBlock.disabledColor = new Color(btnDisabledColour.r, btnDisabledColour.g, btnDisabledColour.b, 0);
        btnContinue.colors = colourBlock;

        killScreen.SetActive(true);
        AudioManager.ForceStopBackgroundMusic();
        AudioManager.PlaySoundEffect("bellCopperDeep");
        StartCoroutine(EnableContinue());
    }

    IEnumerator EnableContinue()
    {
        var colourBlock = btnContinue.colors;
        var btnDisabledColour = colourBlock.normalColor;
        var btnAlphaTarget = btnDisabledColour.a;

        float alpha = 0;
        float alphaStep = 0.01f;
        yield return new WaitForSeconds(3);

        while (alpha < 1)
        {
            if (alpha <= btnAlphaTarget)
            {
                colourBlock.disabledColor = new Color(btnDisabledColour.r, btnDisabledColour.g, btnDisabledColour.b, alpha);
                btnContinue.colors = colourBlock;
            }

            alpha+= alphaStep;
            alphaStep = alphaStep * 1.1f;
            yield return new WaitForSeconds(0.05f);
        }

        colourBlock.disabledColor = new Color(btnDisabledColour.r, btnDisabledColour.g, btnDisabledColour.b, btnAlphaTarget);
        btnContinue.colors = colourBlock;
        btnContinue.interactable = true;
    }

    public void Continue()
    {
        if (dataManager.randomiseOnDeath)
        {
            // Randomise sprite using presets
        }
        if (dataManager.saveOnDeath)
        {
            saveLoadManager.SaveGame(dataManager.saveID); // NEEDS THOROUGH TESTING WHEN CHANGING SAVE SLOTS
        }

        // Replace the following code with the death crawl minigame
        map.ChangeMap(Regions.FindByID("REGION1"));

        TransientDataScript.SetGameState(GameState.CharacterCreation, "DeathManager", gameObject);
        characterCreator.gameObject.SetActive(true);

        killScreen.SetActive(false);

        SpellCompendium.ForgetMemory();
    }
}
