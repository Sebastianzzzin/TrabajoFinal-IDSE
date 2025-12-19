using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private System.Action alTerminarDialogo;
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

    void Update()
    {
        // Solo escuchamos si hay un diálogo abierto
        if (isDialogueActive)
        {
            // OPCIÓN 1: Teclado (Espacio o Enter)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                SiguienteFrase();
            }
            // OPCIÓN 2: Mouse (Clic izquierdo)
            else if (Input.GetMouseButtonDown(0))
            {
                SiguienteFrase();
            }
            // OPCIÓN 3: Gamepad (Botón Sur = A en Xbox / X en PlayStation)
            else if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                SiguienteFrase();
            }
        }
    }
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

    // --- FUNCIONES QUE LLAMAN LOS NPCs ---
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

        // Otorgar recompensa si el NPC la tiene
        if (currentNPC != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("currentNPC es: " + (currentNPC != null));
            Debug.Log("isPlayerInRange = " + isPlayerInRange);
            currentNPC.DarRecompensa(player);
        }

        if (isPlayerInRange)
        {
            interactionPrompt.SetActive(true);
        }

        Debug.Log(">>> Se ejecutó CerrarDialogo()");
        if (alTerminarDialogo != null)
        {
            alTerminarDialogo.Invoke(); // Ejecutar la acción pendiente
            alTerminarDialogo = null;   // Limpiar para la próxima
        }
    }
    public void IniciarDialogoNarrativo(string nombre, Sprite cara, string[] frases, System.Action accionAlTerminar)
    {
        // 1. Guardamos qué hacer cuando termine
        alTerminarDialogo = accionAlTerminar;

        // 2. Configuramos la UI
        isDialogueActive = true;
        interactionPrompt.SetActive(false); // Ocultamos la tecla "F" o botón
        messageContainer.SetActive(true);

        // 3. Llenamos datos visuales
        nameText.text = nombre;
        if (faceImage != null) faceImage.sprite = cara;

        currentLines = frases;
        currentLineIndex = 0;

        // 4. Arrancamos
        SiguienteFrase();
    }
}