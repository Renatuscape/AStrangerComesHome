using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemySet : MonoBehaviour
{
    public AlchemyMenu alchemyMenu;
    public SynthesiserType synthesiser;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        // Add check to ensure that the corresponding synthesiser is actually unlocked
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            if (alchemyMenu == null)
            {
                alchemyMenu = TransientDataCalls.gameManager.menuSystem.alchemyMenu;
            }


            if (alchemyMenu != null)
            {
                alchemyMenu.isDebugging = false;
                alchemyMenu.Initialise(synthesiser);
            }
        }
    }
}
