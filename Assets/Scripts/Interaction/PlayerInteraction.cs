using UnityEngine;

// Cualquier objeto con un collider y este contrato puede interactuarse con la tecla E.
public interface IInteractable
{
    void Interact();
}

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Camera playerCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !GameManager.UiBlocked)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player camera is not assigned.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
