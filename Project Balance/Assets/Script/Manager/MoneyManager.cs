using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{

    public static MoneyManager Instance {get ; private set;}

    [SerializeField] private TextMeshProUGUI MoneyText;

    private int currentMoney = 0;


    private void Awake() 
    {
        Instance = this;
    }

    private void Start() 
    {
        UpdateMoneyText();
    }


    private void UpdateMoneyText()
    {
        MoneyText.text = currentMoney.ToString();
    }

    public void AddMoney(int moneyTOAdd)
    {
        currentMoney += moneyTOAdd;
        UpdateMoneyText();
    }
    public void SuntarctMoney(int moneyTOSuntarct)
    {
        currentMoney -= moneyTOSuntarct;
        UpdateMoneyText();
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }



}


