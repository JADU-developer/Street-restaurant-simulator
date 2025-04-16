using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "Task System/Task")]
public class SOTaskData : ScriptableObject
{
    public string taskName;
    public string description;
    public int requiredCount = 1; 

}
