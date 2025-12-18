using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("Animación")]
    public Animator animadorGuerreros; 

    [Header("Cosas para OCULTAR")]
    public GameObject textoStart;   

    [Header("Cosas para MOSTRAR")]
    public GameObject grupoBotones; 

    [Header("Navegación")]
    public GameObject primerBoton; 

    [Header("Audio")]
    public AudioSource sonidoStart; 
    
    // ESTADOS
    private bool enIntro = true;
    private bool bloqueoInput = false; // <--- EL CANDADO NUEVO

    void Start()
    {
        // 1. Resetear estados
        textoStart.SetActive(true);
        grupoBotones.SetActive(false);
        
        enIntro = true;
        bloqueoInput = false; // Aseguramos que el candado esté abierto al inicio

        // Reactivar el mando por si acaso
        if (EventSystem.current != null) EventSystem.current.sendNavigationEvents = true;
    }

    void Update()
    {
        // Si ya elegimos una opción, no hacemos NADA más.
        if (bloqueoInput) return;

        if (enIntro && Input.anyKeyDown)
        {
            PasarAlMenu();
        }
    }

    void PasarAlMenu()
    {
        enIntro = false;

        textoStart.SetActive(false);

        if (animadorGuerreros != null)
        {
            animadorGuerreros.SetTrigger("Moverse");
        }

        grupoBotones.SetActive(true);
        
        if (sonidoStart != null) sonidoStart.Play();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(primerBoton);
    }
    
    // --- FUNCIÓN PARA BLOQUEAR TODO AL ELEGIR ---
    void BloquearMenu()
    {
        bloqueoInput = true; // Ponemos el candado

        // TRUCO PRO: Desactivamos la navegación del EventSystem.
        // Así, aunque machaques los botones del mando, el menú ni se entera.
        EventSystem.current.sendNavigationEvents = false; 
    }

    // --- TUS BOTONES ---
    
    public void JugarHistoria() 
    { 
        if (bloqueoInput) return; // Si ya hay candado, ignorar clic
        BloquearMenu();

        Debug.Log("Cargando Historia...");
        LevelLoader.Instance.CargarNivel("Nivel1_Historia"); // Pon tu nombre de escena real
    }

    public void JugarSupervivencia() 
    {
        if (bloqueoInput) return; // Si ya hay candado, ignorar clic
        BloquearMenu();

        Debug.Log("Cargando Supervivencia..."); 
        LevelLoader.Instance.CargarNivel("Demo"); 
    }

    public void Salir() 
    { 
        if (bloqueoInput) return;
        BloquearMenu();

        Debug.Log("Saliendo...");
        Application.Quit(); 
    }
}