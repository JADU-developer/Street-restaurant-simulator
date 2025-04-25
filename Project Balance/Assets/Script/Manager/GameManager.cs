using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }


    // Difficulty settings for each star range (editable in inspector)
    [Header("Order Size Settings by Star Range")]
    [SerializeField] private int maxOrderSize_0_1 = 2;
    [SerializeField] private int maxOrderSize_1_2 = 3;
    [SerializeField] private int maxOrderSize_2_3 = 5;
    [SerializeField] private int maxOrderSize_3_4 = 7;
    [SerializeField] private int maxOrderSize_4_5 = 9;

    // List of all customer orders in the game
    [SerializeField] private List<CustomerOrder> customerOrders = new List<CustomerOrder>();


    private void Awake() 
    {
        Instance = this;
    }


    // Call this whenever you want to update difficulty (e.g., after star change)
    public void UpdateAllCustomerOrderSizes()
    {
        int maxOrderSize = GetMaxOrderSizeForCurrentStar();
        foreach (var customer in customerOrders)
        {
            if (customer != null)
                customer.SetMaxOrderSize(maxOrderSize);
        }

        Debug.Log( "Order Size : " + maxOrderSize);
    }

    // Gets the max order size based on the current star value
    public int GetMaxOrderSizeForCurrentStar()
    {
        float star = StarManager.Instance.GetCurrentStar();

        if (star >= 4f && star < 5f)
            return maxOrderSize_4_5;
        else if (star >= 3f && star < 4f)
            return maxOrderSize_3_4;
        else if (star >= 2f && star < 3f)
            return maxOrderSize_2_3;
        else if (star >= 1f && star < 2f)
            return maxOrderSize_1_2;
        else if (star >= 0f && star < 1f)
            return maxOrderSize_0_1;
        else
            return maxOrderSize_0_1;
        
    }

    // Optionally, call this when a new customer is added
    public void RegisterCustomer(CustomerOrder customer)
    {
        if (!customerOrders.Contains(customer))
        {
            customerOrders.Add(customer);
            customer.SetMaxOrderSize(GetMaxOrderSizeForCurrentStar());
        }
    }
}
