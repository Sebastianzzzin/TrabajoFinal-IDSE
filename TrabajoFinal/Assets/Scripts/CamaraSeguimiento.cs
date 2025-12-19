using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    [Header("Objetivo a Seguir")]
    public Transform objetivo; // Arrastra a Goku aquí

    [Header("Configuración de Límites")]
    [Tooltip("La altura máxima (Y) a la que puede subir la cámara, aunque Goku siga subiendo.")]
    public float alturaMaxima = 10f;

    [Header("Suavizado")]
    [Tooltip("Qué tan rápido sigue la cámara al personaje (valores altos = más rígido).")]
    public float velocidadSuavizado = 5f;

    // Distancia inicial entre la cámara y Goku (para mantener la vista lateral)
    private Vector3 offsetInicial;

    void Start()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("¡No has asignado a Goku en el script CamaraSeguimiento!");
            return;
        }

        // Calculamos la diferencia de posición inicial (la distancia X, Y, Z)
        // Esto mantiene la perspectiva lateral exacta que ya acomodaste en la escena.
        offsetInicial = transform.position - objetivo.position;
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        // 1. Calculamos dónde DEBERÍA estar la cámara si siguiera a Goku normalmente
        Vector3 posicionDeseada = objetivo.position + offsetInicial;

        // 2. Aplicamos el TOPE DE ALTURA (Eje Y)
        // Mathf.Min elige el valor más pequeño. Si la posición deseada es 20 pero el tope es 15, se queda en 15.
        // Si la posición es 5 y el tope es 15, se queda en 5 (sigue subiendo normal).
        posicionDeseada.y = Mathf.Min(posicionDeseada.y, alturaMaxima);

        // 3. Mantenemos el eje X fijo (Profundidad) por seguridad si es un juego 2.5D estricto
        // Si tu cámara debe acercarse o alejarse, borra esta línea.
        // Si debe ser un riel perfecto, déjala.
        // posicionDeseada.x = transform.position.x; 

        // 4. Movemos la cámara suavemente hacia esa posición calculada
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.deltaTime);
    }
}