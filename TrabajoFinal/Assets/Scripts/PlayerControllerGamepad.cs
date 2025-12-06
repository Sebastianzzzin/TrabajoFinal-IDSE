using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerGamepad : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float turboSpeedMultiplier = 2f; 
    public float verticalSpeed = 3f;
    public float threshold = 0.2f;

    [Header("Consumo")]
    public float costoTurbo = 30f; 

    [Header("Rotación")]
    public float rotationSpeed = 10f;
    public bool invertirSigno = true;

    [Header("Cámaras")]
    public GameObject camTopDown;
    public GameObject camTerceraPersona;

    private Rigidbody rb;
    private bool touchingObstacle = false;
    private bool modoTerceraPersona = false;

    // REFERENCIAS
    private PlayerStats stats; 
    // No necesitamos referencia al DialogueManager aquí porque usamos su Singleton (Instance)

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>(); 
        rb.isKinematic = false;
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // ============================================================
        // 1. DETECTAR DIÁLOGO (TRIÁNGULO / BUTTON NORTH)
        // ============================================================
        if (gamepad.buttonNorth.wasPressedThisFrame)
        {
            // Verificamos si el DialogueManager existe y si está listo para hablar
            if (DialogueManager.Instance != null && DialogueManager.Instance.isPlayerInRange)
            {
                Debug.Log("Player ha presionado Triángulo -> Enviando señal al DialogueManager");
                DialogueManager.Instance.IntentarInteraccion();
                
                // Opcional: Si quieres que no se mueva mientras habla, pon un return aquí
                // if (DialogueManager.Instance.isDialogueActive) return;
            }
        }

        // ============================================================
        // 2. LÓGICA DE MOVIMIENTO (Solo si no estamos en diálogo, opcional)
        // ============================================================
        
        // Cambio de cámara (R3)
        if (gamepad.rightStickButton.wasPressedThisFrame)
        {
            modoTerceraPersona = !modoTerceraPersona;
            camTopDown.SetActive(!modoTerceraPersona);
            camTerceraPersona.SetActive(modoTerceraPersona);
        }

        // Leer Input de movimiento
        Vector2 stick = gamepad.leftStick.ReadValue();
        bool isMoving = stick.magnitude >= threshold;
        bool isTurboPressed = gamepad.rightTrigger.ReadValue() > 0.1f; 

        float currentSpeed = moveSpeed;

        // Lógica de Consumo y Turbo
        if (isMoving && stats != null)
        {
            bool tieneGasolina = stats.IntentarGastarCombustible(stats.gastoCombustibleAlMover);
            
            if (!tieneGasolina)
            {
                currentSpeed = 0.5f; 
            }

            if (isTurboPressed && tieneGasolina)
            {
                if (stats.IntentarUsarTurbo(costoTurbo))
                {
                    currentSpeed *= turboSpeedMultiplier; 
                }
            }
        }

        // Ejecutar Movimiento
        if (modoTerceraPersona)
        {
            MovimientoGTA(gamepad, stick, currentSpeed);
        }
        else
        {
            MovimientoTopDown(gamepad, stick, currentSpeed);
        }
    }

    // --- FUNCIONES DE MOVIMIENTO (Sin cambios) ---

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
        ControlarAltura(gp);
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
        float forward = stick.y;
        Vector3 moveDir = transform.forward * forward * velocidad * Time.deltaTime;
        transform.position += moveDir;
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