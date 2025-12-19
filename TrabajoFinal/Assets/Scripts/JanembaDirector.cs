using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JanembaDirector : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Image panelNegroNarrativo; // El mismo panel negro del Canvas
    public float velocidadFade = 1f;

    [Header("Datos de Paikuhan")]
    public string nombreNarrador = "Paikuhan";
    public Sprite caraPaikuhan; // Arrastra la cara de Paikuhan aquí

    [Header("Diálogos")]
    [TextArea(3, 5)]
    public string[] dialogoIntro; 
    [TextArea(3, 5)]
    public string[] dialogoFinal; 

    [Header("Referencias")]
    // Nombre EXACTO de la escena del Menú o Créditos (tal cual está en Build Settings)
    public string escenaSiguiente = "Menu Principal"; 

    // Variable para controlar si el jugador puede moverse (Opcional, si quieres bloquearlo)
    public static bool juegoEnCurso = false;

    void Start()
    {
        // Bloqueamos el juego al inicio
        juegoEnCurso = false;

        // 1. Pantalla negra inicial
        panelNegroNarrativo.gameObject.SetActive(true);
        panelNegroNarrativo.color = Color.black; 

        // 2. Arrancar Intro
        StartCoroutine(SecuenciaIntro());
    }

    IEnumerator SecuenciaIntro()
    {
        yield return new WaitForSeconds(1f);

        // 3. Paikuhan habla
        bool dialogoTerminado = false;
        DialogueManager.Instance.IniciarDialogoNarrativo(
            nombreNarrador, 
            caraPaikuhan, 
            dialogoIntro, 
            () => { dialogoTerminado = true; }
        );

        yield return new WaitUntil(() => dialogoTerminado);

        // 4. Fade In (Luz)
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadFade;
            panelNegroNarrativo.color = new Color(0, 0, 0, 1f - t);
            yield return null;
        }
        panelNegroNarrativo.gameObject.SetActive(false);

        // 5. ¡A JUGAR!
        juegoEnCurso = true;
        Debug.Log("¡Corre Goku!");
    }

    // --- ESTA FUNCIÓN LA LLAMARÁ LA CÚPULA AL TOCARLA ---
    public void LlegadaALaMeta()
    {
        if (!juegoEnCurso) return; // Evitar ganar dos veces
        juegoEnCurso = false; // Detener lógica del juego si fuera necesario

        StartCoroutine(SecuenciaFinal());
    }

    IEnumerator SecuenciaFinal()
    {
        // 1. Fade Out a Negro
        panelNegroNarrativo.gameObject.SetActive(true);
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadFade;
            panelNegroNarrativo.color = new Color(0, 0, 0, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // 2. Diálogo Final de Paikuhan
        bool dialogoTerminado = false;
        DialogueManager.Instance.IniciarDialogoNarrativo(
            nombreNarrador, 
            caraPaikuhan, 
            dialogoFinal, 
            () => { dialogoTerminado = true; }
        );

        yield return new WaitUntil(() => dialogoTerminado);

        // 3. Cargar Menu con pantalla de Goku comiendo
        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.CargarNivel(escenaSiguiente);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
        }
    }
}