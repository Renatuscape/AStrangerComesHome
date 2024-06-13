using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class AlchemySelectedIngredients : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public GameObject materialContainer;
    public GameObject infusionContainer;

    private void Update()
    {
        int materialCount = materialContainer.transform.childCount;
        int infusionCount = infusionContainer.transform.childCount;

        materialContainer.SetActive(materialCount > 1); //Account for title
        infusionContainer.SetActive(infusionCount > 1); //Account for title
        tipText.gameObject.SetActive(materialCount == 1 && infusionCount == 1);
    }
}
