using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class ZonaSalidaJanemba : MonoBehaviour
{
     public JanembaDirector director; // Referencia al director para avisarle
    private bool yaGanamos = false;
    [Header("Configuración")]
    [Tooltip("Escribe aquí el nombre EXACTO de la escena a la que quieres ir")]
    public string nombreEscenaDestino = "MenuPrincipal"; 

    // Opcional: Si quieres que suene algo al entrar (tipo un teleport)
    public AudioClip sonidoSalida; 
    
    private bool yaSeActivo = false;

    void OnTriggerEnter(Collider other)
    {
       if (yaGanamos) return;

        if (other.CompareTag("Player"))
        {
            yaGanamos = true;
            Debug.Log("¡Goku llegó a la Cúpula!");
            
            // Avisamos al director que inicie la cinemática final
            if (director != null)
            {
                director.LlegadaALaMeta();
            }
        }
    }
}