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
    public float durationTimer = 0;
    float queueDelay = 1.0f;
    float alertDuration = 5f;

    void Start()
    {
        logAlert = GetComponent<LogAlert>();
        queuedItemAlerts = new();
        queuedTextAlerts = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing())
        {
            queueTimer += Time.deltaTime;

            if (queuedTextAlerts.Count > 0 && queueTimer >= queueDelay)
            {
                if (activeAlerts.Count < 10)
                {
                    PrintTextAlert(queuedTextAlerts[0]);
                }
                queueTimer = 0;
            }
            else if (queuedItemAlerts.Count > 0 && queueTimer >= queueDelay)
            {
                if (activeAlerts.Count < 10)
                {
                    PrintItemAlert(queuedItemAlerts[0]);
                }
                queueTimer = 0;
            }
            else if (queuedItemAlerts.Count == 0)
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

    void PrintItemAlert(ItemIntPair entry)
    {
        //Debug.Log("Attempting to print item to log");
        queuedItemAlerts.RemoveAt(0);
        var alert = BoxFactory.CreateItemRow(entry.item, entry.amount);
        AddBehaviour(alert);
        alert.transform.SetParent(alertContainer.transform, false);
        activeAlerts.Add(alert);
    }

    void PrintTextAlert(string alertText)
    {
        logAlert.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
        //Debug.Log("Attempting to print text to log");
        queuedTextAlerts.RemoveAt(0);
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
        activeAlerts.Remove(prefab);
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
    }

    public static void QueueAffectionAlert(Character character, int amount)
    {
        if (character != null && !character.excludeFromPrint)
        {
            if (amount < 0)
            {
                QueueTextAlert($"{character.NamePlate()} disapproves.");
            }
            else if (amount == 1)
            {
                QueueTextAlert($"{character.NamePlate()} approves.");
            }
            else if (amount == 2)
            {
                QueueTextAlert($"{character.NamePlate()} likes that.");
            }
            else if (amount == 3)
            {
                QueueTextAlert($"{character.NamePlate()} appreciates that.");
            }
            else if (amount == 4)
            {
                QueueTextAlert($"{character.NamePlate()} loves that.");
            }
            else if (amount >= 5)
            {
                QueueTextAlert($"{character.NamePlate()} is extatic.");
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
