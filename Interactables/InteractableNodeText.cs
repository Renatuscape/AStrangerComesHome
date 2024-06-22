using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableNodeText : MonoBehaviour
{
    public enum TextType
    {
        LogAlert,
        PushAlert,
        Book
    }

    public string nodeID;
    public TextType type;
    public string textTag;
    public int cooldown;
    public bool exactCooldown;
    public bool saveToPlayer;
    public bool disableRespawn;
    
    public string loadedText;
    public List<IdIntPair> rewards;
    public RequirementPackage checks;
    void Start()
    {
        nodeID = "WorldNodeLoot_" + textTag + "_" + (disableRespawn ? "disableRespawn" : "allowRespawn") + (exactCooldown ? "_ECD#" : "_CD#") + cooldown; ;
    }
}