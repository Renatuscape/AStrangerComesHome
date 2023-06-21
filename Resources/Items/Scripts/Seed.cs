using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seed Item", menuName = "Scriptable Object/Item/Seed")]

public class Seed : Item
{
    public Plant growsPlant;

    public int maxGrowth = 100;
    public int health = 1;
    public int yield = 1;

    public Sprite stage1;
    public Sprite stage2;
    public Sprite stage3;

    public void Awake()
    {
        type = ItemType.Seed;
    }
}