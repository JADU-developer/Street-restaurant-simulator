using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerOrder : MonoBehaviour, IInteractable
{
    [Header("Order Settings")]
    [SerializeField] private string interactionPrompt = "F : Deliver Food";

    [SerializeField] private List<SOFood> availableFoods = new List<SOFood>();

    [SerializeField] private int maxOrderSize = 3;

    [Header("References")]
    [SerializeField] private GameObject foodContainer;
    [SerializeField] private Transform holdingTrayTransform;
    [SerializeField] private Image orderImageTemplateForOnTopHead;

    [Header("Timer Settings")]
    [SerializeField] private float maxWaitingTime = 30f;
    [SerializeField] private float maxConsumeTime = 10f;

    [Header("Main Ui References")]
    [SerializeField] private GameObject OrderTemplet;
    [SerializeField] private GameObject OrderImage;
    [SerializeField] private GameObject FaceImage;
    [SerializeField] private TextMeshProUGUI OrderNumberText;

    // Face image lists and gender selection
    [Header("Face Image Settings")]
    [SerializeField] private List<Sprite> manFaceImages = new List<Sprite>();
    [SerializeField] private List<Sprite> womanFaceImages = new List<Sprite>();
    [SerializeField] private bool isMan = true;

    [Header("Audio")]
    [SerializeField] private AudioClip OrderDevelerAudio;
    [SerializeField] private AudioClip OrderResiveAudio;
    [SerializeField] private AudioClip PaymentAudio;
    [SerializeField] private float Volume;

    [Header("Emoji Settings")]
    [SerializeField] private List<Sprite> waitingEmojis = new List<Sprite>();
    [SerializeField] private List<Sprite> orderCompleteEmojis = new List<Sprite>();
    [SerializeField] private List<Sprite> notDeliveredEmojis = new List<Sprite>();
    [SerializeField] private GameObject EmojiImage; // Assign an Image GameObject in the inspector

    private GameObject spawnedEmojiImage;
    private float emojiHideTimer = 0f;
    private enum EmojiState { None, Waiting, OrderComplete, NotDelivered }
    private EmojiState emojiState = EmojiState.None;

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
        if (spawnedEmojiImage != null)
        {
            Destroy(spawnedEmojiImage);
            spawnedEmojiImage = null;
        }
        if (EmojiImage != null)
        {
            EmojiImage.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateTimer();
        UpdateEmoji();
        OrderNumberText.text = (OrderTemplet.transform.parent.childCount - 2).ToString();
    }

    private void UpdateTimer()
    {
        if (timerState == TimerState.Waiting)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay($"{Mathf.Max(0, Mathf.CeilToInt(timer))}");
            if (emojiState != EmojiState.Waiting)
                ShowEmoji(waitingEmojis, maxWaitingTime, EmojiState.Waiting);
            if (timer <= 0f && !waitingTimeoutLogged)
            {
                waitingTimeoutLogged = true;
                timerState = TimerState.None;
                Debug.Log("Waiting time expired"); // Add your logic here

                StarManager.Instance.RemoveStar(0.1f); // Example: Remove 0.1 star for timeout
                ShowEmoji(notDeliveredEmojis, 2f, EmojiState.NotDelivered); // Show not delivered emoji for 2 seconds
            }
        }
        else if (timerState == TimerState.Consuming)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay($"{Mathf.Max(0, Mathf.CeilToInt(timer))}");
            if (emojiState != EmojiState.OrderComplete)
                ShowEmoji(orderCompleteEmojis, maxConsumeTime, EmojiState.OrderComplete);
            if (timer <= 0f && !consumeTimeoutLogged)
            {
                consumeTimeoutLogged = true;
                timerState = TimerState.None;
                Debug.Log("Consume time finished"); // Add your logic here

                MoneyManager.Instance.AddMoney(MoneyToGive);
                MoneyToGive = 0;

                StarManager.Instance.AddStar(0.1f);

                soundEffectsManager.instance.playSoundEffectsClip2D(PaymentAudio, transform, Volume);
                HideEmoji();
            }
        }
    }

    private void UpdateEmoji()
    {
        if (emojiState == EmojiState.NotDelivered)
        {
            if (emojiHideTimer > 0f)
            {
                emojiHideTimer -= Time.deltaTime;
                if (emojiHideTimer <= 0f)
                {
                    HideEmoji();
                    emojiState = EmojiState.None;
                }
            }
        }
        // For Waiting and OrderComplete, emoji is shown for the timer duration, so no need to update here
    }

    private void ShowEmoji(List<Sprite> emojiList, float duration, EmojiState state)
    {
        if (spawnedEmojiImage != null)
            Destroy(spawnedEmojiImage);
        if (emojiList != null && emojiList.Count > 0 && EmojiImage != null)
        {
            spawnedEmojiImage = Instantiate(EmojiImage, EmojiImage.transform.position, EmojiImage.transform.rotation, EmojiImage.transform.parent);
            Image img = spawnedEmojiImage.GetComponent<Image>();
            img.sprite = emojiList[Random.Range(0, emojiList.Count)];
            spawnedEmojiImage.SetActive(true);
            EmojiImage.SetActive(false); // Hide the original prefab
            emojiState = state;
            if (state == EmojiState.NotDelivered)
                emojiHideTimer = duration;
        }
    }

    private void HideEmoji()
    {
        if (spawnedEmojiImage != null)
        {
            Destroy(spawnedEmojiImage);
            spawnedEmojiImage = null;
        }
        if (EmojiImage != null)
        {
            EmojiImage.SetActive(false);
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

        soundEffectsManager.instance.playSoundEffectsClip2D(OrderResiveAudio, transform, Volume);
        StartWaitingTimer();
        UpdateOrderUI();
        ShowEmoji(waitingEmojis, maxWaitingTime, EmojiState.Waiting);
    }

    public void Interact(Transform interactorTransform)
    {
        if (currentOrder.Count == 0 || waitingTimeoutLogged || consumeTimeoutLogged)
            return;

        if (holdingTrayTransform.childCount != 0)
        {
            for (int i = 1; i < holdingTrayTransform.GetChild(0).transform.childCount; i++)
            {
                if (holdingTrayTransform.GetChild(0).transform.GetChild(i).TryGetComponent<ObjectGrabbable>(out ObjectGrabbable grabbable))
                {
                    TryCompleteOrder(grabbable.GetFoodSO(), grabbable.gameObject);
                }
            }
        }
        // if (foodContainer.transform.parent.IsChildOf(holdingTrayTransform))
        // {
        //     for (int i = 1; i < foodContainer.transform.childCount; i++)
        //     {
        //         if (foodContainer.transform.GetChild(i).TryGetComponent<ObjectGrabbable>(out ObjectGrabbable grabbable))
        //         {
        //             TryCompleteOrder(grabbable.GetFoodSO(), grabbable.gameObject);
        //         }
        //     }
        // }
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

            soundEffectsManager.instance.playSoundEffectsClip3D(OrderDevelerAudio, transform, Volume);
        }

        UpdateOrderUI();
    }

    public string GetInteractText() => interactionPrompt;
    public bool CanShowInteractContainer() => !IsOrderCompleted();
    public Transform GetTransform() => transform;

    private void UpdateOrderUI()
    {
        for (int i = 1; i < orderImageTemplateForOnTopHead.transform.parent.childCount; i++)
        {
            Destroy(orderImageTemplateForOnTopHead.transform.parent.GetChild(i).gameObject);
        }

        if (NewSpawnedOrder && NewSpawnedFaceImage != null)
        {
            Destroy(NewSpawnedOrder);
            Destroy(NewSpawnedFaceImage);
        }

        // Select random face image based on gender
        Sprite selectedFaceSprite = null;
        List<Sprite> faceList = isMan ? manFaceImages : womanFaceImages;
        if (faceList != null && faceList.Count > 0)
        {
            selectedFaceSprite = faceList[Random.Range(0, faceList.Count)];
        }

        // Show face image as the first order image on top of head
        if (selectedFaceSprite != null)
        {
            GameObject faceClone = Instantiate(orderImageTemplateForOnTopHead.gameObject, orderImageTemplateForOnTopHead.transform.position, Quaternion.identity);
            faceClone.transform.SetParent(orderImageTemplateForOnTopHead.transform.parent);
            faceClone.GetComponent<Image>().sprite = selectedFaceSprite;
            ResetTransform(faceClone, orderImageTemplateForOnTopHead.gameObject);
            faceClone.gameObject.SetActive(true);
        }

        foreach (SOFood item in currentOrder)
        {
            GameObject clone = Instantiate(orderImageTemplateForOnTopHead.gameObject, orderImageTemplateForOnTopHead.transform.position, Quaternion.identity);
            clone.transform.SetParent(orderImageTemplateForOnTopHead.transform.parent);
            clone.GetComponent<Image>().sprite = item.foodImage;
            ResetTransform(clone, orderImageTemplateForOnTopHead.gameObject);
            clone.gameObject.SetActive(true);
        }

        NewSpawnedOrder = Instantiate(OrderTemplet, OrderTemplet.transform.position, Quaternion.identity);
        NewSpawnedFaceImage = Instantiate(FaceImage, FaceImage.transform.position, Quaternion.identity);

        NewSpawnedOrder.transform.SetParent(OrderTemplet.transform.parent);
        NewSpawnedFaceImage.transform.SetParent(NewSpawnedOrder.transform);

        ResetTransform(NewSpawnedOrder, OrderTemplet);
        ResetTransform(NewSpawnedFaceImage, FaceImage);

        // Set the same face image to the UI face image
        Image faceImageComponent = NewSpawnedFaceImage.GetComponent<Image>();
        if (faceImageComponent != null && selectedFaceSprite != null)
        {
            faceImageComponent.sprite = selectedFaceSprite;
        }

        foreach (SOFood item in currentOrder)
        {
            GameObject clone = Instantiate(OrderImage, NewSpawnedOrder.transform.position, Quaternion.identity);
            clone.transform.SetParent(NewSpawnedOrder.transform);
            clone.GetComponent<Image>().sprite = item.foodImage;
            ResetTransform(clone, OrderImage);
            clone.gameObject.SetActive(true);
        }

        NewSpawnedOrder.SetActive(true);
        NewSpawnedFaceImage.SetActive(true);
    }

    private void ResetTransform(GameObject WhoseReset, GameObject Resetwith)
    {
        WhoseReset.GetComponent<RectTransform>().localPosition = Resetwith.GetComponent<RectTransform>().localPosition;
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
        HideEmoji();
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

    public void SetMaxOrderSize(int newMaxOrderSize)
    {
        maxOrderSize = newMaxOrderSize;
    }
}
