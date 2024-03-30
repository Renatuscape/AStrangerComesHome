using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoachManager : MonoBehaviour
{
    public GameObject coachExterior;
    void Start()
    {
        coachExterior.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.GameState == GameState.Overworld)
        {
            if (GlobalSettings.AlwaysHideCoachExterior && coachExterior.activeInHierarchy)
            {
                coachExterior.SetActive(false);
            }
            else if (!GlobalSettings.AlwaysHideCoachExterior && !coachExterior.activeInHierarchy)
            {
                coachExterior.SetActive(true);
            }
        }
    }
}
