using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    public GameObject ginyuPrefab;
    public Transform goku;

    // CONFIGURACION DE SPAWN
    [Header("--- Tiempos ---")]
    public float tiempoEsperaInicial = 10f; // <--- NUEVO: Cuánto tarda en salir el PRIMERO
    public float tiempoEntreSpawns = 4f;    // Cada cuánto salen después

    [Header("--- Posición ---")]
    public float distanciaMinima = 20f;
    public float dispersion = 10f;

    void Start()
    {
        // El primer número (tiempoEsperaInicial) es cuánto espera antes de empezar.
        // El segundo (tiempoEntreSpawns) es el ritmo normal.
        InvokeRepeating("CrearEnemigo", tiempoEsperaInicial, tiempoEntreSpawns);
    }

    void CrearEnemigo()
    {
        if (goku == null) return;

        // 1. Tomamos la posición 20 metros ADELANTE de Goku
        Vector3 puntoFrente = goku.position + (goku.forward * distanciaMinima);

        // 2. Le sumamos aleatoriedad
        Vector3 posicionFinal = puntoFrente + (Random.insideUnitSphere * dispersion);

        // 3. Ajustamos altura
        posicionFinal.y = goku.position.y + Random.Range(-2f, 5f);

        // 4. Crear al Ginyu
        Instantiate(ginyuPrefab, posicionFinal, Quaternion.identity);
    }
}