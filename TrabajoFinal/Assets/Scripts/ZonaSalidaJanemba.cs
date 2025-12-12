using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class ZonaSalidaJanemba : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Escribe aquí el nombre EXACTO de la escena a la que quieres ir")]
    public string nombreEscenaDestino = "MenuPrincipal"; 

    // Opcional: Si quieres que suene algo al entrar (tipo un teleport)
    public AudioClip sonidoSalida; 
    
    private bool yaSeActivo = false;

    void OnTriggerEnter(Collider other)
    {
        // Verificamos que sea el Jugador y que no se haya activado ya
        if (other.CompareTag("Player") && !yaSeActivo)
        {
            yaSeActivo = true; // Candado para no activarlo 2 veces
            Debug.Log("¡Saliendo del nivel!");

            // Si pusiste sonido, lo reproducimos en la posición antes de irnos
            if (sonidoSalida != null)
            {
                // PlayClipAtPoint crea un audio temporal que no se destruye al cambiar de escena inmediatamente
                AudioSource.PlayClipAtPoint(sonidoSalida, transform.position);
            }

            // Cambiamos de escena
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}