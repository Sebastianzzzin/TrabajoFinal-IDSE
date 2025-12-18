using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerKaio : MonoBehaviour
{
    [Header("Referencias OBLIGATORIAS")]
    public Transform planetaCentro;
    public Transform camaraTransform;
    public PlayerStats stats;

    [Header("Ajustes de la Nube")]
    public float alturaFlotacion = 2.5f;
    public float velocidadMovimiento = 8f;
    public float velocidadTurboMultiplicador = 2.5f;
    public float costoTurbo = 30f;
    public float velocidadRotacion = 10f; 

    [Header("Ajustes de Física")]
    public float fuerzaGravedad = 50f;
    public float amortiguacionVertical = 5f; 

    [Header("Audio y FX")]
    public AudioSource sourceVueloNormal;
    public AudioSource sourceVueloTurbo;
    public float velocidadFundido = 2.0f;
    public TrailRenderer trailEstela;
    public float largoNormal = 0.3f;
    public float largoTurbo = 3.0f;
    
    private Rigidbody rb;
    private Vector2 inputMovimiento;
    private float mezclaActual = 0f;
    
    public bool EstaUsandoTurbo { get; private set; } 

    [Header("Ajustes de Nivel Especial")]
    public bool modoCombustibleInfinito = false; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // CONFIGURACIÓN OBLIGATORIA DEL RIGIDBODY
        rb.useGravity = false;
        rb.isKinematic = false; 
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Aseguramos que el drag no nos frene de golpe
        rb.linearDamping = 1f; // En Unity viejos esto es 'drag', ver nota abajo
        rb.angularDamping = 1f;

        if (sourceVueloNormal) { sourceVueloNormal.loop = true; sourceVueloNormal.Play(); sourceVueloNormal.volume = 1; }
        if (sourceVueloTurbo) { sourceVueloTurbo.loop = true; sourceVueloTurbo.Play(); sourceVueloTurbo.volume = 0; }
    }

    void Update()
    {
        LeerInput();
    }

    void FixedUpdate()
    {
        // 1. CHEQUEO DE SEGURIDAD: Si esto falta, no nos movemos
        if (planetaCentro == null || camaraTransform == null)
        {
            Debug.LogError("¡FALTAN REFERENCIAS EN EL PLAYER CONTROLLER! Asigna Planeta y Cámara.");
            return;
        }

        AplicarGravedadYFlotacion();
        AplicarMovimientoYRotacion();
        GestionarEfectos();
    }

    void LeerInput()
    {
        inputMovimiento = Vector2.zero;
        EstaUsandoTurbo = false;
        
        // Lectura de Gamepad
        if (Gamepad.current != null)
        {
            inputMovimiento = Gamepad.current.leftStick.ReadValue();
            bool isTurboPressed = Gamepad.current.rightTrigger.ReadValue() > 0.1f;
            if (isTurboPressed && stats != null && inputMovimiento.magnitude > 0.1f)
            {
                if (stats.IntentarUsarTurbo(costoTurbo)) EstaUsandoTurbo = true;
            }
        }
        
        // Lectura de Teclado (Si no hay gamepad o es cero)
        if (inputMovimiento == Vector2.zero) 
        {
            if (Keyboard.current.wKey.isPressed) inputMovimiento.y += 1;
            if (Keyboard.current.sKey.isPressed) inputMovimiento.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputMovimiento.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputMovimiento.x += 1;
            inputMovimiento.Normalize();
            
            if (Keyboard.current.spaceKey.isPressed && stats != null && inputMovimiento.magnitude > 0.1f)
            {
                if (stats.IntentarUsarTurbo(costoTurbo)) EstaUsandoTurbo = true;
            }
        }
    }

    void AplicarGravedadYFlotacion()
    {
        Vector3 direccionArriba = (transform.position - planetaCentro.position).normalized;
        Vector3 direccionGravedad = -direccionArriba;

        // Gravedad Artificial
        rb.AddForce(direccionGravedad * fuerzaGravedad, ForceMode.Acceleration);

        // Amortiguación (Freno vertical para no rebotar)
        // NOTA: Usamos rb.velocity para compatibilidad máxima (funciona en Unity 6 y 2022)
        float velocidadVertical = Vector3.Dot(rb.linearVelocity, direccionArriba);

        if (Mathf.Abs(velocidadVertical) > 0.01f)
        {
            Vector3 fuerzaFreno = -direccionArriba * velocidadVertical * amortiguacionVertical;
            rb.AddForce(fuerzaFreno, ForceMode.Acceleration);
        }

        // Flotación (Raycast)
        RaycastHit hit;
        if (Physics.Raycast(rb.position, direccionGravedad, out hit, 10f))
        {
            float distanciaAlSuelo = hit.distance;
            float errorAltura = distanciaAlSuelo - alturaFlotacion;
            Vector3 fuerzaAjuste = direccionGravedad * (errorAltura * fuerzaGravedad * 0.5f);
            rb.AddForce(fuerzaAjuste, ForceMode.Acceleration);
        }
    }

    void AplicarMovimientoYRotacion()
    {
        Vector3 direccionArriba = (transform.position - planetaCentro.position).normalized;
        float velocidadActual = EstaUsandoTurbo ? velocidadMovimiento * velocidadTurboMultiplicador : velocidadMovimiento;
        
        Vector3 direccionDeseada = Vector3.zero;

        if (inputMovimiento.magnitude > 0.1f)
        {
            Vector3 camFwd = Vector3.ProjectOnPlane(camaraTransform.forward, direccionArriba).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(camaraTransform.right, direccionArriba).normalized;

            direccionDeseada = (camFwd * inputMovimiento.y + camRight * inputMovimiento.x).normalized;
            
            // Lógica de Combustible (Solo si no es infinito)
             if (stats != null && !modoCombustibleInfinito) 
            {
                stats.IntentarGastarCombustible(stats.gastoCombustibleAlMover);
            }
        }

        // --- FÍSICA DE MOVIMIENTO ---
        Vector3 velocidadTarget = direccionDeseada * velocidadActual;
        
        // Usamos rb.velocity (más compatible que linearVelocity)
        Vector3 velocidadHorizontalActual = Vector3.ProjectOnPlane(rb.linearVelocity, direccionArriba);

        // Aplicamos fuerza para alcanzar la velocidad deseada
        Vector3 fuerzaMovimiento = (velocidadTarget - velocidadHorizontalActual) * 20f; 
        rb.AddForce(fuerzaMovimiento, ForceMode.Acceleration);
        
        // --- ROTACIÓN ---
        Quaternion rotacionGravedad = Quaternion.FromToRotation(transform.up, direccionArriba) * transform.rotation;
        
        if (direccionDeseada != Vector3.zero)
        {
            Quaternion rotacionMirada = Quaternion.LookRotation(direccionDeseada, direccionArriba);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionMirada, velocidadRotacion * 50f * Time.fixedDeltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionGravedad, Time.fixedDeltaTime * velocidadRotacion);
        }
    }

    void GestionarEfectos()
    {
        float objetivo = EstaUsandoTurbo ? 1.0f : 0.0f;
        mezclaActual = Mathf.MoveTowards(mezclaActual, objetivo, Time.deltaTime * velocidadFundido);

        if (sourceVueloNormal && sourceVueloTurbo)
        {
            sourceVueloNormal.volume = 1.0f - mezclaActual;
            sourceVueloTurbo.volume = mezclaActual;
        }

        if (trailEstela)
        {
            trailEstela.time = Mathf.Lerp(trailEstela.time, EstaUsandoTurbo ? largoTurbo : largoNormal, Time.deltaTime * 5f);
        }
    }
}