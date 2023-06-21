using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Treasure Item", menuName = "Scriptable Object/Item/Treasure")]

public class Treasure : Item
{
    public void Awake()
    {
        type = ItemType.Treasure;
        rarity = ItemRarity.Rare;
    }
}