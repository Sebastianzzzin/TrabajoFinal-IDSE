using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerGamepad : MonoBehaviour
{
    [Header("--- AUDIO DE VUELO (CROSSFADE) ---")]
    public AudioSource sourceVueloNormal;
    public AudioSource sourceVueloTurbo;
    public float velocidadFundido = 2.0f;

    [Header("Efectos Visuales")]
    public TrailRenderer trailEstela;
    public float largoNormal = 0.3f;
    public float largoTurbo = 3.0f;
    public float velocidadCambioEstela = 5f;

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

    private PlayerStats stats;
    private float mezclaActual = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
        rb.isKinematic = false;

        if (sourceVueloNormal != null)
        {
            sourceVueloNormal.loop = true;
            sourceVueloNormal.Play();
            sourceVueloNormal.volume = 1;
        }

        if (sourceVueloTurbo != null)
        {
            sourceVueloTurbo.loop = true;
            sourceVueloTurbo.Play();
            sourceVueloTurbo.volume = 0;
        }
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
            if (DialogueManager.Instance != null && DialogueManager.Instance.isPlayerInRange)
            {
                DialogueManager.Instance.IntentarInteraccion();
            }
        }

        // ============================================================
        // 2. CAMBIO DE CÁMARA (R3)
        // ============================================================
        if (gamepad.rightStickButton.wasPressedThisFrame)
        {
            modoTerceraPersona = !modoTerceraPersona;
            camTopDown.SetActive(!modoTerceraPersona);
            camTerceraPersona.SetActive(modoTerceraPersona);
        }

        // ============================================================
        // 3. INPUT BASE DE MOVIMIENTO
        // ============================================================
        Vector2 stick = gamepad.leftStick.ReadValue();
        bool isMoving = stick.magnitude >= threshold;
        bool isTurboPressed = gamepad.rightTrigger.ReadValue() > 0.1f;

        float currentSpeed = moveSpeed;
        bool usandoTurboEfectivo = false;

        // ============================================================
        // 4. CONSUMO / TURBO / SONIDO / ESTELA
        // ============================================================
        if (isMoving && stats != null)
        {
            bool tieneGasolina = stats.IntentarGastarCombustible(stats.gastoCombustibleAlMover);

            if (!tieneGasolina)
            {
                currentSpeed = 0.5f;
            }

            // TURBO
            if (isTurboPressed && tieneGasolina)
            {
                if (stats.IntentarUsarTurbo(costoTurbo))
                {
                    currentSpeed *= turboSpeedMultiplier;
                    usandoTurboEfectivo = true;
                }
            }
        }

        // --- AUDIO CROSSFADING ---
        float objetivoMezcla = usandoTurboEfectivo ? 1.0f : 0.0f;

        mezclaActual = Mathf.MoveTowards(mezclaActual, objetivoMezcla, Time.deltaTime * velocidadFundido);

        if (sourceVueloNormal != null && sourceVueloTurbo != null)
        {
            sourceVueloNormal.volume = 1.0f - mezclaActual;
            sourceVueloTurbo.volume = mezclaActual;
        }

        // --- ESTELA ---
        if (trailEstela != null)
        {
            float targetTime = usandoTurboEfectivo ? largoTurbo : largoNormal;
            trailEstela.time = Mathf.Lerp(trailEstela.time, targetTime, Time.deltaTime * velocidadCambioEstela);
        }

        // ============================================================
        // 5. MOVIMIENTO FINAL
        // ============================================================
        if (modoTerceraPersona)
        {
            MovimientoGTA(gamepad, stick, currentSpeed);
        }
        else
        {
            MovimientoTopDown(gamepad, stick, currentSpeed);
        }
    }

    // ============================================================
    // FUNCIONES DE MOVIMIENTO
    // ============================================================

    void MovimientoTopDown(Gamepad gp, Vector2 stick, float velocidad)
    {
        Vector3 moveVector = new Vector3(stick.x, 0f, stick.y);
        rb.isKinematic = touchingObstacle;

        if (moveVector.magnitude >= threshold)
        {
            moveVector.Normalize();
            transform.position += moveVector * velocidad * Time.deltaTime;
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
