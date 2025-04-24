using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class CustomerMovement : MonoBehaviour
{
    [Header("Movement Points")]
    [Tooltip("List of points the customer can visit to place an order.")]
    public List<Transform> orderPoints;
    [Tooltip("List of points the customer can go to after ordering.")]
    public List<Transform> exitPoints;

    private NavMeshAgent agent;
    private CustomerOrder customerOrder;
    private Transform currentPoint;
    private bool isOrdering = false;
    private bool canOrder = true;
    private bool isMoving = false;
    private bool waitingTimeout = false;
    private bool consumeTimeout = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        customerOrder = GetComponent<CustomerOrder>();
    }

    void Start()
    {
        GoToAvailableOrderPoint();
    }

    void Update()
    {
        if (isMoving && currentPoint != null)
        {
            float dist = Vector3.Distance(transform.position, currentPoint.position);
            Debug.DrawRay(transform.position, currentPoint.position - transform.position, Color.green);
            if (dist < 0.6f)
            {
                isMoving = false;
                if (canOrder && !isOrdering)
                {
                    customerOrder.GenerateRandomOrder();
                    StartCoroutine(OrderRoutine());
                }
            }
        }
    }

    void GoToAvailableOrderPoint()
    {
        foreach (var point in orderPoints)
        {
            if (point.childCount == 0)
            {
                currentPoint = point;

                transform.SetParent(point);
                agent.SetDestination(point.position);
                isMoving = true;
                return;
            }
        }
        currentPoint = orderPoints[Random.Range(0, orderPoints.Count)];
        agent.SetDestination(currentPoint.position);
        isMoving = true;
    }

    IEnumerator OrderRoutine()
    {
        isOrdering = true;
        canOrder = false;
        waitingTimeout = false;
        consumeTimeout = false;
        customerOrder.enabled = true;
        while (!customerOrder.IsOrderCompleted() && !customerOrder.IsWaitingTimeout())
        {
            yield return null;
        }
        waitingTimeout = customerOrder.IsWaitingTimeout();
        if (waitingTimeout)
        {
            yield return LeaveAndGoToRandomPoint();
        }
        else
        {
            while (!customerOrder.IsConsumeTimeout())
            {
                yield return null;
            }
            consumeTimeout = true;
            yield return LeaveAndGoToRandomPoint();
        }
        isOrdering = false;
    }

    IEnumerator LeaveAndGoToRandomPoint()
    {
        yield return new WaitForSeconds(0.5f);
        customerOrder.ClearOrderUI();

        transform.SetParent(null);
        Transform nextPoint = exitPoints[Random.Range(0, exitPoints.Count)];
        currentPoint = nextPoint;
        agent.SetDestination(currentPoint.position);
        isMoving = true;
        while (Vector3.Distance(transform.position, currentPoint.position) > 0.9f)
        {
            Debug.DrawRay(transform.position, currentPoint.position - transform.position, Color.red);
            yield return null;
        }
        canOrder = true;
        isOrdering = false;
        yield return new WaitForSeconds(1f);
        GoToAvailableOrderPoint();
    }
}
