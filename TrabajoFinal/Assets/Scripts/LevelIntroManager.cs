using UnityEngine;

public class LevelIntroManager : MonoBehaviour
{
    [Header("--- Configuración ---")]
    public AudioSource musicaDelNivel;   // Arrastra aquí el objeto que tiene la música de fondo
    public GameObject panelDialogo;      // El objeto visual del diálogo (opcional, por si quieres asegurarte que inicie prendido)

    void Start()
    {
        // 1. CONGELAR EL TIEMPO AL INICIAR
        // Esto detiene físicas, animaciones y scripts dependientes del tiempo.
        Time.timeScale = 0f;

        // 2. PAUSAR MÚSICA (Para que empiece con la acción)
        if (musicaDelNivel != null)
        {
            musicaDelNivel.Pause();
        }

        // 3. ASEGURAR QUE EL DIÁLOGO SE VEA
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(true);
        }

        Debug.Log(">>> JUEGO PAUSADO: Esperando lectura de instrucciones <<<");
    }

    // ESTA FUNCIÓN ES LA QUE LLAMARÁ TU SISTEMA DE DIÁLOGO AL TERMINAR
    public void ComenzarNivel()
    {
        Debug.Log(">>> EL NIVEL HA COMENZADO <<<");

        // 1. DESCONGELAR EL TIEMPO
        Time.timeScale = 1f;

        // 2. ARRANCAR LA MÚSICA
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