using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public static PlayerInteract Instance { get; private set;}

    [SerializeField , Range(0,10)] private float interactRang = 3f;
    [SerializeField] private KeyCode Interactkey = KeyCode.E;

    private void Awake() 
    {
        Instance = this;
    }


    private void Update()
    {
        if (Input.GetKeyDown(Interactkey))
        {
            IInteractable interactable = GetInteractableObject();

            if (interactable != null)
            {
                interactable.Interact(transform);
            }
        }
    }

    public IInteractable GetInteractableObject()
    {
        List<IInteractable> interactablesList = new List<IInteractable>();

        Collider[] colliderArray = Physics.OverlapSphere(transform.position,interactRang);
        foreach(Collider collider in colliderArray)
        {
            if(collider.TryGetComponent(out IInteractable Interactable))
            {
                interactablesList.Add(Interactable);
            }
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactablesList)         
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) < Vector3.Distance(transform.position , closestInteractable.GetTransform().position))
                {
                    closestInteractable = interactable;
                }
            }
        }

        return closestInteractable;
    }
}
