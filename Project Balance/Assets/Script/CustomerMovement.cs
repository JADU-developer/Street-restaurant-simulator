using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : MonoBehaviour
{
   
    
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {

    }
}
