using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private TextMeshProUGUI interactTextMeshProGUI;

    private void Update() 
    {
        if (PlayerInteract.Instance.GetInteractableObject() != null) 
        {
            if (PlayerInteract.Instance.GetInteractableObject().CanShowInteractContainer())
            {
                Show(PlayerInteract.Instance.GetInteractableObject());
            }
            else
            {
                Hide();
            }
        }
        else
        {
            Hide();
        }
    }

    private void Show(IInteractable interactable)
    {
        containerGameObject.SetActive(true);
        interactTextMeshProGUI.text = interactable.GetInteractText();
    }

    private void Hide()
    {
        containerGameObject.SetActive(false);
    }
}
