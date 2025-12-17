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
    // La fuerza de gravedad esférica
    public float fuerzaGravedad = 50f;
    // La amortiguación, qué tan rápido la velocidad vertical vuelve a 0 (SOLUCIÓN VIBRACIÓN)
    public float amortiguacionVertical = 5f; 

    // Audio y Estelas (Mismos que antes)
    // ... (Mantener todas las variables de Audio y Estelas) ...
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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // ¡Volvemos a Física Real!
        rb.useGravity = false;
        rb.isKinematic = false; // <-- Vuelve a ser FALSE
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (sourceVueloNormal) { sourceVueloNormal.loop = true; sourceVueloNormal.Play(); sourceVueloNormal.volume = 1; }
        if (sourceVueloTurbo) { sourceVueloTurbo.loop = true; sourceVueloTurbo.Play(); sourceVueloTurbo.volume = 0; }
    }

    void Update()
    {
        LeerInput();
    }

    void FixedUpdate()
    {
        if (planetaCentro == null || camaraTransform == null) return;

        AplicarGravedadYFlotacion();
        AplicarMovimientoYRotacion();
        GestionarEfectos();
    }

    void LeerInput()
    {
        // (La lógica de lectura de Input y Turbo es la misma de la V4 y funciona bien)
        inputMovimiento = Vector2.zero;
        EstaUsandoTurbo = false;
        
        if (Gamepad.current != null)
        {
            inputMovimiento = Gamepad.current.leftStick.ReadValue();
            bool isTurboPressed = Gamepad.current.rightTrigger.ReadValue() > 0.1f;
            if (isTurboPressed && stats != null && inputMovimiento.magnitude > 0.1f)
            {
                if (stats.IntentarUsarTurbo(costoTurbo)) EstaUsandoTurbo = true;
            }
        }
        
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
        Vector3 direccionGravedad = -direccionArriba; // Hacia el centro

        // 1. APLICAR GRAVEDAD CONSTANTE (Para que luche contra el turbo)
        rb.AddForce(direccionGravedad * fuerzaGravedad, ForceMode.Acceleration);

        // 2. AMORTIGUACIÓN VERTICAL (¡SOLUCIÓN VIBRACIÓN!)
        // Buscamos la velocidad en la dirección "arriba/abajo" del planeta
        float velocidadVertical = Vector3.Dot(rb.linearVelocity, direccionArriba);

        // Si la velocidad vertical no es 0, aplicamos una fuerza de freno
        if (Mathf.Abs(velocidadVertical) > 0.01f)
        {
            // Queremos llevar la velocidad vertical a 0 de forma suave (freno)
            Vector3 fuerzaFreno = -direccionArriba * velocidadVertical * amortiguacionVertical;
            rb.AddForce(fuerzaFreno, ForceMode.Acceleration);
        }

        // 3. CORRECCIÓN DE ALTURA (Para que flote exactamente a alturaFlotacion)
        RaycastHit hit;
        // Lanzamos rayo desde el cuerpo hacia el centro (gravedad)
        if (Physics.Raycast(rb.position, direccionGravedad, out hit, 10f))
        {
            float distanciaAlSuelo = hit.distance;
            float errorAltura = distanciaAlSuelo - alturaFlotacion;

            // Si estamos muy lejos (error positivo) o muy cerca (error negativo), aplicamos un ajuste suave
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
            // Calcular dirección de cámara proyectada en la esfera
            Vector3 camFwd = Vector3.ProjectOnPlane(camaraTransform.forward, direccionArriba).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(camaraTransform.right, direccionArriba).normalized;

            direccionDeseada = (camFwd * inputMovimiento.y + camRight * inputMovimiento.x).normalized;
            
            // Gasto de combustible normal
            if (stats != null) stats.IntentarGastarCombustible(stats.gastoCombustibleAlMover);
        }

        // Aplicamos el movimiento directamente a la velocidad lineal del Rigidbody
        // Solo afectamos la componente *horizontal* (tangente a la esfera)
        Vector3 velocidadTarget = direccionDeseada * velocidadActual;
        
        // Obtenemos la velocidad horizontal actual (proyectada en el plano tangente)
        Vector3 velocidadHorizontalActual = Vector3.ProjectOnPlane(rb.linearVelocity, direccionArriba);

        // Calculamos la fuerza para llevar la velocidad horizontal actual a la velocidadTarget
        Vector3 fuerzaMovimiento = (velocidadTarget - velocidadHorizontalActual) * 20f; // El '20f' es un valor de aceleración
        rb.AddForce(fuerzaMovimiento, ForceMode.Acceleration);
        
        
        // --- ROTACIÓN (SOLUCIÓN BUG 180 GRADOS) ---
        Quaternion rotacionGravedad = Quaternion.FromToRotation(transform.up, direccionArriba) * transform.rotation;
        
        if (direccionDeseada != Vector3.zero)
        {
            Quaternion rotacionMirada = Quaternion.LookRotation(direccionDeseada, direccionArriba);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionMirada, velocidadRotacion * 50f * Time.fixedDeltaTime);
        }
        else
        {
            // Si no hay input, solo alineamos al suelo
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionGravedad, Time.fixedDeltaTime * velocidadRotacion);
        }
    }

    void GestionarEfectos()
    {
        // (Lógica de Audio y Trail Estela de la V4)
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