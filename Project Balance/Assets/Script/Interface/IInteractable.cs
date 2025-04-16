using UnityEngine;

public interface IInteractable
{
   
    void Interact(Transform interactorTransform);
    string GetInteractText();
    bool CanShowInteractContainer();
    Transform GetTransform();
    
}
