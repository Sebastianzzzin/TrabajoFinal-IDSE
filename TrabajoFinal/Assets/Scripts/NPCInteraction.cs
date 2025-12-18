using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("--- Configuración del NPC ---")]
    public string nombreNPC = "Nombre Aqui";

    [TextArea(3, 10)]
    public string[] frasesDialogo;

    public Sprite caraNPC;

    [Header("--- Recompensa ---")]
    public bool daEsferaDelDragon = false;
    public int numeroEsfera = 1; // de 1 a 7

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Iniciar logica de dialogo
            DialogueManager.Instance.EntrarEnRango(this);

            // 2. NUEVO: Detener a Tao Pai Pai
            // Buscamos el objeto por su nombre exacto en la Jerarquía
            GameObject generador = GameObject.Find("GeneradorDeAtaques");

            if (generador != null)
            {
                generador.SetActive(false); // Esto apaga el generador
                Debug.Log("¡Zona Segura! Tao Pai Pai desactivado.");
            }

            // Opcional: Si quieres destruir los pilares que ya están en pantalla
            // podrías buscar los objetos con tag "Enemigo" y destruirlos aquí también.
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.SalirDeRango();
        }
    }

    public void DarRecompensa(GameObject player)
    {
        Debug.Log(">>> Entró a DarRecompensa()");

        if (daEsferaDelDragon)
        {
            Debug.Log(">>> daEsferaDelDragon = TRUE");
            Debug.Log(">>> numeroEsfera = " + numeroEsfera);

            // --- FIX DEFINITIVO ---
            PlayerStats stats = player.GetComponent<PlayerStats>();

            if (stats == null)
                stats = player.GetComponentInParent<PlayerStats>();
            if (stats == null)
                stats = player.GetComponentInChildren<PlayerStats>();

            Debug.Log(">>> PlayerStats encontrado: " + (stats != null));

            if (stats != null)
            {
                stats.AgregarEsferaDragon(numeroEsfera);
                Debug.Log("Piccolo le dio a Goku la esfera número " + numeroEsfera);
            }
            else
            {
                Debug.LogError("ERROR: No se encontró PlayerStats en ningún objeto del jugador.");
            }
        }
        else
        {
            Debug.Log(">>> daEsferaDelDragon = FALSE");
        }
    }
}