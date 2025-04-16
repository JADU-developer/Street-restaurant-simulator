using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class DoorOpeningSystem : MonoBehaviour
{

    public event EventHandler OnTaskComplete;

    public static DoorOpeningSystem instance { get; private set; }


    [SerializeField] private Slider MovingBarSlider;
    [SerializeField] private Transform PerdefineAreaGameObjectParent;
    [SerializeField] private float Speed = 10;
    private int TimeToDoCorrecly = 0;
    private int TimeDoneCurrecly = 0;

    private List<GameObject> ValidAreaList = new();
    private GameObject CurrentValidArea;
    private float MaxValidAreaNumber;
    private float MinValidAreaNumber;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        AddObjectToList();
        MovingBarSlider.gameObject.SetActive(false);
    }


    private void AddObjectToList()
    {
        for (int i = 0, childCount = PerdefineAreaGameObjectParent.childCount; i < childCount; i++)
        {
            ValidAreaList.Add(PerdefineAreaGameObjectParent.GetChild(i).gameObject);
            PerdefineAreaGameObjectParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Update()  //?
    {
        if (TaskComplete())
        {
            OnTaskComplete?.Invoke(this , EventArgs.Empty);

            MovingBarSlider.gameObject.SetActive(false);
            return;
        }

        CheckBarHitSafeArea();
        MoveBar();
    }

    public bool TaskComplete()
    {
        return TimeDoneCurrecly == TimeToDoCorrecly;
    }

    private void GetaRandomArea()
    {
        GameObject newcurrentarea;

        if (CurrentValidArea != null) CurrentValidArea.SetActive(false);

        do
        {
            newcurrentarea = ValidAreaList[Random.Range(0, ValidAreaList.Count)];
        }
        while (newcurrentarea == CurrentValidArea);


        CurrentValidArea = newcurrentarea;
        CurrentValidArea.SetActive(true);

        string CurrentAreaName = CurrentValidArea.name;
        string[] DividedCurrentAreaName = CurrentAreaName.Split(" "); //? Aussuming that the name is writen in (25 40) form with a gape in between

        //?Assing minvalue and maxvalue 

        MinValidAreaNumber = GetStringToInt(DividedCurrentAreaName[0]);
        MaxValidAreaNumber = GetStringToInt(DividedCurrentAreaName[1]);

    }

    private int GetStringToInt(string s)
    {
        if (int.TryParse(s, out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError(gameObject.name + " name is not in the form of (minvalue maxvalue) correct this ex - (20 50)");
            return default;
        }
    }


    private void CheckBarHitSafeArea() //?
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (MovingBarSlider.value > MinValidAreaNumber && MovingBarSlider.value < MaxValidAreaNumber)
            {
                TimeDoneCurrecly++;
                Debug.Log("Get Point " + TimeDoneCurrecly);

                GetaRandomArea();

                //todo play correct sound
            }
            else
            {
                Debug.Log("No");
                //todo play wrong sound
            }
        }
    }

    private void MoveBar()
    {
        MovingBarSlider.value = Mathf.PingPong(Time.time * Speed, MovingBarSlider.maxValue);
    }

    public void Ininsalise(int timeToDo)
    {
        if(TaskComplete())
        MovingBarSlider.gameObject.SetActive(true);
        TimeDoneCurrecly = 0;
        TimeToDoCorrecly = timeToDo;
        GetaRandomArea();
    }
}
