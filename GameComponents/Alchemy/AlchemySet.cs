using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemySet : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public SynthesiserType synthesiser;
    public string synthID = "none";

    // Start is called before the first frame update
    void Start()
    {
        if (synthesiser == SynthesiserType.Stella)
        {
            synthID = "SCR006";
        }
        else if (synthesiser == SynthesiserType.Capital)
        {
            synthID = "SCR007";
        }
        else if (synthesiser == SynthesiserType.Magus)
        {
            synthID = "SCR008";
        }
        else if (synthesiser == SynthesiserType.Home)
        {
            synthID = "SCR009";
        }
    }

    // Update is called once per frame
    bool CheckLevel()
    {
        if (synthID != "none")
        {
            if (Player.GetCount(synthID, name) > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void OnMouseDown()
    {
        if (CheckLevel())
        {
            if (TransientDataScript.GameState == GameState.Overworld)
            {
                if (alchemyMenu == null)
                {
                    alchemyMenu = TransientDataScript.gameManager.menuSystem.alchemyMenu;
                }


                if (alchemyMenu != null)
                {
                    alchemyMenu.isDebugging = false;
                    alchemyMenu.Initialise(synthesiser);
                }
            }
        }
        else
        {
            LogAlert.QueueTextAlert("I don't know how to use an alchemy set.");
        }
    }
}
