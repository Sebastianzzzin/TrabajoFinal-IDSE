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

    // --- CARACTERÍSTICA EXCLUSIVA DEL SCRIPT 1: AUDIO ---
    [Header("--- AUDIO Y DRAMA ---")]
    public AudioSource fuenteVoz;   // Arrastra aquí el AudioSource (SFX)
    public AudioClip clipAuch;      // Sonido de golpe
    public AudioClip clipMaldicion; // Sonido de muerte dramática

    // --- CARACTERÍSTICA EXCLUSIVA DEL SCRIPT 2: ESFERAS ---
    [Header("--- ESFERAS DEL DRAGÓN ---")]
    public bool[] esferasDragon = new bool[7];

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
    private bool estaMuerto = false; // Del Script 1, para evitar doble muerte

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
        estaMuerto = false; // Resetear estado de muerte

        // 3. Actualizar HUD
        ActualizarTodoElHUD();
    }

    // --- LÓGICA DE TURBO (COMÚN EN AMBOS) ---
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

        // NOTA: Si los objetos de las Esferas tienen su propio script que llama a 
        // "AgregarEsferaDragon", no necesitas un trigger aquí. 
        // Si no, puedes añadir aquí: if(other.CompareTag("Esfera")) ...
    }

    // --- SISTEMA DE DAÑO Y SONIDO (MEJORADO DEL SCRIPT 1) ---
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

    // --- SISTEMA DE MUERTE DRAMÁTICA (DEL SCRIPT 1 - MÁS COMPLETO) ---
    void ManejarMuerte()
    {
        if (estaMuerto) return; // Evitar doble muerte
        estaMuerto = true;

        vidasRestantes--;
        PlayerPrefs.SetInt("VidasJugador", vidasRestantes);
        hud.ActualizarImagenVidas(vidasRestantes);

        Debug.Log("¡Maldición... he muerto!");

        // 1. APAGAR MOTORES (Buscamos el script de control)
        // Asegúrate que tu script de movimiento se llame exactamente "PlayerControllerGTA"
        PlayerControllerGTA vuelo = GetComponent<PlayerControllerGTA>();
        if (vuelo != null)
        {
            vuelo.enabled = false; // Quitamos control
            // Apagamos los sonidos de vuelo para que se oiga el grito
            if (vuelo.sourceVueloNormal) vuelo.sourceVueloNormal.Stop();
            if (vuelo.sourceVueloTurbo) vuelo.sourceVueloTurbo.Stop();
        }

        // 2. SONIDO: GRITO FINAL
        if (fuenteVoz != null && clipMaldicion != null)
        {
            fuenteVoz.Stop(); // Calla el "Auch" si estaba sonando
            fuenteVoz.PlayOneShot(clipMaldicion);
        }

        // 3. REINICIAR CON RETRASO
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
            // Reiniciar nivel o ir a Menu Principal
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

    // --- MÉTODOS DE ESFERAS DEL DRAGÓN (DEL SCRIPT 2) ---

    public void AgregarEsferaDragon(int numero)
    {
        if (numero < 1 || numero > 7)
        {
            Debug.LogWarning("Número de esfera inválido: " + numero);
            return;
        }

        int index = numero - 1;

        if (!esferasDragon[index])
        {
            esferasDragon[index] = true;
            Debug.Log("Goku obtuvo la esfera del dragón número " + numero);
            // Opcional: Sonido de obtener item
        }
        else
        {
            Debug.Log("Esa esfera ya fue obtenida.");
        }
    }

    public bool TieneTodasLasEsferas()
    {
        for (int i = 0; i < esferasDragon.Length; i++)
        {
            if (!esferasDragon[i]) return false;
        }
        return true;
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