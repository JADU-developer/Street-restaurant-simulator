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
    private bool isRoaming = false; // New: roaming state
    private Coroutine roamingCoroutine;

    private Animator animator; // Animator reference

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        customerOrder = GetComponent<CustomerOrder>();
        animator = GetComponentInChildren<Animator>(); // Get Animator
    }

    void Start()
    {
        TryGoToOrderPointOrRoam();
    }

    void Update()
    {
        // Update animation speed parameter
        if (animator != null && agent != null)
        {
            float speed = agent.velocity.magnitude > 0.1f ? 1f : 0f;
            animator.SetFloat("Speed", speed);
        }

        if (isMoving && currentPoint != null)
        {
            float dist = Vector3.Distance(transform.position, currentPoint.position);
            Debug.DrawRay(transform.position, currentPoint.position - transform.position, Color.green);
            if (dist < 0.6f)
            {
                isMoving = false;
                transform.rotation = currentPoint.rotation;
                if (canOrder && !isOrdering && !isRoaming && IsAtOrderPoint(currentPoint))
                {
                    customerOrder.GenerateRandomOrder();
                    StartCoroutine(OrderRoutine());
                }
                else if (isRoaming && IsAtExitPoint(currentPoint))
                {
                    // Arrived at exit point, pick another if still roaming
                    if (isRoaming)
                        GoToRandomExitPoint();
                }
            }
        }
        // While roaming, check for available order point
        if (isRoaming && !isOrdering)
        {
            Transform available = GetAvailableOrderPoint();
            if (available != null)
            {
                StopRoamingAndGoToOrderPoint(available);
            }
        }
    }

    void TryGoToOrderPointOrRoam()
    {
        Transform available = GetAvailableOrderPoint();
        if (available != null)
        {
            GoToOrderPoint(available);
        }
        else
        {
            StartRoaming();
        }
    }

    Transform GetAvailableOrderPoint()
    {
        foreach (var point in orderPoints)
        {
            if (point.childCount == 0)
                return point;
        }
        return null;
    }

    void GoToOrderPoint(Transform point)
    {
        currentPoint = point;
        transform.SetParent(point);
        agent.SetDestination(point.position);
        isMoving = true;
        isRoaming = false;
        if (roamingCoroutine != null)
        {
            StopCoroutine(roamingCoroutine);
            roamingCoroutine = null;
        }
    }

    void StartRoaming()
    {
        isRoaming = true;
        isMoving = false;
        transform.SetParent(null);
        GoToRandomExitPoint();
    }

    void GoToRandomExitPoint()
    {
        if (exitPoints.Count == 0) return;
        Transform nextPoint = exitPoints[Random.Range(0, exitPoints.Count)];
        currentPoint = nextPoint;
        agent.SetDestination(currentPoint.position);
        isMoving = true;
    }

    void StopRoamingAndGoToOrderPoint(Transform point)
    {
        isRoaming = false;
        GoToOrderPoint(point);
    }

    bool IsAtOrderPoint(Transform point)
    {
        return orderPoints.Contains(point);
    }
    bool IsAtExitPoint(Transform point)
    {
        return exitPoints.Contains(point);
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
        TryGoToOrderPointOrRoam();
    }
}
