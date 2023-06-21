using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PushAlertManager : MonoBehaviour
{
    public DataManagerScript dataManager;

    public TextMeshProUGUI alertText;
    public TextMeshProUGUI logbookText;
    public bool isDisplayingAlert;
    public List<string> alertQueue;
    public Vector3 originPosition;

    [TextArea(15, 20)]
    public string logbook;

    float textAlpha;
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();

        textAlpha = 1;
        alertText.text = " ";
        isDisplayingAlert = false;
        originPosition = alertText.transform.localPosition;
        logbook = "LOGBOOK";
        logbookText.text = "";
    }

    public void QueueAlert(string alertContent)
    {
        alertQueue.Add(alertContent);

        var currentHour = Mathf.Round(dataManager.timeOfDay * 24);
        logbook += "\nH" + currentHour + ": " + alertContent;
        logbookText.text = logbook;
    }

    void Update()
    {
        if (alertQueue.Count > 0 && isDisplayingAlert == false)
        {
            StartCoroutine("PushAlert", alertQueue[0]);
            isDisplayingAlert = true;
        }
    }
    IEnumerator PushAlert(string alertContent)
    {
        alertText.transform.localPosition = originPosition;
        textAlpha = 1;
        alertText.text = alertContent;
        alertText.color = new Color(1, 1, 1, textAlpha);
        alertQueue.RemoveAt(0);

        yield return new WaitForSeconds(3);
        StartCoroutine("FadeFloatText");
    }

    IEnumerator FadeFloatText()
    {
        yield return new WaitForSeconds(0.005f);
        textAlpha -= 0.02f;
        alertText.color = new Color(1, 1, 1, textAlpha);
        alertText.transform.localPosition = new Vector3(alertText.transform.localPosition.x, alertText.transform.localPosition.y + 0.1f, alertText.transform.localPosition.z);
        if (textAlpha <= 0)
        {
            alertText.text = " ";
            isDisplayingAlert = false;
        }
        else
            StartCoroutine("FadeFloatText");
    }

    public void ClickAwayAlert()
    {
        StopAllCoroutines();
        textAlpha = 0f;
        alertText.color = new Color(1, 1, 1, textAlpha);
        alertText.text = " ";
        isDisplayingAlert = false;

        if (alertQueue.Count > 0)
        {
            StartCoroutine("PushAlert", alertQueue[0]);
        }
    }
}
