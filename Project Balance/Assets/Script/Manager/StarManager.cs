using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{

    public static StarManager Instance { get; private set; }

    [SerializeField] private Image Filledstar;
    [SerializeField] private Image EmptyStar;
    [SerializeField] private Transform StarHandler;
    [SerializeField] private TextMeshProUGUI StarText;

    [SerializeField] private float StartstarCount = 2;
    private float currentStarCount = 0;



    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentStarCount = StartstarCount;

        Filledstar.gameObject.SetActive(false);
        EmptyStar.gameObject.SetActive(false);

        UpdateStarDisplay();

        GameManager.Instance.UpdateAllCustomerOrderSizes();
    }

    void UpdateStarDisplay()
    {
        // Remove all stars except the templates
        for (int i = StarHandler.childCount - 1; i >= 0; i--)
        {
            var child = StarHandler.GetChild(i);
            if (child.gameObject != Filledstar.gameObject && child.gameObject != EmptyStar.gameObject)
            {
                Destroy(child.gameObject);
            }
        }

        int totalStars = 5;
        int filledStars = Mathf.Clamp((int)Mathf.Floor(currentStarCount), 0, totalStars);
        int emptyStars = totalStars - filledStars;

        // Add filled stars
        for (int i = 0; i < filledStars; i++)
        {
            GameObject filled = Instantiate(Filledstar.gameObject, Filledstar.transform.position, Quaternion.identity);
            filled.transform.SetParent(StarHandler, false);
            filled.SetActive(true);
        }
        // Add empty stars
        for (int i = 0; i < emptyStars; i++)
        {
            GameObject empty = Instantiate(EmptyStar.gameObject, EmptyStar.transform.position, Quaternion.identity);
            empty.transform.SetParent(StarHandler, false);
            empty.SetActive(true);
        }

        StarText.text = currentStarCount.ToString("0.0");
    }


    public float GetCurrentStar()
    {

        Debug.Log( " current star " + currentStarCount);
        return currentStarCount;


    }

    public void AddStar(float starCount)
    {
        if (currentStarCount < 5f)
        {
            currentStarCount += starCount;
            UpdateStarDisplay();
        }

        GameManager.Instance.UpdateAllCustomerOrderSizes();
    }
    public void RemoveStar(float starCount)
    {
        if (currentStarCount > 0f)
        {
            currentStarCount -= starCount;
            UpdateStarDisplay();
        }

        GameManager.Instance.UpdateAllCustomerOrderSizes();
    }
}