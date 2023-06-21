using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SalvageBoxMechanics : MonoBehaviour
{
    DataManagerScript dataManager;
    TransientDataScript transientData;
    private SpriteRenderer sprite;
    private BoxCollider2D objectCollider;
    private Animator anim;

    private int salvageRarity;

    public float parallaxMultiplier;
    private float parallaxEffect;
    private float alphaSetting = 1;

    private int commonMoney;
    private int uncommonMoney;
    private int rareMoney;
    private int legendaryMoney;

    private void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManagerScript>();
        transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
    }
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        salvageRarity = Random.Range(0, 100);
        commonMoney = Random.Range(10, 30);
        uncommonMoney = Random.Range(35, 45);
        rareMoney = Random.Range(50, 80);
        legendaryMoney = Random.Range(100, 150);
    }

    void LateUpdate()
    {
        parallaxEffect = transientData.currentSpeed * parallaxMultiplier;

        sprite.color = new Color(1, 1, 1, alphaSetting);
        transform.position = new Vector2(transform.position.x + parallaxEffect, transform.position.y);

        if (transform.position.x <= -28 || transform.position.x >= 28)
        {
            Destroy(gameObject);
        }

        if (this.transform.position.x <= -28 || this.transform.position.x >= 28)
        {
            Destroy(gameObject);
        }

    }

    void OnMouseDown()
    {
        if (transientData.gameState == GameState.Overworld)
        {
            Destroy(objectCollider);
            anim.SetTrigger("Active");

            if (salvageRarity < 50)
            {
                dataManager.playerGold += commonMoney;
                transientData.PushAlert("You found some common salvage! " + commonMoney + " gold gained.");
            }
            if (salvageRarity > 49 && salvageRarity < 86)
            {
                dataManager.playerGold += uncommonMoney;
                transientData.PushAlert("You found some uncommon salvage! " + uncommonMoney + " gold gained.");
            }
            if (salvageRarity > 85 && salvageRarity < 97)
            {
                dataManager.playerGold += rareMoney;
                transientData.PushAlert("You found some rare salvage!! " + rareMoney + " gold gained.");
            }
            if (salvageRarity > 96)
            {
                dataManager.playerGold += legendaryMoney;
                transientData.PushAlert("You found some legendary salvage!! " + legendaryMoney + " gold gained.");
            }

            StartCoroutine(AlphaFade());
        }
    }

    IEnumerator AlphaFade()
    {
        yield return new WaitForSeconds(.08f);
        alphaSetting = alphaSetting - 0.05f;

        if (alphaSetting > 0)
            StartCoroutine(AlphaFade());

        else if (alphaSetting <= 0)
            Destroy(gameObject);
    }
}
