using UnityEngine;
using System.Collections;

public class CamaraGTA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;

    [Header("Configuración")]
    public float distancia = 5.0f;
    public float sensibilidadX = 150.0f; 
    public float sensibilidadY = 150.0f;
    
    [Header("Límites")]
    public float limiteMinY = -20f;
    public float limiteMaxY = 80f;

    [Header("Colisión")]
    public LayerMask capasDeColision;
    public float velocidadAjustePared = 10f;

    // Estado interno
    private float distanciaActual;
    private float rotacionX = 0.0f;
    private float rotacionY = 0.0f;
    private Vector2 inputRecibido;
    
    private Vector3 offsetVibracion; 

    void Start()
    {
        distanciaActual = distancia;

        // --- MODIFICACIÓN AQUÍ ---
        // En lugar de tomar la rotación actual de la cámara, forzamos los valores:
        
        // rotacionX controla el eje Y (Horizontal). Lo ponemos en -180.
        rotacionX = -180f; 

        // rotacionY controla el eje X (Vertical). 
        // Puedes ponerlo en 0, o en 20 para que empiece mirando un poco hacia abajo (estilo GTA).
        rotacionY = 10f; 

        // Si no hay target, avisamos
        if (target == null) Debug.LogWarning("¡Falta asignar el Target en la Cámara!");
    }

    public void RecibirInput(Vector2 input)
    {
        inputRecibido = input;
    }

    public void ActivarVibracion(float duracion, float fuerza)
    {
        StopAllCoroutines(); 
        StartCoroutine(RutinaVibracion(duracion, fuerza));
    }

    private IEnumerator RutinaVibracion(float duracion, float fuerza)
    {
        float tiempo = 0;
        while (tiempo < duracion)
        {
            offsetVibracion = Random.insideUnitSphere * fuerza;
            tiempo += Time.deltaTime;
            yield return null;
        }
        offsetVibracion = Vector3.zero; 
    }

    void LateUpdate()
    {
        if (!target) return;

        // El input del jugador se suma a la rotación inicial que definimos en Start
        float inputX = inputRecibido.x * sensibilidadX * Time.deltaTime;
        float inputY = inputRecibido.y * sensibilidadY * Time.deltaTime;

        rotacionX += inputX;
        rotacionY -= inputY; 
        rotacionY = Mathf.Clamp(rotacionY, limiteMinY, limiteMaxY);

        // Aquí se crea la rotación final
        Quaternion rotacion = Quaternion.Euler(rotacionY, rotacionX, 0);
        
        Vector3 posicionDeseada = rotacion * new Vector3(0.0f, 0.0f, -distancia) + target.position;
        Vector3 direccionHaciaCamara = posicionDeseada - target.position;
        
        RaycastHit hit;
        // Se usa la SphereCast para detectar paredes
        if (Physics.SphereCast(target.position, 0.2f, direccionHaciaCamara.normalized, out hit, distancia, capasDeColision))
        {
            distanciaActual = hit.distance;
        }
        else
        {
            distanciaActual = Mathf.Lerp(distanciaActual, distancia, Time.deltaTime * velocidadAjustePared);
        }

        Vector3 posicionBase = rotacion * new Vector3(0.0f, 0.0f, -distanciaActual) + target.position;

        transform.rotation = rotacion;
        transform.position = posicionBase + offsetVibracion;
    }
}