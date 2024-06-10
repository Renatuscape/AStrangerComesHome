using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyYieldManager : MonoBehaviour
{
    public GameObject prefabContainer;
    public List<GameObject> yieldPrefabs = new();
    public bool isYieldPrinted;

    public void Setup(SynthesiserData synthData)
    {
        isYieldPrinted = false;

        if (AlchemyMenu.synthData.isSynthActive && AlchemyMenu.synthData.progressSynth >= AlchemyMenu.synthData.synthRecipe.workload)
        {
            prefabContainer.SetActive(true);
            CheckCompletion();

        }
        else
        {
            prefabContainer.SetActive(false);
        }
    }

    public void CheckCompletion()
    {
        if (!isYieldPrinted && AlchemyMenu.synthData.isSynthActive && AlchemyMenu.synthData.progressSynth >= AlchemyMenu.synthData.synthRecipe.workload)
        {
            prefabContainer.SetActive(true);

            if (yieldPrefabs.Count == 0)
            {
                isYieldPrinted = true;
                PrintYield();
            }

        }
    }

    public void PrintYield()
    {
        float timer = 0.1f;

        foreach (var yield in AlchemyMenu.synthData.synthRecipe.yield)
        {
            for (int i = 0; i < yield.amount; i++)
            {
                timer += i * 0.01f;

                StartCoroutine(PrintYieldIcon(yield, timer));
            }
        }
    }

    IEnumerator PrintYieldIcon(IdIntPair yield, float timer)
    {
        yield return new WaitForSeconds(timer);

        var yieldPrefab = BoxFactory.CreateItemIcon(Items.FindByID(yield.objectID), false, 96).gameObject;
        yieldPrefab.transform.SetParent(prefabContainer.transform, false);
        yieldPrefabs.Add(yieldPrefab);

        var scaleScript = yieldPrefab.GetComponent<Anim_ScaleOnEnable>();
        scaleScript.startScale = new Vector3 (0, 0, 1f);
    }

    public void Clear()
    {
        foreach (GameObject go in yieldPrefabs)
        {
            go.GetComponent<ItemIconData>().Return("AlchemyYieldManager on Clear");
        }

        yieldPrefabs.Clear();
        isYieldPrinted = false;
    }

    private void OnDisable()
    {
        Clear();
    }
}
