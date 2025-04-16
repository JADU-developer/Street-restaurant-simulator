using UnityEngine;
using TMPro;

public class Task 
{
    public SOTaskData Data { get; private set; }
    public bool IsCompleted { get; private set; }
    private TMP_Text taskText;

    private int currentCount;  // How many items collected

    public Task(SOTaskData data)
    {
        Data = data;
        currentCount = 0;
        IsCompleted = false;
    }

    public void SetTaskUI(TMP_Text textComponent)
    {
        taskText = textComponent;
        UpdateText();
    }

    public void IncrementProgress()
    {
        if (IsCompleted) return;

        currentCount++;
        UpdateText();

        if (currentCount >= Data.requiredCount)
        {
            MarkComplete();
        }
    }

    private void UpdateText()
    {
        if (taskText != null)
        {
            taskText.text = $"{Data.taskName} ({currentCount}/{Data.requiredCount})";
        }
    }

    private void MarkComplete()
    {
        IsCompleted = true;
        UnityEngine.MonoBehaviour.Destroy(taskText.gameObject);
        // taskText.text = $"<s>{Data.taskName} ({Data.requiredCount}/{Data.requiredCount})</s>";
        // taskText.color = Color.green;
    }
}
