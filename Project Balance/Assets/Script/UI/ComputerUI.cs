using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DialogueEditor;

public class ComputerUI : MonoBehaviour
{
    private const string ORDER_NUMBER = "#24567";

    [Header("TeleApp")]
    [SerializeField] private Button TeleAppButton;

    [Header("TeleAppPlane")]
    [SerializeField] private GameObject PlayerRepleyMessage;
    [SerializeField] private GameObject ClientRepleyMessage;
    [SerializeField] private GameObject ClientRepleyMessage_OrderNumber;


    [Header("AmmaFlip Panel Setting"), Space]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button SearchButtonInAmmaFlip;
    [SerializeField] private GameObject OrderImage;
    [SerializeField] private GameObject OrderNotFoundText;


    [Header("Task"), Space]
    [SerializeField] private SOTaskData CheckOrderTaskData;
    [SerializeField] private SOTaskData ServeClient;

    [Header("Conversations"), Space]
    [SerializeField] private NPCConversation SecondConversation;

    private void Awake()
    {

        TeleAppButton.onClick.AddListener(() =>
        {
            if (TaskManager.instance.ContainTask(ServeClient))
            {
                ConversationManager.Instance.StartConversation(SecondConversation);
            }
        });


        SearchButtonInAmmaFlip.onClick.AddListener(() =>
        {
            if (inputField.text == ORDER_NUMBER)
            {
                OrderNotFoundText.SetActive(false);
                OrderImage.SetActive(true);
                TaskManager.instance.IncrementTaskProgress(CheckOrderTaskData);
                TaskManager.instance.AddTask(ServeClient);
            }
            else
            {
                OrderImage.SetActive(false);

                //* Display No oder Found image
                OrderNotFoundText.SetActive(true);
            }
        });
    }

    private void Start()
    {
        OrderImage.SetActive(false);

        PlayerRepleyMessage.SetActive(false);
        ClientRepleyMessage.SetActive(false);
        ClientRepleyMessage_OrderNumber.SetActive(false);

    }

    private void SpawnRepleyMessageBase(GameObject message)
    {
        GameObject clone = Instantiate(message, message.transform.position, message.transform.rotation);
        // clone.transform.parent = message.transform.parent;
        clone.transform.SetParent(message.transform.parent);

        clone.GetComponent<RectTransform>().localPosition = message.GetComponent<RectTransform>().localPosition;
        clone.GetComponent<RectTransform>().localRotation = message.GetComponent<RectTransform>().localRotation;
        clone.GetComponent<RectTransform>().localScale = message.GetComponent<RectTransform>().localScale;

        clone.SetActive(true);
    }


    #region PublicMethode
    public void A_SpawnPlayerRepleyMessage()
    {
        SpawnRepleyMessageBase(PlayerRepleyMessage);
    }

    public void A_SpawnClientrRepleyMessage()
    {
        SpawnRepleyMessageBase(ClientRepleyMessage);
    }

    public void A_SpawnClientRepleyMessage_OrderNumber()
    {
        SpawnRepleyMessageBase(ClientRepleyMessage_OrderNumber);
    }
    #endregion

}
