using UnityEngine;

public class ControladorPlaneta : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform jugador;          // ARRASTRA A TU JUGADOR AQUÍ
    public Transform centroDelPlaneta; 
    public GameObject puaPrefab;

    [Header("Configuración Hostil")]
    [Range(0, 1)] 
    public float agresividad = 0.7f;   // 70% de las púas atacan al jugador, 30% son aleatorias
    public float anticipacion = 3f;    // Qué tan adelante del jugador aparecen (Predicción)
    public float dispersion = 1f;      // "Mala puntería" para que no salgan todas en fila india

    [Header("Configuración General")]
    public int cantidadPuasSimultaneas = 30;
    public float distanciaOrbital = 100f;
    public LayerMask capaPlaneta;

    [Header("Variación de las Puas")]
    public float alturaMin = 2f;
    public float alturaMax = 6f;
    public float velocidadMin = 0.5f;
    public float velocidadMax = 1.5f;

    void Start()
    {
        if (centroDelPlaneta == null) centroDelPlaneta = transform;

        // Llenar el pool inicial
        for (int i = 0; i < cantidadPuasSimultaneas; i++)
        {
            SolicitarNuevaPua(); // Al inicio serán aleatorias o cercanas según suerte
        }
    }

    // Llamado por las púas al morir
    public void SolicitarNuevaPua()
    {
        bool posicionEncontrada = false;
        int intentos = 0;

        while (!posicionEncontrada && intentos < 10)
        {
            intentos++;
            
            // --- AQUÍ ESTÁ LA INTELIGENCIA ARTIFICIAL ---
            Vector3 direccionDeseada;

            // Tiramos un dado. Si sale menor a la agresividad, ATACAMOS.
            if (jugador != null && Random.value < agresividad)
            {
                // 1. Predecir dónde estará el jugador: Posición actual + (Dirección * Distancia)
                Vector3 posicionPredicha = jugador.position + (jugador.forward * anticipacion);

                // 2. Calcular el vector desde el centro del planeta hacia esa predicción
                Vector3 direccionAlJugador = (posicionPredicha - centroDelPlaneta.position).normalized;

                // 3. Añadir "Dispersion" (Error humano) para que salgan alrededor, no solo en un punto exacto
                Vector3 errorAleatorio = Random.insideUnitSphere * dispersion;
                
                // Combinamos y normalizamos
                direccionDeseada = (direccionAlJugador + errorAleatorio).normalized;
            }
            else
            {
                // Si no ataca, sale en cualquier lado del planeta (ambiente)
                direccionDeseada = Random.onUnitSphere;
            }

            // --- LÓGICA DE RAYCAST (Igual que antes) ---
            // Nos vamos al espacio en esa dirección deseada
            Vector3 origenRayo = centroDelPlaneta.position + (direccionDeseada * distanciaOrbital);

            RaycastHit hit;
            // Disparamos hacia el centro
            if (Physics.Raycast(origenRayo, -direccionDeseada, out hit, distanciaOrbital + 50f, capaPlaneta))
            {
                CrearPua(hit.point, direccionDeseada);
                posicionEncontrada = true;
            }
        }
    }

    void CrearPua(Vector3 posicion, Vector3 direccionSalida)
    {
        GameObject nuevaPua = Instantiate(puaPrefab, posicion, Quaternion.identity);
        nuevaPua.transform.parent = centroDelPlaneta;
        
        // La púa mira hacia afuera
        nuevaPua.transform.up = direccionSalida;

        // Configuración aleatoria
        float alturaRandom = Random.Range(alturaMin, alturaMax);
        float velocidadRandom = Random.Range(velocidadMin, velocidadMax);

        // Inicializar lógica de vida
        PuaDinamica scriptPua = nuevaPua.GetComponent<PuaDinamica>();
        if (scriptPua != null)
        {
            scriptPua.Inicializar(this, alturaRandom, velocidadRandom); // Nota: tuve que cambiar el tipo en PuaDinamica abajo
        }
    }
}