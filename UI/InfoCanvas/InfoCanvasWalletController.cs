using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoCanvasWalletController : MonoBehaviour
{
    public bool disregardState = false;
    public bool includeName = false;
    public bool ignoreSmallWallet = false;

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
    float tick = 1f;


    float cooldownTimer;
    bool effectAudioCooldown = true;
    private void OnEnable()
    {
        cooldownTimer = 0;
        effectAudioCooldown = true;
        StartCoroutine(EffectAudioCooldown());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        effectAudioCooldown = true;
    }

    IEnumerator EffectAudioCooldown()
    {

        while (effectAudioCooldown)
        {
            yield return null;
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > 3)
            {
                effectAudioCooldown = false;
            }
        }
    }
    void Update()
    {
        if (disregardState || (TransientDataScript.IsTimeFlowing() && TransientDataScript.GameState != GameState.BankMenu))
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
        var playerGuilders = Player.GetCount("MIS003", name);
        var playerCrowns = Player.GetCount("MIS002", name);
        var playerShillings = Player.GetCount("MIS001", name);
        var playerHellers = Player.GetCount("MIS000", name);
        var playerTotal = MoneyExchange.GetPlayerMoney();

        if (GlobalSettings.UiWalletLarge)
        {
            if (guilders != playerGuilders)
            {
                StartCoroutine(UpdateMoney(Currency.Guilder, guilders, playerGuilders, guilderObject));
                guilders = playerGuilders;
            }
            if (crowns != playerCrowns)
            {
                StartCoroutine(UpdateMoney(Currency.Crown, crowns, playerCrowns, crownObject));
                crowns = playerCrowns;
            }
            if (shillings != playerShillings)
            {
                StartCoroutine(UpdateMoney(Currency.Shilling, shillings, playerShillings, shillingObject));
                shillings = playerShillings;
            }
            if (hellers != playerHellers)
            {
                StartCoroutine(UpdateMoney(Currency.Heller, hellers, playerHellers, hellerObject));
                hellers = playerHellers;
            }
        }

        if (total != playerTotal)
        {
            StartCoroutine(UpdateMoney(Currency.Total, total, playerTotal, totalObject));

            if (!ignoreSmallWallet)
            {
                StartCoroutine(UpdateMoney(Currency.Total, total, playerTotal, smallWalletTotal));
            }

            total = playerTotal;
        }
    }

    IEnumerator UpdateMoney(Currency currency, int oldSum, int newSum, GameObject container)
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

        var startPosition = container.transform.localPosition;
        var extra = "";

        if (includeName)
        {
            if (currency != Currency.Total)
            {
                extra = currency + (newSum > 1 ? "s: " : ": ");
            }
            else
            {
                extra = "Total: ";
            }
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

            text.text = extra + displaySum.ToString();

            StartCoroutine(TextJump(container));

            yield return new WaitForSeconds(0.02f);

        }

        yield return new WaitForSeconds(0.1f);
        container.transform.localPosition = startPosition;

        if (!effectAudioCooldown)
        {
            AudioManager.PlaySoundEffect("handleCoins");
        }
    }

    IEnumerator TextJump (GameObject container)
    {
        container.transform.localPosition = new Vector3(container.transform.localPosition.x + 1, container.transform.localPosition.y + 1, container.transform.localPosition.z);
        yield return new WaitForSeconds(0.1f);
        container.transform.localPosition = new Vector3(container.transform.localPosition.x - 1, container.transform.localPosition.y - 1, container.transform.localPosition.z);
    }
}
