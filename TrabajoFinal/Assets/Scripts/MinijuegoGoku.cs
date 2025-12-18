using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinijuegoGoku : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image imagenGoku;        
    public Transform contenedorPlatos; 
    public TextMeshProUGUI textoContador; 

    [Header("Referencias Prefab")]
    public GameObject prefabPlato;  

    [Header("Sprites")]
    public Sprite gokuEsperando;    
    public Sprite gokuComiendo;     
    public Sprite platoBase;        
    public Sprite platoPila;        

    [Header("Configuración Visual")]
    public float alturaPorPlato = 15f; 
    
    [Header("Configuración de Ritmo (Suavizado)")]
    [Tooltip("Tiempo mínimo entre cada plato. Evita que el spam se vea feo.")]
    public float cooldownEntreBocados = 0.15f; // Recomendado: 0.15 a 0.2
    [Tooltip("Cuánto tiempo se queda la boca abierta/masticando")]
    public float duracionMasticar = 0.2f;      // Debe ser igual o un poco mayor que el cooldown

    // Variables privadas
    private int cantidadPlatos = 0;
    private float tiempoParaCerrarBoca = 0f;
    private float tiempoSiguienteBocado = 0f; // Control del Spam

    void OnEnable()
    {
        // Resetear todo
        cantidadPlatos = 0;
        tiempoSiguienteBocado = 0f;
        ActualizarTexto();
        imagenGoku.sprite = gokuEsperando;

        foreach (Transform hijo in contenedorPlatos)
        {
            Destroy(hijo.gameObject);
        }
    }

    void Update()
    { 
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) 
        {
            return; 
        }
        // 1. LÓGICA DE SPRITE (Cerrar boca)
        // Si el tiempo de masticar se acaba, vuelve a Idle
        if (tiempoParaCerrarBoca > 0)
        {
            tiempoParaCerrarBoca -= Time.unscaledDeltaTime; 
            if (tiempoParaCerrarBoca <= 0)
            {
                imagenGoku.sprite = gokuEsperando;
            }
        }

        // 2. DETECTAR INPUT CON LIMITADOR (Cooldown)
        // Solo comemos si ha pasado el tiempo de enfriamiento
        if (Input.anyKeyDown && Time.unscaledTime >= tiempoSiguienteBocado)
        {
            Comer();
        }
    }

    void Comer()
    {
        // Actualizamos el tiempo para el siguiente bocado
        tiempoSiguienteBocado = Time.unscaledTime + cooldownEntreBocados;

        // 1. Animación Goku
        imagenGoku.sprite = gokuComiendo;
        // Reiniciamos el tiempo para cerrar la boca
        tiempoParaCerrarBoca = duracionMasticar; 

        // 2. Crear el plato
        GameObject nuevoPlato = Instantiate(prefabPlato, contenedorPlatos);
        Image imgComponente = nuevoPlato.GetComponent<Image>();

        // 3. Lógica de Sprites y Posición
        if (cantidadPlatos == 0)
        {
            imgComponente.sprite = platoBase;
            nuevoPlato.transform.localPosition = Vector3.zero; 
        }
        else
        {
            imgComponente.sprite = platoPila;

            float randomX = Random.Range(-5f, 5f);
            float altura = cantidadPlatos * alturaPorPlato;
            
            nuevoPlato.transform.localPosition = new Vector3(randomX, altura, 0);
        }

        // 4. Sumar contador
        cantidadPlatos++;
        ActualizarTexto();
    }

    void ActualizarTexto()
    {
        if (textoContador != null) textoContador.text = "Platos: " + cantidadPlatos;
    }
}