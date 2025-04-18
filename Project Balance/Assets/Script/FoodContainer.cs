using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FoodContainer : MonoBehaviour
{   
    
    [SerializeField] private GameObject FoodPrefab;
    [SerializeField] private Transform PointContainer;
    [SerializeField] private float fSpawnDelay = 5f;
    private List<Transform> points = new();

    void Start()
    {
        for (int i = 0; i < PointContainer.childCount; i++)
        {
            points.Add(PointContainer.GetChild(i));
        }

        StartCoroutine(CheckForFoodSpawn());

    }

    private IEnumerator CheckForFoodSpawn()
    {
        yield return new WaitForSeconds(fSpawnDelay);

        foreach (Transform t in points)
        {
            if (t.childCount == 0)
            {
                //* Spawn food

                GameObject newFood = Instantiate(FoodPrefab , t.position , Quaternion.identity);
                newFood.transform.SetParent(t);
            }
        }

        StartCoroutine(CheckForFoodSpawn());
    }
}