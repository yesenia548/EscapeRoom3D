using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float speed = 5f;                    //VELOCIDAD DE MOVIMIENTO DEL JUGADOR

    [SerializeField] private Transform cameraPivot;         //Donde se encuentra la cámara (para rotarla con el mouse)
    [SerializeField] private float mouseSensitivity = 2f;

    private Rigidbody rb;
    private float pitch;
    private Vector3 desiredMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0f, v);
        if (movement.sqrMagnitude > 1f) movement.Normalize();

        if (rb == null)
        {
            // Versión original de la guía (sin Rigidbody).
            transform.Translate(movement * speed * Time.deltaTime);
            return;
        }

        // Con Rigidbody se mueve por velocidad: así las paredes y la puerta SÍ bloquean.
        desiredMove = transform.TransformDirection(movement) * speed;           // TransformDirection convierte el vector de movimiento de local a global
    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(desiredMove.x, v.y, desiredMove.z);
    }

    private void Look()
    {
        bool blocked = GameManager.UiBlocked;
        Cursor.lockState = blocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = blocked;
        if (blocked)
        {
            desiredMove = Vector3.zero;   // con el panel abierto no se camina
            return;
        }
        if (cameraPivot == null) return;

        transform.Rotate(0f, Input.GetAxis("Mouse X") * mouseSensitivity, 0f);
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity, -80f, 80f);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
