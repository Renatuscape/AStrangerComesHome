using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyButtonManager : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public GameObject confirmMenu;
    public GameObject buttonContainer;
    public TextMeshProUGUI confirmWarning;
    public Button cancelConfirmationButton;
    public Button confirmButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button createButton;
    public Button leaveButton;
    public Button cancelButton;
    public Button clearButton;
    public Button claimButton;

    private void OnEnable()
    {
        confirmMenu.SetActive(false);
        buttonContainer.SetActive(true);

        clearButton.onClick.AddListener(ClearTable);
        pauseButton.onClick.AddListener(Pause);
        resumeButton.onClick.AddListener(Resume);
        createButton.onClick.AddListener(Create);
        leaveButton.onClick.AddListener(Leave);
        cancelButton.onClick.AddListener(Cancel);
        claimButton.onClick.AddListener(Claim);
        cancelConfirmationButton.onClick.AddListener(CancelConfirmation);

        CheckButtons();
    }

    private void OnDisable()
    {
        confirmMenu.SetActive(false);

        clearButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
        createButton.onClick.RemoveAllListeners();
        leaveButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        claimButton.onClick.RemoveAllListeners();
        cancelConfirmationButton.onClick.RemoveAllListeners();
    }

    public void CheckButtons()
    {

        if (AlchemyMenu.synthData != null && AlchemyMenu.synthData.synthRecipe != null && AlchemyMenu.synthData.isSynthActive && AlchemyMenu.synthData.progressSynth >= AlchemyMenu.synthData.synthRecipe.workload)
        {
            claimButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(false);
            createButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
        }
        else
        {
            claimButton.gameObject.SetActive(false);

            if (AlchemyMenu.synthData.isSynthActive)
            {
                createButton.gameObject.SetActive(false);
                clearButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(true);
            }
            else
            {
                createButton.gameObject.SetActive(true);
                clearButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
                resumeButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
            }

            if (AlchemyMenu.synthData.isSynthActive && AlchemyMenu.synthData.isSynthPaused)
            {
                resumeButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
            }
            else if (AlchemyMenu.synthData.isSynthActive && !AlchemyMenu.synthData.isSynthPaused)
            {
                resumeButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);
            }
        }
    }

    public void ClearTable()
    {
        List<AlchemyIngredient> allDraggables = new();

        foreach (var inventoryItem in alchemyMenu.inventoryItems)
        {
            allDraggables.AddRange(inventoryItem.alchemyIngredients);
        }

        for (int i = allDraggables.Count - 1; i >= 0; i--)
        {
            float time = 0.2f * i;
            if (time > 1)
            {
                time = time / 2;
            }
            if (time > 2)
            {
                time = 0.9f;
            }
            if (time < 0.2f)
            {
                time = 0.2f;
            }

            StartCoroutine(ClearTableInStyle(allDraggables[i], time));
        }

        IEnumerator ClearTableInStyle(AlchemyIngredient ingredient, float duration)
        {
            ingredient.transform.SetParent(alchemyMenu.dragParent.transform);
            Vector3 targetLocation = alchemyMenu.pageinatedContainer.gameObject.transform.position;
            Vector3 startPosition = ingredient.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                ingredient.transform.position = Vector3.Lerp(startPosition, targetLocation, t);
                yield return null;
            }

            // Ensure the object is exactly at the target position
            ingredient.transform.position = targetLocation;
            ingredient.ReturnToInventory(true);
        }
    }

    public void Pause()
    {
        if (AlchemyMenu.synthData.isSynthActive)
        {
            AlchemyMenu.synthData.isSynthPaused = true;
        }

        CheckButtons();
    }

    public void Resume()
    {
        if (AlchemyMenu.synthData.isSynthActive)
        {
            AlchemyMenu.synthData.isSynthPaused = false;
        }

        CheckButtons();
    }

    public void Leave()
    {
        alchemyMenu.gameObject.SetActive(false);
        TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
    }

    public void Claim()
    {
        alchemyMenu.HandleClaim();
        CheckButtons();
    }

    public void Create() // Opens confirmation menu
    {
        confirmMenu.gameObject.SetActive(true);
        buttonContainer.SetActive(false);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(ConfirmCreate);
        confirmWarning.text = "Begin synthesis?\nIngredients cannot be recovered once the process begins.";
    }

    public void Cancel() // Opens confirmation menu
    {
        confirmMenu.gameObject.SetActive(true);
        buttonContainer.SetActive(false);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(ConfirmCancel);
        confirmWarning.text = "Cancel synthesis?\nAll ingredients and progress will be lost.";
    }

    public void CancelConfirmation() // Closes confirmation menu
    {
        confirmMenu.gameObject.SetActive(false);
        buttonContainer.SetActive(true);

        CheckButtons();
    }

    public void ConfirmCancel() // Actually cancels the synthesis
    {
        confirmMenu.gameObject.SetActive(false);
        buttonContainer.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
        AlchemyMenu.synthData.isSynthActive = false;
        AlchemyMenu.synthData.progressSynth = 0;

        //Cancel animation and resetting of components goes here

        CheckButtons();
    }

    public void ConfirmCreate() // Actually starts the synthesis
    {
        confirmMenu.gameObject.SetActive(false);
        buttonContainer.SetActive(true);

        alchemyMenu.HandleCreate();

        // Creation animation and deletion of prefabs goes here

        CheckButtons();
    }
}