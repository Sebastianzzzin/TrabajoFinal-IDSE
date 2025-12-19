using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Esto obliga a Unity a poner una bocina (AudioSource) si no la tienes
[RequireComponent(typeof(AudioSource))]
public class ControladorTiempo : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoInicial = 50f;
    private float tiempoRestante;
    private bool juegoTerminado = false;

    [Header("Referencias")]
    public TextMeshProUGUI textoContador;

    // CAMBIO 1: Quitamos el Panel y ponemos el Sonido
    public AudioClip sonidoDerrota; // Arrastra tu sonido aquí
    private AudioSource bocina;

    void Start()
    {
        tiempoRestante = tiempoInicial;

        // Obtenemos la bocina automáticamente
        bocina = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoUI();
        }
        else
        {
            tiempoRestante = 0;
            ActualizarTextoUI();
            PerderJuego();
        }
    }

    void ActualizarTextoUI()
    {
        if (textoContador != null)
        {
            float segundos = Mathf.FloorToInt(tiempoRestante);
            float milisegundos = Mathf.FloorToInt((tiempoRestante * 100) % 100);
            textoContador.text = string.Format("{0:00}:{1:00}", segundos, milisegundos);
        }
    }

    void PerderJuego()
    {
        juegoTerminado = true;
        Debug.Log("¡Tiempo Agotado!");

        // CAMBIO 2: Reproducir el sonido
        if (sonidoDerrota != null && bocina != null)
        {
            bocina.PlayOneShot(sonidoDerrota);
        }

        // Detener el juego
        // Nota: El sonido se seguirá escuchando aunque el tiempo sea 0
        Time.timeScale = 0f;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}