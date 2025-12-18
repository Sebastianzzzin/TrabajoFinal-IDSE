using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{
    public GameObject ginyuPrefab;
    public Transform goku;

    // CONFIGURACION DE SPAWN
    public float tiempoEntreSpawns = 4f;
    public float distanciaMinima = 20f; // Que tan lejos aparece (20 metros)
    public float dispersion = 10f;      // Que tan "desordenados" aparecen

    void Start()
    {
        InvokeRepeating("CrearEnemigo", 2f, tiempoEntreSpawns);
    }

    void CrearEnemigo()
    {
        if (goku == null) return;

        // MATEMATICA PARA APARECER ENFRENTE
        // 1. Tomamos la posición 20 metros ADELANTE de Goku (goku.forward)
        Vector3 puntoFrente = goku.position + (goku.forward * distanciaMinima);

        // 2. Le sumamos un valor aleatorio (izquierda/derecha/arriba/abajo) para que no salgan en fila india
        Vector3 posicionFinal = puntoFrente + (Random.insideUnitSphere * dispersion);

        // 3. Forzamos que la altura (Y) sea parecida a la de Goku para que no salgan bajo tierra o en el espacio
        posicionFinal.y = goku.position.y + Random.Range(-2f, 5f);

        // 4. Crear al Ginyu
        Instantiate(ginyuPrefab, posicionFinal, Quaternion.identity);
    }
}