using UnityEngine;

public class GeneradorAmbiental : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabColumnaLava;
    public int cantidadDeColumnas = 50; 
    
    [Header("Tamaño del Mapa")]
    public float radioDelMapa = 140f; 
    public float zonaSeguraCentro = 15f; 

    [Header("Variedad")]
    public float alturaMinima = 4f;
    public float alturaMaxima = 12f;

    void Start()
    {
        GenerarMapa();
    }

    void GenerarMapa()
    {
        for (int i = 0; i < cantidadDeColumnas; i++)
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioDelMapa;

            if (puntoAleatorio.magnitude < zonaSeguraCentro)
            {
                puntoAleatorio = puntoAleatorio.normalized * (zonaSeguraCentro + 5f);
            }

            // MODIFICACIÓN PEQUEÑA: 
            // Sumamos 'transform.position' para que si mueves este objeto,
            // la zona de aparición se mueva con él.
            Vector3 posicionSpawn = transform.position + new Vector3(puntoAleatorio.x, 0, puntoAleatorio.y);

            GameObject nuevaColumna = Instantiate(prefabColumnaLava, posicionSpawn, Quaternion.identity);
            
            ColumnaLava script = nuevaColumna.GetComponent<ColumnaLava>();
            if (script != null)
            {
                script.esPermanente = true; 
                script.alturaFinal = Random.Range(alturaMinima, alturaMaxima);
            }
            
            nuevaColumna.transform.SetParent(this.transform);
        }
    }

    // --- AQUÍ ESTÁ LA MAGIA DE LOS GIZMOS ---
    void OnDrawGizmosSelected()
    {
        // 1. Dibujar el Límite Exterior (Tu Cúpula)
        Gizmos.color = Color.yellow;
        // Usamos DrawWireSphere para ver el contorno
        Gizmos.DrawWireSphere(transform.position, radioDelMapa);

        // 2. Dibujar la Zona Segura (El centro)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zonaSeguraCentro);
    }
}