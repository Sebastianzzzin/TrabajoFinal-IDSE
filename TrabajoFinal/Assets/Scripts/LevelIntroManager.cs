using UnityEngine;

public class LevelIntroManager : MonoBehaviour
{
    [Header("--- Configuraci�n ---")]
    public AudioSource musicaDelNivel;   // Arrastra aqu� el objeto que tiene la m�sica de fondo
    public GameObject panelDialogo;      // El objeto visual del di�logo (opcional, por si quieres asegurarte que inicie prendido)

    void Start()
    {
        // 1. CONGELAR EL TIEMPO AL INICIAR
        // Esto detiene f�sicas, animaciones y scripts dependientes del tiempo.
        Time.timeScale = 0f;

        // 2. PAUSAR M�SICA (Para que empiece con la acci�n)
        if (musicaDelNivel != null)
        {
            musicaDelNivel.Pause();
        }

        // 3. ASEGURAR QUE EL DI�LOGO SE VEA
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
        }

        Debug.Log(">>> JUEGO PAUSADO: Esperando lectura de instrucciones <<<");
    }

    // ESTA FUNCI�N ES LA QUE LLAMAR� TU SISTEMA DE DI�LOGO AL TERMINAR
    public void ComenzarNivel()
    {
        Debug.Log(">>> EL NIVEL HA COMENZADO <<<");

        // 1. DESCONGELAR EL TIEMPO
        Time.timeScale = 1f;

        // 2. ARRANCAR LA M�SICA
        if (musicaDelNivel != null)
        {
            musicaDelNivel.Play();
        }

        // 3. OCULTAR EL PANEL (Si tu sistema de dialogo no lo hace ya)
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }
}