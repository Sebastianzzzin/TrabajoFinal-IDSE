using UnityEngine;
using UnityEngine.UI;
using TMPro;                
using UnityEngine.InputSystem; 

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; 

    [Header("--- CONTENEDORES (Arrastra aquí tus objetos) ---")]
    public GameObject messageContainer; // El objeto padre que tiene TODO el chat
    public GameObject interactionPrompt; // La imagen del Triángulo en la esquina
    
    [Header("--- ELEMENTOS INTERNOS DEL CHAT ---")]
    public TextMeshProUGUI nameText;    // El texto del nombre
    public TextMeshProUGUI chatText;    // El texto del dialogo
    public Image faceImage;             // La imagen de la cara

    // Variables de control
    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;
    
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
        // ================================================================
        // AQUÍ ESTÁ LA MAGIA: Al darle Play, el script fuerza a esconder todo.
        // No importa si lo dejaste prendido en el editor, esto lo apaga.
        // ================================================================
        messageContainer.SetActive(false);
        interactionPrompt.SetActive(false);
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Detectar botón Triángulo (Button North)
        // Solo funciona si estamos cerca del NPC
        if (isPlayerInRange && gamepad.buttonNorth.wasPressedThisFrame)
        {
            if (!isDialogueActive)
            {
                // Si no estamos hablando -> Empezar a hablar
                AbrirDialogo();
            }
            else
            {
                // Si ya estamos hablando -> Siguiente frase
                SiguienteFrase();
            }
        }
    }

    // --- FUNCIONES QUE LLAMAN LOS NPCS ---

    public void EntrarEnRango(NPCInteraction npc)
    {
        currentNPC = npc;
        isPlayerInRange = true;
        
        // Si no estamos en medio de una charla, mostramos el BOTÓN
        if (!isDialogueActive)
        {
            interactionPrompt.SetActive(true); // <--- MUESTRA TRIÁNGULO
            messageContainer.SetActive(false); // Asegura que el chat siga oculto
        }
    }

    public void SalirDeRango()
    {
        isPlayerInRange = false;
        currentNPC = null;
        
        // Si nos alejamos, se apaga TODO
        interactionPrompt.SetActive(false); // <--- OCULTA TRIÁNGULO
        CerrarDialogo();
    }

    // --- LÓGICA INTERNA VISUAL ---

    void AbrirDialogo()
    {
        isDialogueActive = true;
        
        
        // INTERCAMBIO VISUAL:
        interactionPrompt.SetActive(false); // 1. Ocultamos el botón Triángulo
        messageContainer.SetActive(true);   // 2. Mostramos la ventana de Chat

        // Cargar datos
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
            // Se acabaron las frases
            CerrarDialogo();
        }
    }

    void CerrarDialogo()
    {
        isDialogueActive = false;
        
        // Ocultamos el chat
        messageContainer.SetActive(false); 

        // Lógica inteligente:
        // Si cerramos el chat pero SEGUIMOS cerca del NPC, 
        // volvemos a mostrar el botón por si quiere hablar otra vez.
        if (isPlayerInRange)
        {
            interactionPrompt.SetActive(true);
        }
    }
}