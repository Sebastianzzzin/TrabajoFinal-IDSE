using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerGamepad : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float turboSpeedMultiplier = 2f; // Velocidad x2 con Turbo
    public float verticalSpeed = 3f;
    public float threshold = 0.2f;

    [Header("Consumo")]
    public float costoTurbo = 30f; // Gasta 30 de turbo por segundo

    [Header("Rotación")]
    public float rotationSpeed = 10f;
    public bool invertirSigno = true;

    [Header("Cámaras")]
    public GameObject camTopDown;
    public GameObject camTerceraPersona;

    private Rigidbody rb;
    private bool touchingObstacle = false;
    private bool modoTerceraPersona = false;

    // REFERENCIA NUEVA
    private PlayerStats stats; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>(); // Buscamos el script de vida/gasolina
        rb.isKinematic = false;
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Cambio de cámara (R3)
        if (gamepad.rightStickButton.wasPressedThisFrame)
        {
            modoTerceraPersona = !modoTerceraPersona;
            camTopDown.SetActive(!modoTerceraPersona);
            camTerceraPersona.SetActive(modoTerceraPersona);
        }

        // Leer Input
        Vector2 stick = gamepad.leftStick.ReadValue();
        bool isMoving = stick.magnitude >= threshold;
        bool isTurboPressed = gamepad.rightTrigger.ReadValue() > 0.1f; 

        // Factor de velocidad (Normal o Turbo)
        float currentSpeed = moveSpeed;

        // Lógica de Consumo
        if (isMoving && stats != null)
        {
            // 1. Intentar Gastar Gasolina Normal
            bool tieneGasolina = stats.IntentarGastarCombustible(stats.gastoCombustibleAlMover);
            
            if (!tieneGasolina)
            {
                // Si no hay gasolina, nos movemos muuuuy lento o nada
                currentSpeed = 0.5f; 
            }

            // 2. Lógica Turbo (Solo si aprietas botón y tienes barra azul)
            if (isTurboPressed && tieneGasolina)
            {
                if (stats.IntentarUsarTurbo(costoTurbo))
                {
                    currentSpeed *= turboSpeedMultiplier; // Aceleron
                }
            }
        }

        // Ejecutar Movimiento (GTA o TopDown)
        if (modoTerceraPersona)
        {
            MovimientoGTA(gamepad, stick, currentSpeed);
        }
        else
        {
            MovimientoTopDown(gamepad, stick, currentSpeed);
        }
    }

    // --- HE SEPARADO TUS MOVIMIENTOS PARA QUE SE LEA MEJOR ---

    void MovimientoTopDown(Gamepad gp, Vector2 stick, float velocidad)
    {
        Vector3 moveVector = new Vector3(stick.x, 0f, stick.y);
        rb.isKinematic = touchingObstacle;

        if (moveVector.magnitude >= threshold)
        {
            moveVector.Normalize();
            Vector3 targetPos = transform.position + moveVector * velocidad * Time.deltaTime;
            transform.position = targetPos;
        }

        // Altura (Sin cambios)
        ControlarAltura(gp);

        // Rotación
        Vector2 vecRot = new Vector2(stick.x, -stick.y);
        if (vecRot.magnitude >= threshold)
        {
            float angle = Mathf.Atan2(vecRot.x, vecRot.y) * Mathf.Rad2Deg;
            if (invertirSigno) angle = -angle;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, angle, 0f), rotationSpeed * Time.deltaTime);
        }
    }

    void MovimientoGTA(Gamepad gp, Vector2 stick, float velocidad)
    {
        // Avanzar
        float forward = stick.y;
        Vector3 moveDir = transform.forward * forward * velocidad * Time.deltaTime;
        transform.position += moveDir;

        // Girar
        float turn = stick.x;
        if (Mathf.Abs(turn) > threshold)
        {
            float turnAmount = turn * rotationSpeed * Time.deltaTime * 10f;
            transform.Rotate(0f, turnAmount, 0f);
        }

        ControlarAltura(gp);
    }

    void ControlarAltura(Gamepad gp)
    {
        float vertical = 0f;
        if (gp.leftShoulder.isPressed) vertical -= 1f;
        if (gp.rightShoulder.isPressed) vertical += 1f;

        if (vertical != 0f)
        {
            transform.position += new Vector3(0f, vertical * verticalSpeed * Time.deltaTime, 0f);
        }
    }
}