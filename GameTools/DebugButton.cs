using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    public void DebugItemsSkillsUpgrades()
    {
        Items.DebugAllItems();
        Skills.DebugAllSkills();
        Upgrades.DebugAllUpgrades();
    }
}
