using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("Referencias")]
    public CanvasGroup grupoCanvas;   // El CanvasGroup del padre (Controla transparencia de TODO)
    public GameObject pantallaCarga;  // El objeto 'PantallaCarga_Goku'

    [Header("Configuración")]
    public float duracionFade = 0.5f; 
    public float tiempoMinimoMinijuego = 5.0f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CargarNivel(string nombreEscena)
    {
        StartCoroutine(TransicionSuave(nombreEscena));
    }

    IEnumerator TransicionSuave(string escena)
    {
        // 1. Bloquear clics para que no toquen nada más
        grupoCanvas.blocksRaycasts = true;

        // ---------------------------------------------------------
        // FADE OUT (Aparición de la pantalla de carga)
        // ---------------------------------------------------------
        
        // Encendemos a Goku YA, pero como el Alpha es 0, no se ve todavía.
        pantallaCarga.SetActive(true);

        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            // Vamos de Transparente a Opaco (Goku va apareciendo)
            grupoCanvas.alpha = Mathf.Lerp(0, 1, tiempo / duracionFade);
            yield return null;
        }
        grupoCanvas.alpha = 1; // Opacidad total

        // ---------------------------------------------------------
        // CARGA Y MINIJUEGO
        // ---------------------------------------------------------

        AsyncOperation operacion = SceneManager.LoadSceneAsync(escena);
        operacion.allowSceneActivation = false; // "Carga en secreto"

        float tiempoJugando = 0f;

        // Esperamos a que cargue Y a que pasen los 5 segundos
        while (operacion.progress < 0.9f || tiempoJugando < tiempoMinimoMinijuego)
        {
            tiempoJugando += Time.deltaTime;
            yield return null;
        }

        // ---------------------------------------------------------
        // CAMBIO DE ESCENA (Bajo el telón)
        // ---------------------------------------------------------
        
        // ¡OJO AQUÍ! NO apagamos a Goku todavía.
        // Permitimos que la escena cambie DETRÁS de la pantalla de carga.
        operacion.allowSceneActivation = true;

        // Esperamos a que Unity termine de colocar los objetos nuevos
        while (!operacion.isDone)
        {
            yield return null;
        }

        // ---------------------------------------------------------
        // FADE IN (Revelando el Nivel)
        // ---------------------------------------------------------
        
        // Ahora mismo la escena nueva ya está cargada, pero Goku la está tapando.
        // Vamos a hacer transparente a Goku (y al fondo negro) suavemente.

        tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            // Vamos de Opaco (1) a Transparente (0)
            grupoCanvas.alpha = Mathf.Lerp(1, 0, tiempo / duracionFade);
            yield return null;
        }
        grupoCanvas.alpha = 0; // Transparencia total
        
        // AHORA SÍ, que ya no se ve nada, apagamos el objeto para ahorrar recursos
        pantallaCarga.SetActive(false);
        grupoCanvas.blocksRaycasts = false;
    }
}