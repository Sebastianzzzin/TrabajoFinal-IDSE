using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("--- ESTADÍSTICAS ---")]
    public int vidaMaxima = 100;
    public float combustibleMaximo = 100f;
    public float turboMaximo = 100f;
    public int vidasIniciales = 3;

    [Header("--- SISTEMA DE CARGAS TURBO ---")]
    public int cargasTurboMaximas = 5;
    private int cargasTurboActuales;

    [Header("--- AUDIO Y DRAMA (NUEVO) ---")]
    public AudioSource fuenteVoz;   // Arrastra aquí el AudioSource 3 (SFX)
    public AudioClip clipAuch;      // Arrastra el sonido de golpe
    public AudioClip clipMaldicion; // Arrastra el sonido de muerte

    [Header("--- CONSUMO ---")]
    public float gastoCombustibleAlMover = 5f;

    [Header("--- DAÑO ---")]
    public float tiempoInmunidad = 1f;
    private bool esInmune = false;

    // Variables Internas
    private int vidaActual;
    private float combustibleActual;
    private float turboActual;
    private int vidasRestantes;
    private bool estaMuerto = false; // Para evitar que te maten 2 veces

    public HUDController hud; 

    void Start()
    {
        // 1. Cargar Vidas
        if (PlayerPrefs.HasKey("VidasJugador"))
            vidasRestantes = PlayerPrefs.GetInt("VidasJugador");
        else
        {
            vidasRestantes = vidasIniciales;
            PlayerPrefs.SetInt("VidasJugador", vidasRestantes);
        }

        // 2. Inicializar Stats
        vidaActual = vidaMaxima;
        combustibleActual = combustibleMaximo;
        turboActual = turboMaximo;
        cargasTurboActuales = cargasTurboMaximas;
        estaMuerto = false;

        // 3. Actualizar HUD
        ActualizarTodoElHUD();
    }

    // --- LÓGICA DE TURBO ---
    public bool IntentarUsarTurbo(float cantidadGasto)
    {
        if (turboActual > 0)
        {
            turboActual -= cantidadGasto * Time.deltaTime;
            hud.ActualizarTurbo(turboActual, turboMaximo);
            return true;
        }
        else
        {
            if (cargasTurboActuales > 0)
            {
                cargasTurboActuales--;
                turboActual = turboMaximo;
                hud.ActualizarCargasTurbo(cargasTurboActuales);
                hud.ActualizarTurbo(turboActual, turboMaximo);
                return true;
            }
            else
            {
                turboActual = 0;
                hud.ActualizarTurbo(0, turboMaximo);
                return false;
            }
        }
    }

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

    // --- COLISIONES ---
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) RecibirDano(20);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) RecibirDano(20);
        
        if (other.CompareTag("ItemTurbo")) 
        {
            RecuperarCargaTurbo();
            Destroy(other.gameObject);
        }
    }

    // --- SISTEMA DE DAÑO Y SONIDO ---
    
    // IMPORTANTE: Asegúrate de que el script de la Lava llame a "RecibirDano" (sin Ñ)
    // O cambia el nombre de esta función a "RecibirDaño" si la lava usa la Ñ.
    public void RecibirDano(int dano)
    {
        if (esInmune || estaMuerto) return;

        vidaActual -= dano;
        if (vidaActual < 0) vidaActual = 0;
        
        hud.ActualizarVida(vidaActual, vidaMaxima);

        // SONIDO: AUCH
        if (fuenteVoz != null && clipAuch != null)
        {
            fuenteVoz.PlayOneShot(clipAuch);
        }

        if (vidaActual <= 0)
        {
            ManejarMuerte();
        }
        else
        {
            StartCoroutine(RutinaInmunidad());
        }
    }

    IEnumerator RutinaInmunidad()
    {
        esInmune = true;
        // Aquí podrías hacer que parpadee el modelo
        yield return new WaitForSeconds(tiempoInmunidad);
        esInmune = false;
    }

    // --- SISTEMA DE MUERTE DRAMÁTICA ---
    void ManejarMuerte()
    {
        if (estaMuerto) return; // Evitar doble muerte
        estaMuerto = true;

        vidasRestantes--;
        PlayerPrefs.SetInt("VidasJugador", vidasRestantes);
        hud.ActualizarImagenVidas(vidasRestantes);

        Debug.Log("¡Maldición... he muerto!");

        // 1. APAGAR MOTORES (Buscamos el otro script)
        PlayerControllerGTA vuelo = GetComponent<PlayerControllerGTA>();
        if (vuelo != null)
        {
            vuelo.enabled = false; // Quitamos control
            // Apagamos los sonidos de vuelo para que se oiga el grito
            if(vuelo.sourceVueloNormal) vuelo.sourceVueloNormal.Stop();
            if(vuelo.sourceVueloTurbo) vuelo.sourceVueloTurbo.Stop();
        }

        // 2. SONIDO: GRITO FINAL
        if (fuenteVoz != null && clipMaldicion != null)
        {
            fuenteVoz.Stop(); // Calla el "Auch" si estaba sonando
            fuenteVoz.PlayOneShot(clipMaldicion);
        }

        // 3. REINICIAR CON RETRASO (Esperamos 3 segundos)
        StartCoroutine(ReiniciarEscenaConRetraso());
    }

    IEnumerator ReiniciarEscenaConRetraso()
    {
        yield return new WaitForSeconds(3.0f); // Espera dramática

        if (vidasRestantes > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("GAME OVER REAL");
            PlayerPrefs.DeleteKey("VidasJugador");
            // Aquí podrías cargar una escena de "Menu Principal" o "GameOver"
            // Por ahora reiniciamos el nivel actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        }
    }

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
        hud.ActualizarCargasTurbo(cargasTurboActuales);
        hud.ActualizarImagenVidas(vidasRestantes);
    }
}