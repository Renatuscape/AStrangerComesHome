using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoMapCanvas : MonoBehaviour
{
    Canvas canvas;
    public Button tutorialPanel;
    public Button infoPanel;
    public Button btnOpenInfoPanel;
    public Location markerLocation;
    public TextMeshProUGUI locationName;
    public TextMeshProUGUI locationDescription;
    public TextMeshProUGUI regionTitle;
    void Start()
    {
        canvas = GetComponent<Canvas>();

        btnOpenInfoPanel.onClick.AddListener(() =>
        {
            if (markerLocation != null && !string.IsNullOrEmpty(markerLocation.objectID))
            {
                infoPanel.gameObject.SetActive(true);
            }
            else
            {
                tutorialPanel.gameObject.SetActive(true);
            }

            btnOpenInfoPanel.gameObject.SetActive(false);
        });
        infoPanel.onClick.AddListener(() =>
        {
            tutorialPanel.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(false);
            btnOpenInfoPanel.gameObject.SetActive(true);
        });
        tutorialPanel.onClick.AddListener(() =>
        {
            tutorialPanel.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(false);
            btnOpenInfoPanel.gameObject.SetActive(true);
        });

        tutorialPanel.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(false);
        btnOpenInfoPanel.gameObject.SetActive(false);

        if (TransientDataScript.GameState != GameState.MapMenu)
        {
            ToggleEnable(false);
        }
    }

    public void ToggleEnable(bool enable)
    {
        if (canvas != null)
        {
            canvas.enabled = enable;
        }
        else
        {
            canvas = GetComponent<Canvas>();
            canvas.enabled = enable;
        }

        regionTitle.text = TransientDataScript.transientData.currentRegion.name;
    }

    public void SetLocation(Location location)
    {
        markerLocation = location;

        if (location != null)
        {
            locationName.text = location.name;
            locationDescription.text = DialogueTagParser.ParseText(location.description);

            tutorialPanel.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(true);
            btnOpenInfoPanel.gameObject.SetActive(false);
        }
        else
        {
            tutorialPanel.gameObject.SetActive(false);
            infoPanel.gameObject.SetActive(false);
            btnOpenInfoPanel.gameObject.SetActive(true);
        }
    }

    public void ChangeRegion()
    {
        regionTitle.text = TransientDataScript.transientData.currentRegion.name;

        tutorialPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(false);
        btnOpenInfoPanel.gameObject.SetActive(true);
    }
}
