using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Catalyst Item", menuName = "Scriptable Object/Item/Catalyst")]

public class Catalyst : Item
{

    public void Awake()
    {
        type = ItemType.Catalyst;
    }
}