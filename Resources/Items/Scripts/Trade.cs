using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trade Item", menuName = "Scriptable Object/Item/Trade")]

public class Trade : Item
{
    public void Awake()
    {
        type = ItemType.Trade;
    }
}