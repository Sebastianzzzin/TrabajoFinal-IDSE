using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("--- CONTENEDORES ---")]
    public GameObject messageContainer;
    public GameObject interactionPrompt;

    [Header("--- ELEMENTOS INTERNOS ---")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI chatText;
    public Image faceImage;

    // Variables de control (Ahora públicas para que el Player pueda leerlas)
    public bool isPlayerInRange = false; 
    public bool isDialogueActive = false;

    // Datos temporales
    private string[] currentLines;
    private int currentLineIndex = 0;
    private NPCInteraction currentNPC;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        messageContainer.SetActive(false);
        interactionPrompt.SetActive(false);
    }

    // --- ELIMINAMOS EL UPDATE COMPLETO ---
    // Ya no detectamos input aquí.
    
    // --- NUEVA FUNCIÓN PÚBLICA QUE LLAMARÁ EL PLAYER ---
    public void IntentarInteraccion()
    {
        // Solo hacemos algo si el jugador está en rango
        if (isPlayerInRange)
        {
            if (!isDialogueActive)
            {
                AbrirDialogo();
            }
            else
            {
                SiguienteFrase();
            }
        }
    }

    // --- FUNCIONES QUE LLAMAN LOS NPCS ---

    public void EntrarEnRango(NPCInteraction npc)
    {
        currentNPC = npc;
        isPlayerInRange = true;

        if (!isDialogueActive)
        {
            interactionPrompt.SetActive(true);
            messageContainer.SetActive(false);
        }
    }

    public void SalirDeRango()
    {
        isPlayerInRange = false;
        currentNPC = null;

        interactionPrompt.SetActive(false);
        CerrarDialogo();
    }

    // --- LÓGICA INTERNA VISUAL ---

    void AbrirDialogo()
    {
        isDialogueActive = true;
        interactionPrompt.SetActive(false);
        messageContainer.SetActive(true);

        nameText.text = currentNPC.nombreNPC;
        faceImage.sprite = currentNPC.caraNPC;
        currentLines = currentNPC.frasesDialogo;
        currentLineIndex = 0;

        SiguienteFrase();
    }

    void SiguienteFrase()
    {
        if (currentLineIndex < currentLines.Length)
        {
            chatText.text = currentLines[currentLineIndex];
            currentLineIndex++;
        }
        else
        {
            CerrarDialogo();
        }
    }

    void CerrarDialogo()
    {
        isDialogueActive = false;
        messageContainer.SetActive(false);

        if (isPlayerInRange)
        {
            interactionPrompt.SetActive(true);
        }
    }
}