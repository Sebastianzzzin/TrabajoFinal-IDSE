using UnityEngine;
using System.Collections;

public class PicoEmergente : MonoBehaviour
{
    [Header("Configuración del Pico")]
    public float alturaMinima = 0.5f; // Altura base
    public float alturaMaxima = 5.0f; // Máxima altura que alcanzará
    public float velocidadCrecimiento = 3.0f; // Velocidad de subida/bajada
    public float tiempoEsperaMax = 2.0f; // Tiempo máximo que espera en la cima
    
    [Header("Configuración de Peligro")]
    public Collider colliderDePeligro; // El Collider que daña (¡Arrastra aquí el collider!)
    public float alturaParaSerPeligroso = 3.0f; // A partir de qué altura hace daño

    private float alturaObjetivo;
    private float tiempoEspera;
    private float tiempoEsperaActual;
    private bool estaCreciendo = true;
    
    void Start()
    {
        // Asegurarse de que el pico empieza abajo
        Vector3 escala = transform.localScale;
        escala.y = alturaMinima;
        transform.localScale = escala;
        
        // El primer objetivo es crecer
        SetNuevoObjetivo();
        
        // Configuración inicial del Collider
        if (colliderDePeligro != null) colliderDePeligro.enabled = false;

        StartCoroutine(CicloCrecimiento());
    }

    void Update()
    {
        // 1. Mover la Escala
        Vector3 escalaActual = transform.localScale;
        
        // Movemos la escala hacia el objetivo
        escalaActual.y = Mathf.MoveTowards(escalaActual.y, alturaObjetivo, velocidadCrecimiento * Time.deltaTime);
        transform.localScale = escalaActual;
        
        // 2. Controlar Peligro
        ControlarPeligro();
    }

    void ControlarPeligro()
    {
        if (colliderDePeligro == null) return;
        
        // Activamos el collider si la altura excede el umbral de peligro
        if (transform.localScale.y > alturaParaSerPeligroso)
        {
            colliderDePeligro.enabled = true;
        }
        else
        {
            colliderDePeligro.enabled = false;
        }
    }

    void SetNuevoObjetivo()
    {
        if (estaCreciendo)
        {
            // Objetivo: Crecer hasta una altura aleatoria (entre la mitad y el máximo)
            alturaObjetivo = Random.Range(alturaMaxima * 0.5f, alturaMaxima);
            tiempoEspera = Random.Range(0.5f, tiempoEsperaMax);
        }
        else
        {
            // Objetivo: Bajar a la altura mínima
            alturaObjetivo = alturaMinima;
            tiempoEspera = Random.Range(0.2f, 1.0f); // Espera corta antes de volver a crecer
        }
    }
    
    IEnumerator CicloCrecimiento()
    {
        while (true)
        {
            // 1. Esperar a que el pico llegue a su objetivo (crecer o encogerse)
            yield return new WaitUntil(() => Mathf.Approximately(transform.localScale.y, alturaObjetivo));

            // 2. Esperar un tiempo en el tope/suelo
            yield return new WaitForSeconds(tiempoEspera);

            // 3. Cambiar el estado
            estaCreciendo = !estaCreciendo; // Si estaba creciendo, ahora decrece, y viceversa
            SetNuevoObjetivo(); // Establecer la nueva altura objetivo
        }
    }
}