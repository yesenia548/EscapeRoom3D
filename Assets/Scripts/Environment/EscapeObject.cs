using UnityEngine;

public enum EscapeKind { Chest, Key, Door, Note }

// Un solo script para los objetos interactuables del juego: cofre, llave, puerta y nota.
// La lógica de cada tipo es la del Anexo C de la guía (ChestController, KeyItem, DoorController).
public class EscapeObject : MonoBehaviour, IInteractable
{
    [SerializeField] private EscapeKind kind;
    [SerializeField] private Animator animator;                 // Chest y Door
    [SerializeField] private bool unlocked;                     // solo Chest
    [SerializeField, TextArea] private string noteText = "Pista: los cuatro primeros números naturales, en orden.";

    // Lo llama PuzzleManager cuando el código es correcto.
    public void Unlock()
    {
        unlocked = true;
        Debug.Log("Cofre desbloqueado.");
    }

    public void Interact()
    {
        switch (kind)
        {
            case EscapeKind.Chest: InteractChest(); break;
            case EscapeKind.Key: InteractKey(); break;
            case EscapeKind.Door: InteractDoor(); break;
            case EscapeKind.Note: Debug.Log(noteText); break;
        }
    }

    private void InteractChest()
    {
        if (!unlocked)
        {
            Debug.Log("El cofre está cerrado.");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        Debug.Log("Cofre abierto.");
    }

    private void InteractKey()
    {
        GameManager.Instance.ObtainKey();
        gameObject.SetActive(false);
        Debug.Log("Llave obtenida.");
    }

    private void InteractDoor()
    {
        if (!GameManager.Instance.HasKey)
        {
            Debug.Log("Necesitas la llave.");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        Debug.Log("Puerta abierta. ¡Escape completado!");
    }
}
