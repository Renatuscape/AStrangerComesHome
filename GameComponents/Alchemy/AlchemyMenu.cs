using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SynthesiserType
{
    StellaPoint,
    Capital,
    Magus,
    PlayerHouse,
    Coach
}
public class AlchemyMenu : MonoBehaviour
{
    public IdIntPair selectedCatalyst;
    public IdIntPair selectedPlant;
    public List<IdIntPair> selectedMaterials;

    public List<GameObject> tablePrefabs;
    public List<GameObject> infusionPrefabs;

    public GameObject tableContainer;
    public GameObject bowlContainer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
