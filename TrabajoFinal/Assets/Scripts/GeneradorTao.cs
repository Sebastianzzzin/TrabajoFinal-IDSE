using System.Collections;
using UnityEngine;

public class GeneradorTao : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject taoPrefab;
    public Transform gokuTransform;

    [Header("Posición en Cámara")]
    [Tooltip("Distancia desde el borde derecho. 0 es el borde exacto. 0.5 es un poco adentro.")]
    public float margenDesdeBorde = 0.5f;

    [Tooltip("Ajuste de altura (Y) relativo a Goku")]
    public float offsetAltura = 0f;

    [Header("Tiempos")]
    public float tiempoEsperaInicial = 10f; // <--- NUEVO: Espera antes de empezar el ataque
    public float tiempoMinimo = 2f;
    public float tiempoMaximo = 5f;

    void Start()
    {
        if (gokuTransform == null)
        {
            Debug.LogError("¡Falta asignar a Goku en el Inspector!");
            return;
        }
        StartCoroutine(RutinaDeGeneracion());
    }

    IEnumerator RutinaDeGeneracion()
    {
        // 1. ESPERA INICIAL (Lectura de instrucciones)
        yield return new WaitForSeconds(tiempoEsperaInicial);

        // 2. BUCLE INFINITO DE ATAQUE
        while (true)
        {
            // Espera aleatoria entre cada Tao
            float espera = Random.Range(tiempoMinimo, tiempoMaximo);
            yield return new WaitForSeconds(espera);

            Camera cam = Camera.main;

            // Calculamos el borde derecho exacto de la pantalla
            Vector3 bordeDerechoCamara = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 10));

            // Posición X: Borde derecho MENOS el margen pequeño
            float spawnX = bordeDerechoCamara.x - margenDesdeBorde;

            // Posición Y: La altura de Goku
            float spawnY = gokuTransform.position.y + offsetAltura;

            Vector3 posicionSpawn = new Vector3(spawnX, spawnY, 0);

            Instantiate(taoPrefab, posicionSpawn, taoPrefab.transform.rotation);
        }
    }
}