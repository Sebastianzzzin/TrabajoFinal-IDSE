using UnityEngine;

public class GeneradorPilares : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pilarPrefab;

    [Header("Configuración de Spawn")]
    public float tiempoEntreSpawns = 2.5f; // Aumenté un poco el tiempo para que no salgan tan seguidos
    public float alturaMinima = -2f;
    public float alturaMaxima = 2f;

    [Header("Espera al Inicio")]
    public float esperaInicial = 3f; // Segundos antes de que salga el PRIMER pilar

    private float contadorTiempo;

    void Start()
    {
        // Lanza el primer pilar INMEDIATAMENTE al dar Play
        SpawnearPilar();

        // Y reinicia el contador para el siguiente
        contadorTiempo = tiempoEntreSpawns;
    }

    void Update()
    {
        if (contadorTiempo <= 0)
        {
            SpawnearPilar();
            contadorTiempo = tiempoEntreSpawns;
        }
        else
        {
            contadorTiempo -= Time.deltaTime;
        }
    }

    void SpawnearPilar()
    {
        float alturaAleatoria = Random.Range(alturaMinima, alturaMaxima);
        Vector3 posicionSpawn = new Vector3(transform.position.x, alturaAleatoria, transform.position.z);
        Instantiate(pilarPrefab, posicionSpawn, Quaternion.identity);
    }
}