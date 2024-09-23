using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMenu : MonoBehaviour
{
    public static MemoryMenu instance;
    public GameObject menuCanvas;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Button btnPlay;
    public Button btnClose;

    public Memory selectedMemory;

    private void Start()
    {
        instance = this;
        Close();
        btnPlay.onClick.AddListener(PlayMemory);
        btnClose.onClick.AddListener(Close);
    }

    public static void PlayMemory(Memory memory, bool forceConfirm)
    {
        if (instance != null)
        {
            instance.selectedMemory = memory;

            if (forceConfirm || memory.contentWarning)
            {
                instance.SetUpText(memory);
                instance.menuCanvas.SetActive(true);
            }
            else
            {
                instance.PlayMemory();
            }
        }
    }

    void SetUpText(Memory memory)
    {
        if (!string.IsNullOrEmpty(memory.name))
        {
            title.text = memory.name;
        }
        else
        {
            instance.title.text = "???";
            Debug.LogWarning("Name missing from " + memory.objectID);
        }

        if (!string.IsNullOrEmpty(memory.description))
        {
            description.text = memory.description;
        }
        else if (memory.contentWarning)
        {
            description.text = "This memory has an unspecified content warning.";
            Debug.LogWarning("Content warning description missing from " + memory.objectID);
        }
        else
        {
            description.text = "";
        }
    }

    void PlayMemory()
    {
        if (selectedMemory != null)
        {
            TransientDataScript.gameManager.storySystem.StartMemory(selectedMemory);
            Close();
        }
    }

    void Close()
    {
        menuCanvas.SetActive(false);
        selectedMemory = null;
        description.text = "";
    }
}
