using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
   
    public static TaskManager instance { get; private set; }

    [SerializeField] private GameObject taskPrefab;
    [SerializeField] private Transform taskContainer;

    private Dictionary<SOTaskData, Task> taskDictionary = new Dictionary<SOTaskData, Task>();

    private void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        // Example: Assign tasks via the Unity Inspector
    }

    public void AddTask(SOTaskData taskData)
    {
        if (!taskDictionary.ContainsKey(taskData))
        {
            Task newTask = new Task(taskData);
            taskDictionary.Add(taskData, newTask);

            GameObject taskUI = Instantiate(taskPrefab, taskContainer);
            newTask.SetTaskUI(taskUI.GetComponent<TMP_Text>());
        }
    }

    public bool ContainTask(SOTaskData taskData)
    {
        return taskDictionary.ContainsKey(taskData);
    }

    public void IncrementTaskProgress(SOTaskData taskData)
    {
        if (taskDictionary.ContainsKey(taskData))
        {
            taskDictionary[taskData].IncrementProgress();
        }
    }
}
