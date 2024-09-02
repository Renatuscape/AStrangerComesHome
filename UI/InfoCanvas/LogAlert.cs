using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogAlert : MonoBehaviour
{
    public static LogAlert logAlert;
    public GameObject alertContainer;
    public List<GameObject> activeAlerts;
    public List<ItemIntPair> queuedItemAlerts;
    public List<string> queuedTextAlerts;
    public float queueTimer = 0;
    float queueDelay = 1f;
    int maxActiveAlerts = 8;

    void Start()
    {
        logAlert = GetComponent<LogAlert>();
        queuedItemAlerts = new();
        queuedTextAlerts = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing() || TransientDataScript.GameState == GameState.CharacterCreation)
        {
            if (queuedItemAlerts.Count > 0 || queuedTextAlerts.Count > 0)
            {
                queueTimer += Time.deltaTime;
            }
            else if (activeAlerts.Count == 0)
            {
                queueTimer = 0.5f;
            }

            if (queueTimer >= queueDelay && queuedTextAlerts.Count > 0 )
            {
                if (activeAlerts.Count < maxActiveAlerts)
                {
                    PrintTextAlert(ConsolidateAndRemoveTextAlerts(queuedTextAlerts[0]));

                    if (activeAlerts.Count >= maxActiveAlerts)
                    {
                        DestroyAlertPrefab(activeAlerts[0]);
                    }
                }

                queueTimer = 0;
            }
            else if (queueTimer >= queueDelay && queuedItemAlerts.Count > 0)
            {
                if (activeAlerts.Count < maxActiveAlerts)
                {
                    PrintItemAlert(ConsolidateAndRemoveItemAlerts(queuedItemAlerts[0]));

                    if (activeAlerts.Count >= maxActiveAlerts)
                    {
                        DestroyAlertPrefab(activeAlerts[0]);
                    }
                }

                queueTimer = 0;
            }
        }
    }

    string ConsolidateAndRemoveTextAlerts (string reference)
    {
        string consolidatedAlert = reference;
        List<string> alertsToRemove = new();
        int repeats = 0;

        foreach (var queuedItem in queuedTextAlerts)
        {
            if (queuedItem == reference)
            {
                repeats++;
                alertsToRemove.Add(queuedItem);
            }
        }

        Debug.Log("Consolidating alerts: " + repeats);

        foreach (var alert in alertsToRemove)
        {
            queuedTextAlerts.Remove(alert);
        }

        if (repeats > 1)
        {
            consolidatedAlert += $" (x{repeats})";
        }

        return consolidatedAlert;
    }

    ItemIntPair ConsolidateAndRemoveItemAlerts(ItemIntPair reference)
    {
        ItemIntPair consolidatedAlert = new() { item = reference.item};
        List<ItemIntPair> alertsToRemove = new();

        foreach (var queuedItem in queuedItemAlerts)
        {
            if (queuedItem.item.objectID == reference.item.objectID)
            {
                alertsToRemove.Add(queuedItem);
                consolidatedAlert.amount += queuedItem.amount;
            }
        }

        Debug.Log("Consolidating alerts: " + queuedItemAlerts.Count);

        foreach (var alert in alertsToRemove)
        {
            queuedItemAlerts.Remove(alert);
        }

        return consolidatedAlert;
    }
    void PrintItemAlert(ItemIntPair entry)
    {
        var alert = BoxFactory.CreateItemRow(entry.item, entry.amount);
        AddBehaviour(alert);
        alert.transform.SetParent(alertContainer.transform, false);
        activeAlerts.Add(alert);
    }

    void PrintTextAlert(string alertText)
    {
        logAlert.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
        //Debug.Log("Attempting to print text to log");
        //queuedTextAlerts.RemoveAt(0);
        var alert = BoxFactory.CreateButton(alertText, 330);
        AddBehaviour(alert);
        alert.transform.SetParent(alertContainer.transform, false);
        activeAlerts.Add(alert);

        logAlert.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
        logAlert.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
        logAlert.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }

    void AddBehaviour(GameObject prefab)
    {
        prefab.GetComponent<Button>().onClick.AddListener(() => DestroyAlertPrefab(prefab));
        prefab.AddComponent<LogAlertPrefab>();

        if (prefab.GetComponent<Anim_ScaleOnEnable>() == null)
        {
            prefab.AddComponent<Anim_ScaleOnEnable>();
        }
        var animator = prefab.GetComponent<Anim_ScaleOnEnable>();
        animator.startScale = new Vector3(0.2f, 0.8f, 0);
        animator.endScale = new Vector3(1, 1, 1);
        animator.duration = 0.2f;
        animator.destroyOnEnd = false;
        animator.enabled = true;
    }

    public void DestroyAlertPrefab(GameObject prefab)
    {
        if (activeAlerts.Contains(prefab))
        {
            activeAlerts.Remove(prefab);
        }
        Destroy(prefab);
    }

    public static void QueueItemAlert(Item item, int amount)
    {
        if (item == null)
        {
            Debug.Log("Item was null upon reaching Log Alert");
        }

        if (logAlert.queuedItemAlerts.Count >= 20)
        {
            Debug.Log($"Too many queued alerts. Alert for {item.name} ignored");
        }
        else if (item.type != ItemType.Script)
        {
            logAlert.queuedItemAlerts.Add(new ItemIntPair() { item = item, amount = amount });
        }
        else if (item.type == ItemType.Script && !string.IsNullOrEmpty(item.description))
        {
            QueueTextAlert(item.description);
        }
    }

    public static void QueueAffectionAlert(Character character, int amount)
    {
        if (character != null && !character.excludeFromPrint)
        {
            if (amount <= -5)
            {
                QueueTextAlert($"{character.NamePlate()} seethes with loathing.");
            }
            else if (amount == -4)
            {
                QueueTextAlert($"{character.NamePlate()} hates that.");
            }
            else if (amount == -3)
            {
                QueueTextAlert($"{character.NamePlate()} is disgusted.");
            }
            else if (amount == -2)
            {
                QueueTextAlert($"{character.NamePlate()}\ndislikes that a lot.");
            }
            else if (amount == -1)
            {
                QueueTextAlert($"{character.NamePlate()} disapproves.");
            }
            else if (amount == 1)
            {
                QueueTextAlert($"{character.NamePlate()} approves.");
            }
            else if (amount == 2)
            {
                QueueTextAlert($"{character.NamePlate()}\nlikes that a lot.");
            }
            else if (amount == 3)
            {
                QueueTextAlert($"{character.NamePlate()} is delighted.");
            }
            else if (amount == 4)
            {
                QueueTextAlert($"{character.NamePlate()} loves that!");
            }
            else if (amount >= 5)
            {
                QueueTextAlert($"{character.NamePlate()} is ecstatic!");
            }
        }
    }

    public static void QueueTextAlert(string alertText)
    {
        if (logAlert != null)
        {
            if (logAlert.queuedTextAlerts.Count < 25)
            {
                logAlert.queuedTextAlerts.Add(alertText);
            }
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

public class LogAlertPrefab: MonoBehaviour
{
    float timer;
    float alertDuration = 5;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= alertDuration)
        {
            LogAlert.logAlert.DestroyAlertPrefab(gameObject);
        }
    }
}