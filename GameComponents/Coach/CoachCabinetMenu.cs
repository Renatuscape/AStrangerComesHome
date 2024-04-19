using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CoachCabinetMenu : MonoBehaviour
{
    public GameObject menuContainer;
    public Canvas cabinetCanvas;
    public GameObject miniSynthPrefab;
    public List<GameObject> miniSynthInstances;

    public Button btnClose;
    int coachSynthesisersUnlocked = 0;

    private void Start()
    {
        btnClose.onClick.AddListener(() => CloseCabinet());
        cabinetCanvas.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView != CameraView.Normal)
        {
            TransientDataScript.SetGameState(GameState.AlchemyMenu, name, gameObject);

            coachSynthesisersUnlocked = Player.GetCount("SCR004-SCR-NN", name);

            if (coachSynthesisersUnlocked > 0 && !cabinetCanvas.gameObject.activeInHierarchy)
            {
                OpenCabinet();
            }
            else
            {
                TransientDataScript.PushAlert("Nothing in here yet.");
            }
        }
    }

    public void OpenCabinet()
    {
        cabinetCanvas.gameObject.SetActive(true);

        if (coachSynthesisersUnlocked > 0)
        {
            var synthesisers = TransientDataScript.gameManager.dataManager.alchemySynthesisers;
            var coachSynthesisers = synthesisers.Where(s => s.synthesiserID.ToLower().Contains("coach")).ToList();

            if (coachSynthesisers != null && coachSynthesisers.Count == coachSynthesisersUnlocked)
            {
                Debug.Log("Synthesisers found matched synthesisers unlocked.");
            }
            else if (coachSynthesisers != null && coachSynthesisers.Count < coachSynthesisersUnlocked)
            {
                while (coachSynthesisers.Count < coachSynthesisersUnlocked)
                {
                    SynthesiserData newSynth = new SynthesiserData() { synthesiserID = $"Coach{coachSynthesisers.Count + 1}", consumesMana = true };
                    synthesisers.Add(newSynth);
                    coachSynthesisers.Add(newSynth);
                }
            }
            else if (coachSynthesisers == null || coachSynthesisers.Count == 0)
            {
                Debug.Log($"No coach synthesisers found. Creating {coachSynthesisersUnlocked} new coach synthesisers.");

                for (int i = 0; i < coachSynthesisersUnlocked; i++)
                {
                    SynthesiserData newSynth = new SynthesiserData() { synthesiserID = $"Coach{i + 1}", consumesMana = true };
                    synthesisers.Add(newSynth);
                    coachSynthesisers.Add(newSynth);

                    Debug.Log($"Synthesiser with ID Coach{i + 1} created.");
                }
            }

            if (coachSynthesisers.Count > 3)
            {
                menuContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(1250, 790);
            }
            else
            {
                menuContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 790);
            }

            PrintSynthesisers(coachSynthesisers);
        }
        else
        {
            Debug.Log("No coach synthesisers unlocked.");
        }
    }

    public void CloseCabinet(bool returnToOverworld = true)
    {
        foreach (GameObject prefab in miniSynthInstances)
        {
            Destroy(prefab);
        }

        miniSynthInstances.Clear();
        cabinetCanvas.gameObject.SetActive(false);

        if (returnToOverworld)
        {
            TransientDataScript.SetGameState(GameState.Overworld, name, gameObject);
        }
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
