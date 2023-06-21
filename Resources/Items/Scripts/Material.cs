using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material Item", menuName = "Scriptable Object/Item/Material")]

public class Material : Item
{

    public void Awake()
    {
        type = ItemType.Material;
    }
}