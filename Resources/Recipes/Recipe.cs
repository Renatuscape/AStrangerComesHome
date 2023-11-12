using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptable Object/Recipe")]
public class Recipe : MotherObject
{
    public Item createsItem;
    public bool isDiscoverable;
    public int maxSynth = 50;
    public int manaDrainRate = 1;

    public Item ingredientA;
    public int ingredientAmountA;
    public Item ingredientB;
    public int ingredientAmountB;
    public Item ingredientC;
    public int ingredientAmountC;

    private void Awake()
    {
        maxValue = 1;
    }
    public void UpdatePrintName()
    {
        if (createsItem != null)
            name = createsItem.name;
        else
            Debug.Log(name + " recipe has a null reference in createsItem variable.");
    }
}
