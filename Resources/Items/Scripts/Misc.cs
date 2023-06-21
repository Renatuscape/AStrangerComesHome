using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Misc Item", menuName = "Scriptable Object/Item/Misc")]

public class Misc : Item
{
    public bool isQuestItem; //is tied to a specific quest
    public bool isUnique; //only one can exist at a time
    public void Awake()
    {
        type = ItemType.Misc;
    }
}