using UnityEngine;

public class ColumnaLava : MonoBehaviour
{
    [Header("Configuración")]
    public float alturaFinal = 5f;
    public float velocidad = 2f;
    public int daño = 10;
    
    // --- NUEVA VARIABLE ---
    public bool esPermanente = false; // Si marcas esto, nunca desaparece
    // ----------------------

    [Header("Luces y Efectos")]
    public Light luzLava;
    public float intensidadLuzMaxima = 4f;

    private Transform jugadorTarget; 
    private Vector3 escalaInicial;
    private bool estaDescendiendo = false;
    private bool haCrecidoPorCompleto = false;

    public void Inicializar(Transform jugador)
    {
        jugadorTarget = jugador;
    }

    void Start()
    {
        escalaInicial = transform.localScale;
        transform.localScale = new Vector3(escalaInicial.x, 0, escalaInicial.z);
        if(luzLava != null) luzLava.intensity = 0;
    }

    void Update()
    {
        // 1. CRECIMIENTO (Aplica para todas)
        if (transform.localScale.y < alturaFinal && !haCrecidoPorCompleto && !estaDescendiendo)
        {
            float nuevoY = Mathf.MoveTowards(transform.localScale.y, alturaFinal, velocidad * Time.deltaTime);
            transform.localScale = new Vector3(escalaInicial.x, nuevoY, escalaInicial.z);

            if (luzLava != null)
            {
                float porcentaje = transform.localScale.y / alturaFinal;
                luzLava.intensity = Mathf.Lerp(0, intensidadLuzMaxima, porcentaje);
            }

            if (Mathf.Abs(transform.localScale.y - alturaFinal) < 0.01f) haCrecidoPorCompleto = true;
        }

        // --- SI ES PERMANENTE, AQUÍ TERMINA SU LÓGICA ---
        if (esPermanente) return; 
        // ------------------------------------------------

        // 2. LÓGICA DE DESAPARECER (Solo para las que persiguen al jugador)
        
        // Si ya empezó a bajar...
        if (estaDescendiendo)
        {
            float nuevoY = Mathf.MoveTowards(transform.localScale.y, 0, velocidad * Time.deltaTime);
            transform.localScale = new Vector3(escalaInicial.x, nuevoY, escalaInicial.z);

            if (luzLava != null)
            {
                float porcentaje = transform.localScale.y / alturaFinal;
                luzLava.intensity = Mathf.Lerp(0, intensidadLuzMaxima, porcentaje);
            }

            if (transform.localScale.y <= 0.01f) Destroy(gameObject);
            return;
        }

        // Chequeo si quedó atrás
        if (jugadorTarget != null)
        {
            Vector3 direccionHaciaColumna = transform.position - jugadorTarget.position;
            float productoPunto = Vector3.Dot(jugadorTarget.forward, direccionHaciaColumna);

            if (productoPunto < -5.0f) // Le di un poco más de margen (-5)
            {
                estaDescendiendo = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hacen daño siempre, sean permanentes o no
        if (!estaDescendiendo && other.CompareTag("Player"))
        {
            other.SendMessage("RecibirDano", daño, SendMessageOptions.DontRequireReceiver);
        }
    }
}