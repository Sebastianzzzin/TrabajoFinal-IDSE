using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para la Corrutina (IEnumerator)

public class PlayerStats : MonoBehaviour
{
    [Header("--- ESTADÍSTICAS ---")]
    public int vidaMaxima = 100;
    public float combustibleMaximo = 100f;
    public float turboMaximo = 100f;
    public int vidasIniciales = 3;

    [Header("--- DAÑO E INMUNIDAD ---")]
    public float tiempoInmunidad = 1f; // Tiempo que eres invencible tras un golpe
    private bool esInmune = false;     // Interruptor interno

    [Header("--- CONSUMO ---")]
    public float gastoCombustibleAlMover = 5f;
    public float recuperacionTurbo = 10f;

    // Variables Internas
    private int vidaActual;
    private float combustibleActual;
    private float turboActual;
    private int vidasRestantes;

    // Referencia al HUD
    public HUDController hud; 

    void Start()
    {
        // 1. Cargar Vidas o usar iniciales
        if (PlayerPrefs.HasKey("VidasJugador"))
        {
            vidasRestantes = PlayerPrefs.GetInt("VidasJugador");
        }
        else
        {
            vidasRestantes = vidasIniciales;
            PlayerPrefs.SetInt("VidasJugador", vidasRestantes);
        }

        // 2. Llenar tanques
        vidaActual = vidaMaxima;
        combustibleActual = combustibleMaximo;
        turboActual = turboMaximo;

        // 3. Actualizar la pantalla
        ActualizarTodoElHUD();
    }

    void Update()
    {
        // Recargar Turbo pasivamente
        if (turboActual < turboMaximo)
        {
            turboActual += recuperacionTurbo * Time.deltaTime;
            hud.ActualizarTurbo(turboActual, turboMaximo);
        }
    }

    // ============================================================
    // =======      LOGICA DE COLISIONES (NUEVA)            =======
    // ============================================================

    // 1. Si chocamos contra algo SÓLIDO (Collision)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            RecibirDano(20);
            Debug.Log("Golpe con Obstáculo Sólido");
        }
    }

    // 2. Si atravesamos algo TRANSPARENTE (Trigger)
    void OnTriggerEnter(Collider other)
    {
        // Daño por trampas o zonas de peligro
        if (other.CompareTag("Obstacle"))
        {
            RecibirDano(20);
            Debug.Log("Golpe con Obstáculo Trigger");
        }

        // Recoger Esferas del Dragón
        if (other.CompareTag("EsferaDragon"))
        {
            // Intentamos buscar un componente que nos diga qué número es
            // Si no tienes script en la esfera, puedes usar el nombre del objeto
            // Ejemplo: si el objeto se llama "Esfera_4", extraemos el 4.
            
            // Para este ejemplo, destruimos el objeto y damos una esfera aleatoria o fija
            // Lo ideal es que tu esfera tenga un script "ItemEsfera" con public int numero = 4;
            
            // Simulación:
            Debug.Log("Esfera recogida!");
            Destroy(other.gameObject);
            
            // hud.RecolectarEsfera(numero); <--- Descomenta esto cuando tengas el script en la esfera
        }
    }

    // ============================================================
    // =======        SISTEMA DE DAÑO E INMUNIDAD           =======
    // ============================================================

    public void RecibirDano(int dano)
    {
        // Si estamos en inmunidad, ignoramos el daño
        if (esInmune) return;

        vidaActual -= dano;
        
        // Evitar números negativos
        if (vidaActual < 0) vidaActual = 0;

        // Actualizar barra verde
        hud.ActualizarVida(vidaActual, vidaMaxima);

        if (vidaActual <= 0)
        {
            ManejarMuerte();
        }
        else
        {
            // Si seguimos vivos, activamos la inmunidad temporal
            StartCoroutine(RutinaInmunidad());
        }
    }

    IEnumerator RutinaInmunidad()
    {
        esInmune = true;
        
        // Opcional: Aquí podrías hacer parpadear al personaje
        // GetComponent<MeshRenderer>().enabled = false; ... etc
        
        yield return new WaitForSeconds(tiempoInmunidad);
        
        esInmune = false;
    }

    // ============================================================
    // =======             MUERTE Y VIDAS                   =======
    // ============================================================

    void ManejarMuerte()
    {
        vidasRestantes--;
        PlayerPrefs.SetInt("VidasJugador", vidasRestantes);
        hud.ActualizarVidas(vidasRestantes);

        if (vidasRestantes > 0)
        {
            // Reiniciar Nivel (Mantiene inventario de PlayerPrefs si lo hubiera)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("GAME OVER - Fin de la partida");
            // Borramos el guardado para empezar de 0 la próxima vez
            PlayerPrefs.DeleteKey("VidasJugador");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        }
    }

    // ============================================================
    // =======       SISTEMA DE COMBUSTIBLE Y TURBO         =======
    // ============================================================

    public bool IntentarGastarCombustible(float cantidad)
    {
        if (combustibleActual > 0)
        {
            combustibleActual -= cantidad * Time.deltaTime;
            if (combustibleActual < 0) combustibleActual = 0;
            
            hud.ActualizarCombustible(combustibleActual, combustibleMaximo);
            return true;
        }
        return false;
    }

    public bool IntentarUsarTurbo(float cantidad)
    {
        if (turboActual > 0)
        {
            turboActual -= cantidad * Time.deltaTime;
            hud.ActualizarTurbo(turboActual, turboMaximo);
            return true;
        }
        return false;
    }

    void ActualizarTodoElHUD()
    {
        if (hud == null) return;
        hud.ActualizarVida(vidaActual, vidaMaxima);
        hud.ActualizarCombustible(combustibleActual, combustibleMaximo);
        hud.ActualizarTurbo(turboActual, turboMaximo);
        hud.ActualizarVidas(vidasRestantes);
    }
}