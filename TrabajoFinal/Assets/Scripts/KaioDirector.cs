using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KaioDirector : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Image panelNegroNarrativo; // Un Panel negro (UI) que cubre toda la pantalla
    public float velocidadFade = 1f;

    [Header("Datos de Kaio-Sama")]
    public string nombreKaio = "Kaio-Sama";
    public Sprite caraKaio; // Arrastra la cara de Kaio aquí

    [Header("Diálogos")]
    [TextArea(3, 5)]
    public string[] dialogoIntro; // "Goku, el planeta se deforma..."
    [TextArea(3, 5)]
    public string[] dialogoFinal; // "Bien hecho, ahora ve al infierno..."

    [Header("Referencias")]
    public SurvivalLevelManager survivalManager;
    public string nombreEscenaInfierno = "NivelJanemba"; // Nombre EXACTO de la siguiente escena

    void Start()
    {
        // 1. Al iniciar, aseguramos que la pantalla esté negra y bloqueada
        panelNegroNarrativo.gameObject.SetActive(true);
        panelNegroNarrativo.color = Color.black; 

        // 2. Iniciamos la secuencia
        StartCoroutine(SecuenciaIntro());
    }

    IEnumerator SecuenciaIntro()
    {
        yield return new WaitForSeconds(1f); // Pequeña pausa dramática en negro

        // 3. Kaio habla (mientras todo sigue negro o medio oscuro)
        // Usamos una variable bool para esperar a que el diálogo termine
        bool dialogoTerminado = false;
        
        DialogueManager.Instance.IniciarDialogoNarrativo(
            nombreKaio, 
            caraKaio, 
            dialogoIntro, 
            () => { dialogoTerminado = true; } // Esto se ejecuta al cerrar el cuadro
        );

        // Esperamos a que el jugador termine de leer
        yield return new WaitUntil(() => dialogoTerminado);

        // 4. Fade In (La pantalla negra se vuelve transparente)
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadFade;
            panelNegroNarrativo.color = new Color(0, 0, 0, 1f - t); // Alpha baja a 0
            yield return null;
        }
        panelNegroNarrativo.gameObject.SetActive(false); // Apagamos el panel

        // 5. ¡A JUGAR!
        survivalManager.IniciarSupervivencia();
    }

    // Esta función la llama el SurvivalLevelManager cuando acaba el tiempo
    public void NivelCompletado()
    {
        StartCoroutine(SecuenciaFinal());
    }

    IEnumerator SecuenciaFinal()
    {
        // 1. Fade Out a Negro (Suave)
        panelNegroNarrativo.gameObject.SetActive(true);
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadFade;
            panelNegroNarrativo.color = new Color(0, 0, 0, t); // Alpha sube a 1
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // 2. Diálogo Final (Con pantalla negra de fondo)
        bool dialogoTerminado = false;
        DialogueManager.Instance.IniciarDialogoNarrativo(
            nombreKaio, 
            caraKaio, 
            dialogoFinal, 
            () => { dialogoTerminado = true; }
        );

        yield return new WaitUntil(() => dialogoTerminado);

        // 3. Cargar Nivel con el sistema de Platos
        // Usamos el LevelLoader que ya tienes
        LevelLoader.Instance.CargarNivel(nombreEscenaInfierno);
    }
}