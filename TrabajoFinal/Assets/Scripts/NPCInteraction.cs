using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("--- Configuración del NPC ---")]
    public string nombreNPC = "Nombre Aqui";
    
    [TextArea(3, 10)] // Caja de texto grande en el inspector
    public string[] frasesDialogo;
    
    public Sprite caraNPC; // Arrastra la foto aquí

    // Detectar entrada
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.EntrarEnRango(this);
        }
    }

    // Detectar salida
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.SalirDeRango();
        }
    }
}