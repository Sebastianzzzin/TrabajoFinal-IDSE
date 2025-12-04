using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform jugador;

    [Header("Top Down")]
    public Vector3 offsetTopDown = new Vector3(0, 15f, -8f);
    public Vector3 rotacionTopDown = new Vector3(60f, 0f, 0f);

    [Header("Suavizado")]
    public float suavizado = 8f;

    private Gamepad gamepad;

    void Start()
    {
        // Rotación fija top-down
        transform.rotation = Quaternion.Euler(rotacionTopDown);
    }

    void Update()
    {
        gamepad = Gamepad.current;
        if (gamepad == null) return;

        // (Antes el joystick derecho se usaba aquí, ahora no hace nada)
        // Lo dejamos vacío para evitar cualquier bug.
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        // Movimiento suave hacia el offset
        Vector3 objetivo = jugador.position + offsetTopDown;

        transform.position = Vector3.Lerp(
            transform.position,
            objetivo,
            suavizado * Time.deltaTime
        );

        // Mantiene la cámara mirando hacia abajo
        transform.rotation = Quaternion.Euler(rotacionTopDown);
    }
}
