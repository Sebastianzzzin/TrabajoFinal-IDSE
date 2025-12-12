using UnityEngine;

public class GeneradorLava : MonoBehaviour
{
    public GameObject prefabColumnaLava;
    public Transform jugador; 

    [Header("Tiempo")]
    public float tiempoEntreSpawns = 0.5f; // Lo bajé para que salgan más y se vea más lleno
    private float cronometro;

    [Header("Zona de Aparición")]
    public float distanciaMinima = 10f; 
    public float distanciaMaxima = 30f; // Aumenté esto para que se vea un "mar" de columnas a lo lejos
    public float anchoDelCamino = 15f; 
    public float alturaSuelo = 0f;

    void Update()
    {
        if (jugador == null) return;

        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreSpawns)
        {
            GenerarColumnaAdelante();
            cronometro = 0;
        }
    }

    void GenerarColumnaAdelante()
    {
        float distanciaAdelante = Random.Range(distanciaMinima, distanciaMaxima);
        float desviacionLateral = Random.Range(-anchoDelCamino / 2f, anchoDelCamino / 2f);

        Vector3 posicionSpawn = jugador.position + 
                                (jugador.forward * distanciaAdelante) + 
                                (jugador.right * desviacionLateral);

        posicionSpawn.y = alturaSuelo;

        // --- CAMBIO AQUÍ ---
        // Guardamos la referencia del objeto creado
        GameObject nuevaColumna = Instantiate(prefabColumnaLava, posicionSpawn, Quaternion.identity);
        
        // Obtenemos su script y le pasamos el jugador
        ColumnaLava scriptLava = nuevaColumna.GetComponent<ColumnaLava>();
        if (scriptLava != null)
        {
            scriptLava.Inicializar(jugador);
        }
        // -------------------
    }

    void OnDrawGizmos()
    {
        if (jugador != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 inicioZona = jugador.position + (jugador.forward * distanciaMinima);
            Vector3 finZona = jugador.position + (jugador.forward * distanciaMaxima);
            Gizmos.DrawWireCube((inicioZona + finZona) / 2, new Vector3(anchoDelCamino, 1, distanciaMaxima - distanciaMinima));
        }
    }
}