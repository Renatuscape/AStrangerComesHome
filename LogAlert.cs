using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class LogAlert : MonoBehaviour
{
    public static LogAlert logAlert;
    public GameObject alertContainer;
    public List<GameObject> activeAlerts;
    public List<ItemIntPair> queuedAlerts;
    public float queueDelay = 1.0f;
    public float alertDuration = 3f;
    public float queueTimer = 0;
    public float durationTimer = 0;
    void Start()
    {
        logAlert = GetComponent<LogAlert>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            queueTimer += Time.deltaTime;

            if (queuedAlerts.Count > 0 && queueTimer >= queueDelay)
            {
                if (activeAlerts.Count < 15)
                {
                    PrintAlert(queuedAlerts[0]);
                }
                queueTimer = 0;
            }
            else if (queuedAlerts.Count == 0)
            {
                queueTimer = queueDelay;
            }

            if (activeAlerts.Count > 0)
            {
                durationTimer += Time.deltaTime;

                if (durationTimer >= alertDuration)
                {
                    DestroyAlertPrefab(activeAlerts[0]);
                    durationTimer = 0;
                }
            }
        }
    }

    void PrintAlert(ItemIntPair entry)
    {
        Debug.Log("Attempting to print item to log");
        queuedAlerts.RemoveAt(0);
        var alert = BoxFactory.CreateItemRewardRow(entry.item, entry.count);
        alert.GetComponent<Button>().onClick.AddListener(() => DestroyAlertPrefab(alert));
        alert.transform.SetParent(alertContainer.transform, false);
        activeAlerts.Add(alert);
    }

    public void DestroyAlertPrefab(GameObject prefab)
    {
        activeAlerts.Remove(prefab);
        Destroy(prefab);
    }

    public static void QueueAlert(Item item, int amount)
    {
        if (item == null)
        {
            Debug.Log("Item was null upon reaching Log Alert");
        }

        if (logAlert.queuedAlerts.Count >= 25)
        {
            Debug.Log($"Too many queued alerts. Alert for {item.name} ignored");
        }
        else
        {
            logAlert.queuedAlerts.Add(new ItemIntPair() { item = item, count = amount });
        }
    }

    private void OnDisable()
    {
        foreach (GameObject prefab in activeAlerts)
        {
            Destroy(prefab);
        }

        activeAlerts = new();
    }
}
