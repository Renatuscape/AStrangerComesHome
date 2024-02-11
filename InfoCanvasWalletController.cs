using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoCanvasWalletController : MonoBehaviour
{
    public int guilders;
    public GameObject guilderObject;

    public int crowns;
    public GameObject crownObject;

    public int shillings;
    public GameObject shillingObject;

    public int hellers;
    public GameObject hellerObject;

    public int total;
    public GameObject totalObject;

    public GameObject smallWalletTotal;

    float timer = 0.0f;
    float tick = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TransientDataScript.IsTimeFlowing() && TransientDataScript.GameState != GameState.BankMenu)
        {
            timer += Time.deltaTime;

            if (timer > tick)
            {
                UpdateWallet();
                timer = 0.0f;
            }
        }
    }

    void UpdateWallet()
    {
        var playerGuilders = Player.GetCount("MIS003-RAR-NN", name);
        var playerCrowns = Player.GetCount("MIS002-UNC-NN", name);
        var playerShillings = Player.GetCount("MIS001-COM-NN", name);
        var playerHellers = Player.GetCount("MIS000-JUN-NN", name);
        var playerTotal = MoneyExchange.GetPlayerMoney();

        if (GlobalSettings.uiWalletLarge)
        {
            if (guilders != playerGuilders)
            {
                StartCoroutine(UpdateMoney(guilders, playerGuilders, guilderObject));
                guilders = playerGuilders;
            }
            if (crowns != playerCrowns)
            {
                StartCoroutine(UpdateMoney(crowns, playerCrowns, crownObject));
                crowns = playerCrowns;
            }
            if (shillings != playerShillings)
            {
                StartCoroutine(UpdateMoney(shillings, playerShillings, shillingObject));
                shillings = playerShillings;
            }
            if (hellers != playerHellers)
            {
                StartCoroutine(UpdateMoney(hellers, playerHellers, hellerObject));
                hellers = playerHellers;
            }
        }

        if (total != playerTotal)
        {
            StartCoroutine(UpdateMoney(total, playerTotal, totalObject));
            StartCoroutine(UpdateMoney(total, playerTotal, smallWalletTotal));
            total = playerTotal;
        }
    }

    IEnumerator UpdateMoney(int oldSum, int newSum, GameObject container)
    {
        var text = container.GetComponentInChildren<TextMeshProUGUI>();
        int displaySum;

        if (oldSum > newSum)
        {
            displaySum = newSum - 20;
        }
        else
        {
            displaySum = newSum + 20;
        }

        while (displaySum != newSum)
        {
            if (displaySum < newSum)
            {
                displaySum++;
            }
            else
            {
                displaySum--;
            }

            text.text = displaySum.ToString();
            StartCoroutine(TextJump(container));

            yield return new WaitForSeconds(0.02f);

        }
        AudioManager.PlayUISound("handleCoins");
    }

    IEnumerator TextJump (GameObject container)
    {
        var originalPosition = container.transform.localPosition;
        container.transform.localPosition = new Vector3(container.transform.localPosition.x + 1, container.transform.localPosition.y + 1, container.transform.localPosition.z);
        yield return new WaitForSeconds(0.1f);
        container.transform.localPosition = originalPosition;
    }
}
