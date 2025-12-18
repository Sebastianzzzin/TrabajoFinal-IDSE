using UnityEngine;
using System.Collections.Generic; // Necesario para usar Listas

public class GeneradorLava : MonoBehaviour
{
 [Header("Referencias OBLIGATORIAS")]
    public GameObject prefabColumnaLava;
    public Transform jugador; 
    public PlayerControllerGTA scriptJugador; // <-- ARRASTRA AQUÍ AL JUGADOR CON EL SCRIPT

    [Header("Configuración Básica")]
    public float alturaSuelo = 34f; 
    public bool usarCarriles = true; 

    [Header("--- IA PREDICTIVA (El Castigador) ---")]
    public bool activarCastigo = true;
    public float tiempoParaCastigo = 2.0f; 
    public float toleranciaAngulo = 5.0f;
    private float cronometroRecta = 0f;
    private Vector3 ultimaDireccion;

    [Header("Ajustes de VELOCIDAD/TURBO")]
    [Tooltip("Si usa turbo, la lava aparece X veces más lejos")]
    public float multiplicadorDistanciaTurbo = 1.8f; 
    [Tooltip("Si usa turbo, el castigo se activa más rápido (0.5 = mitad de tiempo)")]
    public float factorReduccionTiempoCastigo = 0.7f; 

    [Header("Ritmo de Aparición")]
    public float distanciaMinimaEntreSpawns = 15f; 
    public float distanciaSpawnAdelante = 45f; // Distancia base (sin turbo)
    
    [Header("Dificultad")]
    [Range(0, 100)] public int probabilidadDobleColumna = 40; 
    public float separacionCarriles = 10f; 
    public float alturaMinima = 30f;
    public float alturaMaxima = 90f;

    // Variables internas
    private Vector3 ultimaPosicionJugador;
    private float distanciaRecorridaAcumulada;

    void Start()
    {
        if (jugador != null)
        {
            ultimaPosicionJugador = jugador.position;
            ultimaDireccion = jugador.forward;
        }
    }

    void Update()
    {
        if (jugador == null || scriptJugador == null) return;

        // 1. VERIFICAR SI USA TURBO
        bool enTurbo = scriptJugador.EstaUsandoTurbo;

        // ------------------------------------------------------------
        // 2. LÓGICA DE CASTIGO (ADAPTADA AL TURBO)
        // ------------------------------------------------------------
        if (activarCastigo)
        {
            float anguloCambio = Vector3.Angle(ultimaDireccion, jugador.forward);
            float velocidadAprox = (jugador.position - ultimaPosicionJugador).magnitude / Time.deltaTime;

            if (anguloCambio < toleranciaAngulo && velocidadAprox > 1f)
            {
                // Si va con turbo, el tiempo corre más rápido (se llena la barra de castigo antes)
                float factorTiempo = enTurbo ? (1f / factorReduccionTiempoCastigo) : 1f;
                cronometroRecta += Time.deltaTime * factorTiempo;

                if (cronometroRecta >= tiempoParaCastigo)
                {
                    Debug.Log(enTurbo ? "<color=red>¡CASTIGO TURBO!</color>" : "<color=orange>¡CASTIGO!</color>");
                    
                    // Si va en turbo, el castigo debe aparecer MUCHO más lejos
                    float distCastigo = enTurbo ? (distanciaSpawnAdelante * multiplicadorDistanciaTurbo) : distanciaSpawnAdelante;
                    
                    GenerarAtaqueDirecto(distCastigo); 
                    
                    cronometroRecta = 0; 
                    distanciaRecorridaAcumulada = 0; 
                }
            }
            else
            {
                cronometroRecta = 0;
            }
            ultimaDireccion = jugador.forward;
        }

        // ------------------------------------------------------------
        // 3. SPAWN NORMAL
        // ------------------------------------------------------------
        float distanciaFrame = Vector3.Distance(jugador.position, ultimaPosicionJugador);
        if (distanciaFrame > 0.01f) distanciaRecorridaAcumulada += distanciaFrame;
        
        ultimaPosicionJugador = jugador.position;

        if (distanciaRecorridaAcumulada >= distanciaMinimaEntreSpawns)
        {
            // Calculamos la distancia de spawn basada en la velocidad actual
            float distanciaFinal = enTurbo ? (distanciaSpawnAdelante * multiplicadorDistanciaTurbo) : distanciaSpawnAdelante;

            GenerarOlaNormal(distanciaFinal);
            distanciaRecorridaAcumulada = 0; 
        }
    }

    void GenerarAtaqueDirecto(float distancia)
    {
        Vector3 posicionSpawn = jugador.position + (jugador.forward * distancia);
        posicionSpawn.y = alturaSuelo;
        CrearColumnaEnPosicion(posicionSpawn, true); 
    }

    void GenerarOlaNormal(float distancia)
    {
        List<int> carrilesDisponibles = new List<int> { -1, 0, 1 };
        int cantidad = (Random.Range(0, 100) < probabilidadDobleColumna) ? 2 : 1;

        for (int i = 0; i < cantidad; i++)
        {
            if (carrilesDisponibles.Count == 0) break;
            int indice = Random.Range(0, carrilesDisponibles.Count);
            int carril = carrilesDisponibles[indice];
            carrilesDisponibles.RemoveAt(indice);

            Vector3 centroAdelante = jugador.position + (jugador.forward * distancia);
            Vector3 posicionLane = centroAdelante + (jugador.right * (carril * separacionCarriles));
            posicionLane.y = alturaSuelo;

            CrearColumnaEnPosicion(posicionLane, false);
        }
    }

    void CrearColumnaEnPosicion(Vector3 pos, bool esAtaqueDirecto)
    {
        GameObject nuevaColumna = Instantiate(prefabColumnaLava, pos, Quaternion.identity);
        ColumnaLava script = nuevaColumna.GetComponent<ColumnaLava>();
        
        if (script != null)
        {
            script.Inicializar(jugador);
            script.alturaFinal = Random.Range(alturaMinima, alturaMaxima);

            // Si es un ataque directo O el jugador va en turbo, hacemos que la lava reaccione agresiva
            if (esAtaqueDirecto || scriptJugador.EstaUsandoTurbo)
            {
                // Aumentamos un poco la velocidad de la animación para dar sensación de peligro
                script.velocidadCrecimiento += 10f; 
            }
        }
        nuevaColumna.transform.SetParent(this.transform);
    }

    void OnDrawGizmos()
    {
        if (jugador != null)
        {
            if (activarCastigo)
            {
                // La línea se pone roja si el castigo está a punto de salir
                float porcentaje = cronometroRecta / tiempoParaCastigo;
                Gizmos.color = Color.Lerp(Color.green, Color.red, porcentaje);
                Gizmos.DrawRay(jugador.position, jugador.forward * 15f);
            }
        }
    }
}