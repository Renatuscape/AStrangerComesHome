using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyYieldManager : MonoBehaviour
{
    public SynthesiserData synthData;
    public GameObject prefabContainer;
    public List<GameObject> yieldPrefabs = new();
    public bool isYieldPrinted;

    public void Setup(SynthesiserData synthData)
    {
        isYieldPrinted = false;
        this.synthData = synthData;

        if (synthData.isSynthActive && synthData.progressSynth >= synthData.synthRecipe.workload)
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
        if (!isYieldPrinted && synthData.isSynthActive && synthData.progressSynth >= synthData.synthRecipe.workload)
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

        foreach (var yield in synthData.synthRecipe.yield)
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

        //yieldPrefab.AddComponent<Anim_BobLoop>();
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
