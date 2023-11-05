using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MotherObject : ScriptableObject    //A shared type for all scriptable objects that have to be saved
{
    public string printName; //REMOVE
    public int dataValue;
    public int maxValue = 99;
    public int basePrice;
    public ObjectType objectType;
    public ItemRarity rarity;
    public Sprite sprite;

    [TextArea(5, 20)]
    public string shortDescription;
    [TextArea(15, 20)]
    public string longDescription;
}
