using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerOrder : MonoBehaviour, IInteractable
{
    [SerializeField] private string InteractionText = "F : Deliver Food";
    [SerializeField] private List<SOFood> foodlist = new List<SOFood>();
    [SerializeField] private GameObject FoodContainer;
    [SerializeField] private Transform HoldingTrayGameobject;


    private List<SOFood> CurrentOrderFood = new List<SOFood>();


    private void Start()
    {
        GetRandomFood();
    }



    private void GetRandomFood()
    {
        int randomamount = Random.Range(1, foodlist.Count);

        for (int i = 0; i < randomamount; i++)
        {
            int randomIndex = Random.Range(0, foodlist.Count);
            SOFood randomFood = foodlist[randomIndex];

            CurrentOrderFood.Add(randomFood);
            Debug.Log("Order Food: " + randomFood.foodName);
        }
    }

    public void Interact(Transform interactorTransform)
    {
        if (FoodContainer.transform.parent.IsChildOf(HoldingTrayGameobject))
        {
            for (int i = 0; i < FoodContainer.transform.childCount; i++)
            {
                if (FoodContainer.transform.GetChild(i).TryGetComponent<ObjectGrabbable>(out ObjectGrabbable component))
                {
                    CompleteOrder(component.GetFoodSO(), component.gameObject);
                }
            }
        }

    }

    private void CompleteOrder(SOFood sOFood, GameObject objectToDestroy)
    {
        if (CurrentOrderFood.Contains(sOFood))
        {
            CurrentOrderFood.Remove(sOFood);
            Debug.Log("Order Completed: " + sOFood.foodName);

            Destroy(objectToDestroy);

        }
        else
        {
            Debug.Log("Order Failed: " + sOFood.foodName);
        }

        if (CurrentOrderFood.Count == 0)
        {
            Debug.Log("All Orders Completed!");

            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("                        NEW ORDER                          ");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            GetRandomFood();
        }


    }

    public string GetInteractText()
    {
        return InteractionText;
    }

    public bool CanShowInteractContainer()
    {
        return true;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
