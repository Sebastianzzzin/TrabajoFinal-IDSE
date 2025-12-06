using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("--- ESTADÍSTICAS ---")]
    public int vidaMaxima = 100;
    public float combustibleMaximo = 100f;
    public float turboMaximo = 100f; // Tamaño de la barra azul
    public int vidasIniciales = 3;

    [Header("--- SISTEMA DE CARGAS TURBO ---")]
    public int cargasTurboMaximas = 5; // Cuántas flechas verdes tienes
    private int cargasTurboActuales;

    [Header("--- CONSUMO ---")]
    public float gastoCombustibleAlMover = 5f;
    // Eliminada la recuperación pasiva de turbo, ahora es por cargas

    [Header("--- DAÑO ---")]
    public float tiempoInmunidad = 1f;
    private bool esInmune = false;

    // Variables Internas
    private int vidaActual;
    private float combustibleActual;
    private float turboActual;
    private int vidasRestantes;

    public HUDController hud; 

    void Start()
    {
        // 1. Cargar Vidas
        if (PlayerPrefs.HasKey("VidasJugador"))
            vidasRestantes = PlayerPrefs.GetInt("VidasJugador");
        else
            vidasRestantes = vidasIniciales;
            PlayerPrefs.SetInt("VidasJugador", vidasRestantes);

        // 2. Inicializar Stats
        vidaActual = vidaMaxima;
        combustibleActual = combustibleMaximo;
        turboActual = turboMaximo;
        
        // Empezamos con todas las flechas verdes
        cargasTurboActuales = cargasTurboMaximas;

        // 3. Actualizar HUD
        ActualizarTodoElHUD();
    }

    // --- LÓGICA DE TURBO (MODIFICADA) ---
    public bool IntentarUsarTurbo(float cantidadGasto)
    {
        // 1. Si tenemos barra azul, la gastamos
        if (turboActual > 0)
        {
            turboActual -= cantidadGasto * Time.deltaTime;
            hud.ActualizarTurbo(turboActual, turboMaximo);
            return true; // Estamos usando turbo
        }
        // 2. Si la barra azul llegó a 0... ¡RECARGA AUTOMÁTICA!
        else
        {
            if (cargasTurboActuales > 0)
            {
                // Gastamos una flecha verde
                cargasTurboActuales--;
                
                // Rellenamos la barra azul a tope
                turboActual = turboMaximo;
                
                // Actualizamos HUD (Barra llena y una flecha menos)
                hud.ActualizarCargasTurbo(cargasTurboActuales);
                hud.ActualizarTurbo(turboActual, turboMaximo);
                
                return true; // Seguimos usando turbo gracias a la recarga
            }
            else
            {
                // No queda barra azul NI cargas verdes. Se acabó.
                turboActual = 0;
                hud.ActualizarTurbo(0, turboMaximo);
                return false;
            }
        }
    }

    // --- RESTO DEL CÓDIGO (Combustible, Daño, Muerte) ---

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) RecibirDano(20);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) RecibirDano(20);
        
        // PODRÍAS PONER ITEMS PARA RECUPERAR CARGAS TURBO
        if (other.CompareTag("ItemTurbo")) 
        {
            RecuperarCargaTurbo();
            Destroy(other.gameObject);
        }
    }

    public void RecibirDano(int dano)
    {
        if (esInmune) return;
        vidaActual -= dano;
        if (vidaActual < 0) vidaActual = 0;
        hud.ActualizarVida(vidaActual, vidaMaxima);

        if (vidaActual <= 0) ManejarMuerte();
        else StartCoroutine(RutinaInmunidad());
    }

    IEnumerator RutinaInmunidad()
    {
        esInmune = true;
        yield return new WaitForSeconds(tiempoInmunidad);
        esInmune = false;
    }

    void ManejarMuerte()
    {
        vidasRestantes--;
        PlayerPrefs.SetInt("VidasJugador", vidasRestantes);
        
        // Actualizar la imagen de vidas antes de reiniciar
        hud.ActualizarImagenVidas(vidasRestantes);

        if (vidasRestantes > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("GAME OVER");
            PlayerPrefs.DeleteKey("VidasJugador");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        }
    }

    // Función extra por si quieres poner items que te den cargas
    public void RecuperarCargaTurbo()
    {
        if (cargasTurboActuales < cargasTurboMaximas)
        {
            cargasTurboActuales++;
            hud.ActualizarCargasTurbo(cargasTurboActuales);
        }
    }

    void ActualizarTodoElHUD()
    {
        if (hud == null) return;
        hud.ActualizarVida(vidaActual, vidaMaxima);
        hud.ActualizarCombustible(combustibleActual, combustibleMaximo);
        hud.ActualizarTurbo(turboActual, turboMaximo);
        
        // Nuevas actualizaciones
        hud.ActualizarCargasTurbo(cargasTurboActuales);
        hud.ActualizarImagenVidas(vidasRestantes);
    }
}