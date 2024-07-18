using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTextSpeedController: MonoBehaviour
{
    public Button btnSpeed;
    public TextMeshProUGUI speedText;

    private void Start()
    {
        btnSpeed.onClick.AddListener(() =>
        {
            GlobalSettings.TextSpeed++;

            if (GlobalSettings.TextSpeed > 4 || GlobalSettings.TextSpeed == 0)
            {
                GlobalSettings.TextSpeed = 1;
            }

            GlobalSettings.SaveSettings();

            speedText.text = GetSpeedText();
        });
    }
    private void OnEnable()
    {
        speedText.text = GetSpeedText();
    }

    string GetSpeedText()
    {
        if (GlobalSettings.TextSpeed == 1)
        {
            return ">";
        }
        else if (GlobalSettings.TextSpeed == 2)
        {
            return ">>";
        }
        else if (GlobalSettings.TextSpeed == 3)
        {
            return ">>>";
        }
        else
        {
            return ">>>>";
        }
    }
}