using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerOrder : MonoBehaviour, IInteractable
{
    [Header("Order Settings")]
    [Tooltip("Text shown when interacting with the customer.")]
    [SerializeField] private string interactionPrompt = "F : Deliver Food";

    [Tooltip("List of possible foods for orders.")]
    [SerializeField] private List<SOFood> availableFoods = new List<SOFood>();

    [Tooltip("Maximum number of items in an order.")]
    [SerializeField] private int maxOrderSize = 3;

    [Header("References")] [Tooltip("Container for delivered food objects. Which is under the tray.")]
    [SerializeField] private GameObject foodContainer;

    [Tooltip("Transform representing the holding tray.")]
    [SerializeField] private Transform holdingTrayTransform;

    [SerializeField] private Image orderImageTemplateForOnTopHead;


    [Header("Timer Settings")]
    [SerializeField] private float maxWaitingTime = 30f;
    [SerializeField] private float maxConsumeTime = 10f;


    [Header("Main Ui References")]
    [SerializeField] private GameObject OrderTemplet;
    [SerializeField] private GameObject OrderImage;
    [SerializeField] private GameObject FaceImage;


    [Header("Audio")]
    [SerializeField] private AudioClip OrderDevelerAudio;
    [SerializeField] private AudioClip OrderResiveAudio;
    [SerializeField] private AudioClip PaymentAudio;
    [SerializeField] private float Volume;


    private int MoneyToGive = 0;


    private GameObject NewSpawnedOrder;
    private GameObject NewSpawnedFaceImage;

    private List<SOFood> currentOrder = new List<SOFood>();


    private float timer = 0f;
    private TimerState timerState = TimerState.None;
    private bool waitingTimeoutLogged = false;
    private bool consumeTimeoutLogged = false;

    private enum TimerState { None, Waiting, Consuming }

    private void Start()
    {
        // GenerateRandomOrder();
        orderImageTemplateForOnTopHead.gameObject.SetActive(false);
        if (maxOrderSize > availableFoods.Count)
        {
            Debug.LogError($"Max Order Size ({maxOrderSize}) is greater than the available food count ({availableFoods.Count})");
        }
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (timerState == TimerState.Waiting)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay($"{Mathf.Max(0, Mathf.CeilToInt(timer))}");
            if (timer <= 0f && !waitingTimeoutLogged)
            {
                waitingTimeoutLogged = true;
                timerState = TimerState.None;
                Debug.Log("Waiting time expired"); // Add your logic here

                StarManager.Instance.RemoveStar(0.1f); // Example: Remove 0.1 star for timeout
            }
        }
        else if (timerState == TimerState.Consuming)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay($"{Mathf.Max(0, Mathf.CeilToInt(timer))}");
            if (timer <= 0f && !consumeTimeoutLogged)
            {
                consumeTimeoutLogged = true;
                timerState = TimerState.None;
                Debug.Log("Consume time finished"); // Add your logic here

                MoneyManager.Instance.AddMoney(MoneyToGive);
                MoneyToGive = 0;

                StarManager.Instance.AddStar(0.1f);

                soundEffectsManager.instance.playSoundEffectsClip2D(PaymentAudio , transform , Volume);
            }
        }
    }

    private void UpdateTimerDisplay(string text)
    {
        if (NewSpawnedFaceImage != null)
            NewSpawnedFaceImage.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void StartWaitingTimer()
    {
        timer = maxWaitingTime;
        timerState = TimerState.Waiting;
        waitingTimeoutLogged = false;
        consumeTimeoutLogged = false;
        UpdateTimerDisplay($"{Mathf.CeilToInt(timer)}");
    }

    private void StartConsumeTimer()
    {
        timer = maxConsumeTime;
        timerState = TimerState.Consuming;
        consumeTimeoutLogged = false;
        UpdateTimerDisplay($"{Mathf.CeilToInt(timer)}");
        ClearOrderUI();
    }

    public void GenerateRandomOrder()
    {
        currentOrder.Clear();
        int orderCount = Random.Range(1, maxOrderSize);
        for (int i = 0; i < orderCount; i++)
        {
            int randomIndex = Random.Range(0, availableFoods.Count);
            SOFood randomFood = availableFoods[randomIndex];
            currentOrder.Add(randomFood);
            Debug.Log($"Order Food: {randomFood.foodName}");

            MoneyToGive += randomFood.price;
        }

        soundEffectsManager.instance.playSoundEffectsClip2D(OrderResiveAudio , transform , Volume);
        StartWaitingTimer();
        UpdateOrderUI();
    }

    public void Interact(Transform interactorTransform)
    {
        if (currentOrder.Count == 0 || waitingTimeoutLogged || consumeTimeoutLogged)
            return;

        if (foodContainer.transform.parent.IsChildOf(holdingTrayTransform))
        {
            for (int i = 0; i < foodContainer.transform.childCount; i++)
            {
                if (foodContainer.transform.GetChild(i).TryGetComponent<ObjectGrabbable>(out ObjectGrabbable grabbable))
                {
                    TryCompleteOrder(grabbable.GetFoodSO(), grabbable.gameObject);
                }
            }
        }
    }

    private void TryCompleteOrder(SOFood food, GameObject foodObject)
    {
        if (currentOrder.Contains(food))
        {
            currentOrder.Remove(food);
            Debug.Log($"Order Completed: {food.foodName}");
            Destroy(foodObject);
        }
        else
        {
            Debug.Log($"Order Failed: {food.foodName}");
        }
        if (currentOrder.Count == 0)
        {
            Debug.Log("All Orders Completed!");
            timerState = TimerState.None;
            StartConsumeTimer();

            soundEffectsManager.instance.playSoundEffectsClip3D(OrderDevelerAudio , transform , Volume);
        }
        UpdateOrderUI();
    }

    public string GetInteractText() => interactionPrompt;
    public bool CanShowInteractContainer() => true;
    public Transform GetTransform() => transform;

    private void UpdateOrderUI()
    {
        for (int i = 1; i < orderImageTemplateForOnTopHead.transform.parent.childCount; i++)
        {
            Destroy(orderImageTemplateForOnTopHead.transform.parent.GetChild(i).gameObject);
        }

        if(NewSpawnedOrder && NewSpawnedFaceImage != null)
        {
            Destroy(NewSpawnedOrder);
            Destroy(NewSpawnedFaceImage);
        }

        foreach (SOFood item in currentOrder)
        {
            GameObject clone = Instantiate(orderImageTemplateForOnTopHead.gameObject, orderImageTemplateForOnTopHead.transform.position, Quaternion.identity);

            clone.transform.SetParent(orderImageTemplateForOnTopHead.transform.parent);
            clone.GetComponent<Image>().sprite = item.foodImage;
            clone.GetComponent<RectTransform>().localPosition= orderImageTemplateForOnTopHead.GetComponent<RectTransform>().localPosition;
            clone.GetComponent<RectTransform>().localScale = orderImageTemplateForOnTopHead.GetComponent<RectTransform>().localScale;
            clone.GetComponent<RectTransform>().localRotation = orderImageTemplateForOnTopHead.GetComponent<RectTransform>().localRotation;
            clone.gameObject.SetActive(true);
        }

        NewSpawnedOrder = Instantiate(OrderTemplet, OrderTemplet.transform.position, Quaternion.identity);
        NewSpawnedFaceImage = Instantiate(FaceImage, FaceImage.transform.position, Quaternion.identity);

        NewSpawnedOrder.transform.SetParent(OrderTemplet.transform.parent);
        NewSpawnedFaceImage.transform.SetParent(NewSpawnedOrder.transform);

                    ResetTectTransform(NewSpawnedOrder , OrderTemplet);
                    ResetTectTransform(NewSpawnedFaceImage , FaceImage);
                    

        foreach(SOFood item in currentOrder)
        {
            GameObject clone = Instantiate(OrderImage, NewSpawnedOrder.transform.position, Quaternion.identity);

            clone.transform.SetParent(NewSpawnedOrder.transform);

            clone.GetComponent<Image>().sprite = item.foodImage;
            // clone.GetComponent<RectTransform>().localPosition= OrderImage.GetComponent<RectTransform>().localPosition;
            // clone.GetComponent<RectTransform>().localScale = OrderImage.GetComponent<RectTransform>().localScale;
            // clone.GetComponent<RectTransform>().localRotation = OrderImage.GetComponent<RectTransform>().localRotation;

            ResetTectTransform(clone , OrderImage);

            clone.gameObject.SetActive(true);
        }

        NewSpawnedOrder.SetActive(true);
        NewSpawnedFaceImage.SetActive(true);
    }

    private void ResetTectTransform(GameObject WhoseReset , GameObject Resetwith)
    {
        WhoseReset.GetComponent<RectTransform>().localPosition= Resetwith.GetComponent<RectTransform>().localPosition;
            WhoseReset.GetComponent<RectTransform>().localScale = Resetwith.GetComponent<RectTransform>().localScale;
            WhoseReset.GetComponent<RectTransform>().localRotation = Resetwith.GetComponent<RectTransform>().localRotation;

    }



    public void ClearOrderUI()
    {
        // Hide or clear all order UI elements
        if (orderImageTemplateForOnTopHead != null)
            orderImageTemplateForOnTopHead.gameObject.SetActive(false);
        if (NewSpawnedFaceImage != null)
            NewSpawnedFaceImage.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        // Optionally destroy any instantiated order images
        if (orderImageTemplateForOnTopHead != null && orderImageTemplateForOnTopHead.transform.parent != null)
        {
            for (int i = 1; i < orderImageTemplateForOnTopHead.transform.parent.childCount; i++)
            {
                Destroy(orderImageTemplateForOnTopHead.transform.parent.GetChild(i).gameObject);
            }
        }

        Destroy(NewSpawnedOrder);
        Destroy(NewSpawnedFaceImage);
    }




    // Add these methods for movement script integration
    public bool IsOrderCompleted()
    {
        // Order is completed if all food is delivered and consume timer started
        return currentOrder.Count == 0 && timerState == TimerState.Consuming;
    }



    public bool IsWaitingTimeout()
    {
        // Waiting timeout means the player failed to deliver in time
        return waitingTimeoutLogged;
    }



    public bool IsConsumeTimeout()
    {
        // Consuming timeout means the customer finished eating
        return consumeTimeoutLogged;
    }
}
