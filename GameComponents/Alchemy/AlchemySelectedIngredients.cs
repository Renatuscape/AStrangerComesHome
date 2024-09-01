using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AlchemySelectedIngredients : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public GameObject materialContainer;
    public GameObject infusionContainer;
    int previousMats;
    int previousInfusion;
    private void Update()
    {
        int materialCount = materialContainer.transform.childCount;
        int infusionCount = infusionContainer.transform.childCount;

        if (materialCount !=  previousMats || infusionCount != previousInfusion)
        {
            StartCoroutine(ForceRefresh());
            previousMats = materialCount;
            previousInfusion = infusionCount;
        }

        materialContainer.SetActive(materialCount > 1); //Account for title
        infusionContainer.SetActive(infusionCount > 1); //Account for title
        tipText.gameObject.SetActive(materialCount == 1 && infusionCount == 1);
    }

    IEnumerator ForceRefresh()
    {
        Debug.Log("Force-refreshing alchemy ingredients");
        yield return null;
        Canvas.ForceUpdateCanvases();
        GetComponent<VerticalLayoutGroup>().enabled = false;
        yield return null;
        Canvas.ForceUpdateCanvases();
        GetComponent<VerticalLayoutGroup>().enabled = true;
        yield return null;
        Canvas.ForceUpdateCanvases();
    }
}
