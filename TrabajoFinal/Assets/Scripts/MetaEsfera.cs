using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MetaEsfera : MonoBehaviour
{
    [Header("Animación")]
    public float velocidadGiro = 100f;    // Qué tan rápido da vueltas
    public float velocidadFlote = 2f;     // Qué tan rápido sube y baja
    public float alturaFlote = 0.5f;      // Qué tanto se mueve de arriba a abajo

    [Header("Configuración de Nivel")]
    public string nombreSiguienteEscena = "Nivel2"; // Escribe aquí el nombre EXACTO de tu siguiente escena

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos la posición donde pusiste la moneda para que flote alrededor de ahí
        posicionInicial = transform.position;
    }

    void Update()
    {
        // 1. GIRAR (Rotación constante sobre el eje Y)
        transform.Rotate(Vector3.up * velocidadGiro * Time.deltaTime);

        // 2. FLOTAR (Movimiento de ola suave usando Seno)
        // Matemáticas: Sin(tiempo) crea una ola que va de -1 a 1.
        float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlote) * alturaFlote;
        
        // Aplicamos la nueva altura manteniendo la X y Z originales
        transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
    }


}