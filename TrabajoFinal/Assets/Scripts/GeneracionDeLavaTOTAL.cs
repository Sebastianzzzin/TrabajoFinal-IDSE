using UnityEngine;

public class GeneracionDeLavaTotal : MonoBehaviour
{
    public GameObject prefabColumnaLava;
    public Transform jugador; 

    [Header("Tiempo")]
    public float tiempoEntreSpawns = 0.5f; 
    private float cronometro;

    [Header("Límites del Escenario (Coordenadas Globales)")]
    // Define aquí el tamaño de tu mapa en el mundo
    public float minX = -50f;
    public float maxX = 50f;
    public float minZ = -50f;
    public float maxZ = 50f;
    
    public float alturaSuelo = 0f;

    // Opcional: Para que no aparezca lava JUSTO encima del jugador y sea injusto
    public float zonaSeguraAlrededorJugador = 5f; 

    void Update()
    {
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreSpawns)
        {
            GenerarColumnaAleatoria();
            cronometro = 0;
        }
    }

    void GenerarColumnaAleatoria()
    {
        // 1. Elegimos una posición totalmente al azar dentro de los límites
        float posX = Random.Range(minX, maxX);
        float posZ = Random.Range(minZ, maxZ);

        Vector3 posicionSpawn = new Vector3(posX, alturaSuelo, posZ);

        // (OPCIONAL) Validación: Si cae demasiado cerca del jugador, cancelamos este spawn
        // para evitar que aparezca justo encima de su cabeza de forma injusta.
        if (jugador != null && Vector3.Distance(posicionSpawn, jugador.position) < zonaSeguraAlrededorJugador)
        {
            return; // Saltamos este turno
        }

        // 2. Instanciamos la columna
        GameObject nuevaColumna = Instantiate(prefabColumnaLava, posicionSpawn, Quaternion.identity);
        
        // 3. Inicializamos (mantenemos esto por si la columna necesita lógica interna)
        ColumnaLava scriptLava = nuevaColumna.GetComponent<ColumnaLava>();
        if (scriptLava != null && jugador != null)
        {
            scriptLava.Inicializar(jugador);
        }
    }

    void OnDrawGizmos()
    {
        // Dibujamos el área total del escenario en Verde para diferenciarlo
        Gizmos.color = Color.green;
        
        // Calculamos el centro y el tamaño para dibujar el cubo
        Vector3 centro = new Vector3((minX + maxX) / 2, alturaSuelo, (minZ + maxZ) / 2);
        Vector3 tamano = new Vector3(maxX - minX, 1, maxZ - minZ);
        
        Gizmos.DrawWireCube(centro, tamano);
    }
}
