using System.Collections;
using UnityEngine;

public class InteractableNodeText : MonoBehaviour
{
    public enum TextType
    {
        LogAlert,
        PushAlert,
        Book
    }

    public TextType type;
    public string textTag;
    public string loadedText;
    void Start()
    {
        
    }
}
