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

    [Header("Cámaras")] // NUEVO
    public GameObject camTopDown;
    public GameObject camTerceraPersona;

    private Rigidbody rb;
    private bool touchingObstacle = false;

    private bool modoTerceraPersona = false; // NUEVO

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // ============================================================
        // =======       DETECTAR R3 PARA CAMBIAR DE CAMARA     =======
        // ============================================================
        if (gamepad.rightStickButton.wasPressedThisFrame)
        {
            modoTerceraPersona = !modoTerceraPersona;
            camTopDown.SetActive(!modoTerceraPersona);
            camTerceraPersona.SetActive(modoTerceraPersona);
        }


        // ============================================================
        // =======           MODO TERCERA PERSONA (GTA V)       =======
        // ============================================================
        if (modoTerceraPersona)
        {
            MovimientoGTA(gamepad);
            return; // IMPORTANTE: no ejecutar el movimiento top-down
        }


        // ============================================================
        // =======         TU MOVIMIENTO ORIGINAL TOP-DOWN       =======
        // ============================================================

        Vector2 stick = gamepad.leftStick.ReadValue();
        Vector3 moveVector = new Vector3(stick.x, 0f, stick.y);

        // Activar kinematic si está tocando un obstáculo
        rb.isKinematic = touchingObstacle;

        // Movimiento horizontal
        if (moveVector.magnitude >= threshold)
        {
            moveVector.Normalize();
            Vector3 targetPos = transform.position + moveVector * moveSpeed * Time.deltaTime;

            transform.position = targetPos;
        }

        // Subir/Bajar (L1/R1)
        float vertical = 0f;
        if (gamepad.leftShoulder.isPressed) vertical -= 1f;
        if (gamepad.rightShoulder.isPressed) vertical += 1f;

        if (vertical != 0f)
        {
            Vector3 verticalMove = new Vector3(0f, vertical * verticalSpeed * Time.deltaTime, 0f);
            transform.position += verticalMove;
        }

        // Rotación top-down
        Vector2 vecRot = new Vector2(stick.x, -stick.y);
        if (vecRot.magnitude >= threshold)
        {
            float angle = Mathf.Atan2(vecRot.x, vecRot.y) * Mathf.Rad2Deg;
            if (invertirSigno) angle = -angle;

            Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ============================================================
    // ===============    ESTILO AUTO GTA V   =====================
    // ============================================================
    void MovimientoGTA(Gamepad gp)
    {
        Vector2 stick = gp.leftStick.ReadValue();

        // Avanzar / retroceder
        float forward = stick.y;
        Vector3 moveDir = transform.forward * forward * moveSpeed * Time.deltaTime;
        transform.position += moveDir;

        // Girar (solo izquierda/derecha)
        float turn = stick.x;
        if (Mathf.Abs(turn) > threshold)
        {
            float turnAmount = turn * rotationSpeed * Time.deltaTime * 10f;
            transform.Rotate(0f, turnAmount, 0f);
        }

        // Subir / Bajar (igual que antes)
        float vertical = 0f;
        if (gp.leftShoulder.isPressed) vertical -= 1f;
        if (gp.rightShoulder.isPressed) vertical += 1f;

        if (vertical != 0f)
        {
            Vector3 verticalMove = new Vector3(0f, vertical * verticalSpeed * Time.deltaTime, 0f);
            transform.position += verticalMove;
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
