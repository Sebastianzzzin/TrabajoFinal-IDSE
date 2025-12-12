using UnityEngine;
using System.Collections;

public class CamaraSacudida : MonoBehaviour
{
    // Singleton simple para llamarlo desde cualquier lado fácilmente
    public static CamaraSacudida Instance;

    private Vector3 posicionOriginal;
    private float tiempoRestante;
    private float poderSacudida;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Si hay tiempo restante, movemos la cámara a lo loco
        if (tiempoRestante > 0)
        {
            // Generamos una posición aleatoria muy pequeña dentro de una esfera de radio 1
            transform.localPosition = posicionOriginal + Random.insideUnitSphere * poderSacudida;
            tiempoRestante -= Time.deltaTime;
        }
        else
        {
            tiempoRestante = 0f;
            // IMPORTANTE: Si tienes un script que sigue al jugador, 
            // esto podría necesitar ajustes, pero para empezar funciona bien.
            // transform.localPosition = posicionOriginal; 
        }
    }

    // Llama a esta función para activar el temblor
    public void Sacudir(float duracion, float magnitud)
    {
        posicionOriginal = transform.localPosition;
        tiempoRestante = duracion;
        poderSacudida = magnitud;
    }
}