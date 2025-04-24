using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class FoodContainer : MonoBehaviour
{   
    
    [SerializeField] private GameObject FoodPrefab;
    [SerializeField] private Transform PointContainer;
    [SerializeField] private float fSpawnDelay = 5f;
    [SerializeField] private int StartingFoodStock = 29;

    [Header("Audio")]
    [SerializeField] private AudioClip MoneySpendAudio;
    [SerializeField]private float Volume = 1f;
    private int maxFoodStock = 99;
    public TMPro.TextMeshProUGUI stockText;
    private int currentFoodStock;
    private List<Transform> points = new();

    void Start()
    {

        currentFoodStock = StartingFoodStock;
        

        for (int i = 0; i < PointContainer.childCount; i++)
        {
            points.Add(PointContainer.GetChild(i));
        }

        UpdateStockText();
        StartCoroutine(CheckForFoodSpawn());

    }

    private void UpdateStockText()
    {
        if (stockText != null)
        {
            int n = 0; 
            foreach (Transform t in points)
            {
                if (t.childCount != 0)
                {
                    n++;
                }
            }
            stockText.text = (currentFoodStock + n).ToString();
        }
        else
        {
            Debug.LogError("No text to show food stock!");
        }
    }

    private IEnumerator CheckForFoodSpawn()
    {
        yield return new WaitForSeconds(fSpawnDelay);

        foreach (Transform t in points)
        {
            if (t.childCount == 0 && currentFoodStock > 0)
            {
                GameObject newFood = Instantiate(FoodPrefab , t.position , Quaternion.identity);
                newFood.transform.SetParent(t);
                currentFoodStock--;

            }
        }
        
        UpdateStockText();

        StartCoroutine(CheckForFoodSpawn());
    }

    public void IncreaseFoodStockBu10()
    {
        if (MoneyManager.Instance.GetCurrentMoney() >= FoodPrefab.GetComponent<ObjectGrabbable>().GetFoodSO().BuldlePrice)
        {
            int increaseby = 10;
            currentFoodStock = Mathf.Clamp(currentFoodStock + increaseby, 0, maxFoodStock);
            MoneyManager.Instance.SuntarctMoney(FoodPrefab.GetComponent<ObjectGrabbable>().GetFoodSO().BuldlePrice);
            UpdateStockText();

            soundEffectsManager.instance.playSoundEffectsClip2D(MoneySpendAudio , transform , Volume);
        }
    }
}