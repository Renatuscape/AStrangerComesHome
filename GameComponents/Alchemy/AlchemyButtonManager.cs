using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyButtonManager : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public SynthesiserData synthData;
    public SynthesiserType synthesiserType;
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

    private void OnEnable()
    {
        synthData = alchemyMenu.synthData;
        synthesiserType = alchemyMenu.synthesiserType;
        confirmMenu.SetActive(false);
        buttonContainer.SetActive(true);

        clearButton.onClick.AddListener(() => ClearTable());
        pauseButton.onClick.AddListener(() => Pause());
        resumeButton.onClick.AddListener(() => Resume());
        createButton.onClick.AddListener(() => Create());
        leaveButton.onClick.AddListener(() => Leave());
        cancelButton.onClick.AddListener(() => Cancel());
        cancelConfirmationButton.onClick.AddListener(() => CancelConfirmation());

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
        cancelConfirmationButton.onClick.RemoveAllListeners();
    }

    public void CheckButtons()
    {
        if (synthData.isSynthActive)
        {
            createButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(true);
        }
        else
        {
            createButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
        }

        if (synthData.isSynthActive && synthData.isSynthPaused)
        {
            resumeButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }
        else if (synthData.isSynthActive && !synthData.isSynthPaused)
        {
            resumeButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
    }

    public void ClearTable()
    {
        for (int i = alchemyMenu.draggableIngredientPrefabs.Count - 1; i >= 0; i--)
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
            StartCoroutine(ClearTableInStyle(alchemyMenu.draggableIngredientPrefabs[i], time));
        }
    }

    IEnumerator ClearTableInStyle(GameObject prefab, float duration)
    {
        prefab.transform.SetParent(alchemyMenu.dragParent.transform);
        Vector3 targetLocation = alchemyMenu.inventoryPage.transform.position;
        Vector3 startPosition = prefab.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            prefab.transform.position = Vector3.Lerp(startPosition, targetLocation, t);
            yield return null;
        }

        // Ensure the object is exactly at the target position
        prefab.transform.position = targetLocation;

        alchemyMenu.ReturnIngredientToInventory(prefab);
    }

    public void Pause()
    {
        if (synthData.isSynthActive)
        {
            synthData.isSynthPaused = true;
        }

        CheckButtons();
    }

    public void Resume()
    {
        if (synthData.isSynthActive)
        {
            synthData.isSynthPaused = false;
        }

        CheckButtons();
    }

    public void Leave()
    {
        gameObject.SetActive(false);
        TransientDataCalls.SetGameState(GameState.Overworld, name, gameObject);
    }

    public void Create() // Opens confirmation menu
    {
        confirmMenu.gameObject.SetActive(true);
        buttonContainer.SetActive(false);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => ConfirmCreate());
        confirmWarning.text = "Begin synthesis?\nIngredients cannot be recovered once the process begins.";
    }

    public void Cancel() // Opens confirmation menu
    {
        confirmMenu.gameObject.SetActive(true);
        buttonContainer.SetActive(false);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => ConfirmCancel());
        confirmWarning.text = "Cancel synthesis?\nAll ingredients and progress will be lost.";
    }

    public void CancelConfirmation() // Closes confirmation menu
    {
        confirmMenu.gameObject.SetActive(false);
        buttonContainer.SetActive(true);

        CheckButtons();
    }
    public void ConfirmCreate() // Actually starts the synthesis
    {
        confirmMenu.gameObject.SetActive(false);
        buttonContainer.SetActive(true);

        // Placeholder for testing
        synthData.isSynthActive = true;
        synthData.isSynthPaused = false;

        CheckButtons();
    }

    public void ConfirmCancel() // Actually cancels the synthesis
    {
        confirmMenu.gameObject.SetActive(false);
        buttonContainer.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
        synthData.isSynthActive = false;
        synthData.progressSynth = 0;

        CheckButtons();
    }
}