using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoachCabinetMenu : MonoBehaviour
{
    public GameObject menuContainer;
    public Canvas cabinetCanvas;
    public GameObject miniSynthPrefab;
    public List<GameObject> miniSynthInstances;

    public void OpenCabinet()
    {
        cabinetCanvas.gameObject.SetActive(true);
        int coachSynthesisersUnlocked = Player.GetCount("SCR004-SCR-NN", name);

        if (coachSynthesisersUnlocked > 0)
        {
            var synthesisers = TransientDataCalls.gameManager.dataManager.alchemySynthesisers;
            var coachSynthesisers = synthesisers.Where(s => s.synthesiserID.ToLower().Contains("coach")).ToList();

            if (coachSynthesisers != null && coachSynthesisers.Count == coachSynthesisersUnlocked)
            {
                Debug.Log("Synthesisers found matched synthesisers unlocked.");
            }
            else if (coachSynthesisers != null && coachSynthesisers.Count < coachSynthesisersUnlocked)
            {
                while (coachSynthesisers.Count < coachSynthesisersUnlocked)
                {
                    SynthesiserData newSynth = new SynthesiserData() { synthesiserID = $"Coach{coachSynthesisers.Count + 1}" };
                    synthesisers.Add(newSynth);
                    coachSynthesisers.Add(newSynth);
                }
            }
            else if (coachSynthesisers == null || coachSynthesisers.Count == 0)
            {
                Debug.Log($"No coach synthesisers found. Creating {coachSynthesisersUnlocked} new coach synthesisers.");

                for (int i = 0; i < coachSynthesisersUnlocked; i++)
                {
                    SynthesiserData newSynth = new SynthesiserData() { synthesiserID = $"Coach{i + 1}" };
                    synthesisers.Add(newSynth);
                    coachSynthesisers.Add(newSynth);

                    Debug.Log($"Synthesiser with ID Coach{i + 1} created.");
                }
            }

            PrintSynthesisers(coachSynthesisers);
        }
        else
        {
            Debug.Log("No coach synthesisers unlocked.");
        }
    }

    public void CloseCabinet()
    {
        foreach (GameObject prefab in miniSynthInstances)
        {
            Destroy(prefab);
        }

        miniSynthInstances.Clear();
        cabinetCanvas.gameObject.SetActive(true);
    }

    void PrintSynthesisers(List<SynthesiserData> synthesisers)
    {
        if (synthesisers != null && synthesisers.Count > 0)
        {
            foreach (SynthesiserData synthData in synthesisers)
            {
                GameObject miniSynth = Instantiate(miniSynthPrefab);

                miniSynth.transform.SetParent(menuContainer.transform, false);

                var script = miniSynth.GetComponent<AlchemyMiniSynth>();
                script.Initialise(synthData, this);

                miniSynthInstances.Add(miniSynth);
                miniSynth.name = synthData.synthesiserID;
            }
        }
    }
}
