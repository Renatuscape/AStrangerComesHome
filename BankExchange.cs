using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BankExchange : MonoBehaviour
{
    public BankManager bankManager;
    public TextMeshProUGUI commissionRate;
    public TextMeshProUGUI buySellValue;
    public Slider slider;
    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        commissionRate.text = MoneyExchange.CalculateCommission() + "%";
    }

    public void OnSliderValueChanged(float value)
    {
        if (slider.value >= 999)
        {
            buySellValue.text = "All";
        }
        else
        {
            buySellValue.text = slider.value.ToString();
        }
    }

    public void SellHellers()
    {
        int sliderValue = (int)slider.value;

        if (sliderValue >= 999)
        {
            sliderValue = Player.GetCount("MIS003-RAR-NN", "Money Manager, ExchangeGoldToSovereign()");
        }

        int remainder = sliderValue - (sliderValue % 100);
        slider.value = remainder;

        bool result = MoneyExchange.ExchangeHellersToShillings(remainder, out int sovereign);
        bankManager.UpdateWallet();

        Debug.Log($"Result of exchange: {result}. Attempted to exchange {remainder}");
    }
}
