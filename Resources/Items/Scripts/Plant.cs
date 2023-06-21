using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Item", menuName = "Scriptable Object/Item/Plant")]

public class Plant : Item
{
    public void Awake()
    {
        type = ItemType.Plant;
    }
}