using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerGamepad : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 3f;
    public float threshold = 0.2f;

    [Header("Rotación")]
    public float rotationSpeed = 10f;
    public bool invertirSigno = true;

    [Header("Detección de colisión")]
    public float collisionCheckDistance = 0.5f;

    private Rigidbody rb;
    private bool touchingObstacle = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Por defecto dinámico
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        Vector2 stick = gamepad.leftStick.ReadValue();
        Vector3 moveVector = new Vector3(stick.x, 0f, stick.y);

        // ======= Activar kinematic si está tocando un obstáculo =======
        rb.isKinematic = touchingObstacle;

        // ======= Movimiento horizontal =======
        if (moveVector.magnitude >= threshold)
        {
            moveVector.Normalize();
            Vector3 targetPos = transform.position + moveVector * moveSpeed * Time.deltaTime;

            transform.position = targetPos;
        }

        // ======= Subir/Bajar (L1/R1) =======
        float vertical = 0f;
        if (gamepad.leftShoulder.isPressed) vertical -= 1f;
        if (gamepad.rightShoulder.isPressed) vertical += 1f;

        if (vertical != 0f)
        {
            Vector3 verticalMove = new Vector3(0f, vertical * verticalSpeed * Time.deltaTime, 0f);
            transform.position += verticalMove;
        }

        // ======= Rotación =======
        Vector2 vecRot = new Vector2(stick.x, -stick.y);
        if (vecRot.magnitude >= threshold)
        {
            float angle = Mathf.Atan2(vecRot.x, vecRot.y) * Mathf.Rad2Deg;
            if (invertirSigno) angle = -angle;

            Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ======= Detectar colisiones =======
    void OnCollisionEnter(Collision collision)
    {
        touchingObstacle = true;
    }

    void OnCollisionExit(Collision collision)
    {
        touchingObstacle = false;
    }
}
