using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Necesario si quieres mostrar el timer en texto

public class SurvivalLevelManager : MonoBehaviour
{
    [Header("Configuración del Desafío")]
    public float tiempoParaSobrevivir = 60f;
    public string nombreEscenaSiguiente = "MenuPrincipal";

    [Header("Castigo por Inactividad")]
    public float tiempoMaximoQuieto = 2.0f;
    public int danoPorInactividad = 5;
    public float umbralMovimiento = 0.5f; 

    [Header("Referencias")]
    public PlayerControllerKaio playerController;
    public PlayerStats playerStats;
    public TextMeshProUGUI textoTemporizador; 

    [Header("Control Cinemática")]
    public bool juegoIniciado = false; 
    public KaioDirector directorEscena;

    // Variables internas
    private float cronometroNivel;
    private float cronometroInactividad;
    private Rigidbody playerRb;
    private bool nivelCompletado = false;

    void Start()
    {
        // 1. Configuración Inicial
        cronometroNivel = tiempoParaSobrevivir;
        
        if(playerController != null)
        {
            playerRb = playerController.GetComponent<Rigidbody>();

        }
    }

    void Update()
    {
         if (!juegoIniciado || nivelCompletado) return;

        ManejarTemporizador();
        VerificarInactividad();
    }

    void ManejarTemporizador()
    {
        cronometroNivel -= Time.deltaTime;

        // Actualizar UI si existe
        if (textoTemporizador != null)
        {
            textoTemporizador.text = "SOBREVIVE: " + Mathf.Ceil(cronometroNivel).ToString() + "s";
        }

        // CONDICIÓN DE VICTORIA
        if (cronometroNivel <= 0)
        {
            Victoria();
        }
    }

    void VerificarInactividad()
    {
        if (playerRb == null || playerStats == null) return;

        // Medimos la velocidad horizontal (ignorando caídas verticales por gravedad si quieres ser estricto)
        // O usamos linearVelocity.magnitude para cualquier movimiento.
        // Usaremos magnitud total para obligarlo a moverse de verdad.
        float velocidadActual = playerRb.linearVelocity.magnitude;

        // Si se mueve más lento que el umbral (está quieto)
        if (velocidadActual < umbralMovimiento)
        {
            cronometroInactividad += Time.deltaTime;

            // Alerta visual en consola (opcional)
            // Debug.Log($"¡MUEVETE! {cronometroInactividad:F2}");

            if (cronometroInactividad >= tiempoMaximoQuieto)
            {
                CastigarJugador();
            }
        }
        else
        {
            // Si se mueve, reiniciamos el contador
            cronometroInactividad = 0f;
        }
    }

    void CastigarJugador()
    {
        // Aplicar daño
        playerStats.RecibirDano(danoPorInactividad);
        
        Debug.Log("¡Castigo por inactividad! -5 HP");

        // Reiniciamos el contador para que no le baje 5 de vida CADA FRAME (eso lo mataría en 0.1 segundos)
        // Ahora tiene otros 2 segundos para reaccionar antes del siguiente castigo.
        cronometroInactividad = 0f; 
    }

    void Victoria()
    {
        nivelCompletado = true;
        Debug.Log("¡TIEMPO CUMPLIDO!");
        
        // En lugar de cargar escena, avisamos al Director
        if (directorEscena != null)
        {
            directorEscena.NivelCompletado();
        }
    }
    public void IniciarSupervivencia()
    {
        juegoIniciado = true;
        if (playerController != null) playerController.modoCombustibleInfinito = true;
    }
}