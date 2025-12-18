using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerGTA : MonoBehaviour
{
    [Header("--- AUDIO DE VUELO (CROSSFADE) ---")]
    public AudioSource sourceVueloNormal; // Arrastra el AudioSource 1
    public AudioSource sourceVueloTurbo;  // Arrastra el AudioSource 2
    public float velocidadFundido = 2.0f; // Qué tan rápido cambia el sonido

    [Header("Conexión con Cámara")]
    public CamaraGTA scriptCamara;

    [Header("Efectos Visuales")]
    public TrailRenderer trailEstela;
    public float largoNormal = 0.3f;
    public float largoTurbo = 3.0f;
    public float velocidadCambioEstela = 5f;

    [Header("Configuración Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float turboSpeedMultiplier = 2f;
    public float verticalSpeed = 3f;
    public float costoTurbo = 30f;
    
    private Rigidbody rb;
    private PlayerStats stats;
    private bool puedeMoverse = true; 
    public bool EstaUsandoTurbo { get; private set; } 
    
    // Variable para controlar la mezcla (0 = Normal, 1 = Turbo)
    private float mezclaActual = 0f; 
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
        if (sourceVueloNormal != null)
        {
            sourceVueloNormal.loop = true;
            sourceVueloNormal.Play();
            sourceVueloNormal.volume = 1; // Empieza sonando
        }

        if (sourceVueloTurbo != null)
        {
            sourceVueloTurbo.loop = true;
            sourceVueloTurbo.Play();
            sourceVueloTurbo.volume = 0; // Empieza silenciado
        }
    }

    // --- FUNCIÓN DE STUN (La que usa el rebote) ---
    public void RecibirImpactoRebote(float tiempoAturdimiento)
    {
        StartCoroutine(RutinaAturdimiento(tiempoAturdimiento));
    }

    private IEnumerator RutinaAturdimiento(float tiempo)
    {
        puedeMoverse = false; 
        yield return new WaitForSeconds(tiempo);
        // rb.linearVelocity = Vector3.zero; // Comentado por si acaso, como dijimos antes
        puedeMoverse = true;  
    }
    // ---------------------------------------------

        void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        if (scriptCamara != null) scriptCamara.RecibirInput(gamepad.rightStick.ReadValue());
        if (!puedeMoverse) return; 

        if (gamepad.buttonNorth.wasPressedThisFrame && DialogueManager.Instance != null && DialogueManager.Instance.isPlayerInRange)
            DialogueManager.Instance.IntentarInteraccion();

        Vector2 inputMove = gamepad.leftStick.ReadValue();
        float velocidadActual = moveSpeed;

        // --- LÓGICA TURBO ---
        bool isTurboPressed = gamepad.rightTrigger.ReadValue() > 0.1f;
        bool isMoving = inputMove.magnitude > 0.1f;
        bool usandoTurboEfectivo = false; 

        if (isMoving && stats != null)
        {
            bool tieneGasolina = stats.IntentarGastarCombustible(stats.gastoCombustibleAlMover);
            if (!tieneGasolina) velocidadActual = 0.5f;

            if (isTurboPressed && tieneGasolina)
            {
                if (stats.IntentarUsarTurbo(costoTurbo))
                {
                    velocidadActual *= turboSpeedMultiplier;
                    usandoTurboEfectivo = true;
                    EstaUsandoTurbo = usandoTurboEfectivo; 
                }
            }
        }

        // --- GESTIÓN DE AUDIO (CROSSFADE) ---
        // Definimos el objetivo: 1 si hay turbo, 0 si no
        float objetivoMezcla = usandoTurboEfectivo ? 1.0f : 0.0f;
        
        // Movemos la mezcla suavemente hacia el objetivo
        mezclaActual = Mathf.MoveTowards(mezclaActual, objetivoMezcla, Time.deltaTime * velocidadFundido);

        if (sourceVueloNormal != null && sourceVueloTurbo != null)
        {
            // El normal suena más cuando mezcla es 0
            sourceVueloNormal.volume = 1.0f - mezclaActual; 
            // El turbo suena más cuando mezcla es 1
            sourceVueloTurbo.volume = mezclaActual;
        }

        // --- GESTIÓN ESTELA ---
        if (trailEstela != null)
        {
            float targetTime = usandoTurboEfectivo ? largoTurbo : largoNormal;
            trailEstela.time = Mathf.Lerp(trailEstela.time, targetTime, Time.deltaTime * velocidadCambioEstela);
        }

        MoverseComoGTA(inputMove, velocidadActual);
        ControlarAltura(gamepad);
    }
    
    void MoverseComoGTA(Vector2 input, float velocidad)
    {
        if (input.magnitude < 0.1f || scriptCamara == null) return;

        Transform camTransform = scriptCamara.transform;
        Vector3 camFwd = camTransform.forward;
        Vector3 camRight = camTransform.right;

        camFwd.y = 0;
        camRight.y = 0;
        camFwd.Normalize();
        camRight.Normalize();

        Vector3 direccionDeseada = (camFwd * input.y + camRight * input.x).normalized;
        transform.position += direccionDeseada * velocidad * Time.deltaTime;

        if (direccionDeseada != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionDeseada);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, rotationSpeed * Time.deltaTime);
        }
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