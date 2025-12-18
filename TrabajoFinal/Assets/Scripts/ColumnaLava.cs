using UnityEngine;
using System.Collections;

public class ColumnaLava : MonoBehaviour
{
    [Header("Referencias (Hijos)")]
    public Transform visualLava;   
    public Transform avisoVisual;  

    [Header("Configuración Altura")]
    public float alturaFinal = 5f; 
    
    [Header("Velocidad de Animación")]
    public float velocidadCrecimiento = 20f; 
    public float tiempoEspera = 0.5f; 
    
    [Header("Daño")]
    public int daño = 10;
    public bool esPermanente = false;

    [Header("Luces")]
    public Light luzLava;
    public float intensidadLuzMaxima = 4f;

    private Transform jugadorTarget;
    private Vector3 escalaBaseX_Z; 
    
    // Estados
    private bool estaDescendiendo = false;
    private bool lavaHaCrecido = false;
    private bool haciendoDano = false; 

    public void Inicializar(Transform jugador)
    {
        jugadorTarget = jugador;
    }

    void Start()
    {
        if (visualLava == null || avisoVisual == null) return;

        escalaBaseX_Z = visualLava.localScale;

        // Resetear alturas a 0
        SetAlturaCilindro(visualLava, 0);
        SetAlturaCilindro(avisoVisual, 0);
        
        visualLava.gameObject.SetActive(true);
        avisoVisual.gameObject.SetActive(true);

        if (luzLava != null) luzLava.intensity = 0;

        StartCoroutine(CoreografiaAparicion());
    }

    IEnumerator CoreografiaAparicion()
    {
        // =========================================================
        // FASE 1: CRECER EL AVISO (SOMBRA)
        // =========================================================
        float alturaActual = 0f;
        while (alturaActual < alturaFinal)
        {
            alturaActual += velocidadCrecimiento * Time.deltaTime;
            if (alturaActual > alturaFinal) alturaActual = alturaFinal;

            SetAlturaCilindro(avisoVisual, alturaActual);
            yield return null; 
        }

        // =========================================================
        // FASE 2: ESPERA
        // =========================================================
        yield return new WaitForSeconds(tiempoEspera);

        // =========================================================
        // FASE 3: CRECER LA LAVA (DENTRO DE LA SOMBRA)
        // =========================================================
        
        // --- CAMBIO: YA NO APAGAMOS EL AVISO AQUÍ ---
        // avisoVisual.gameObject.SetActive(false); // <--- ESTO LO QUITAMOS
        
        haciendoDano = true; // La lava empieza a salir, ya hace daño

        alturaActual = 0f;
        while (alturaActual < alturaFinal)
        {
            alturaActual += velocidadCrecimiento * Time.deltaTime;
            if (alturaActual > alturaFinal) alturaActual = alturaFinal;
            
            SetAlturaCilindro(visualLava, alturaActual);

            // Luces
            if (luzLava != null)
            {
                float porcentaje = alturaActual / alturaFinal;
                luzLava.intensity = Mathf.Lerp(0, intensidadLuzMaxima, porcentaje);
            }

            yield return null;
        }

        // --- OPCIONAL: APAGAR AVISO AL FINAL ---
        // Una vez la lava llegó arriba, apagamos el contenedor.
        // Si prefieres que se quede como un "borde" brillante, borra esta línea también.
        avisoVisual.gameObject.SetActive(false); 
        
        lavaHaCrecido = true;
    }

    // TRUCO PARA EL PIVOTE DEL CILINDRO
    void SetAlturaCilindro(Transform t, float alturaDeseada)
    {
        float nuevaEscalaY = alturaDeseada / 2f;
        float nuevaPosY = alturaDeseada / 2f;

        t.localScale = new Vector3(escalaBaseX_Z.x, nuevaEscalaY, escalaBaseX_Z.z);
        t.localPosition = new Vector3(0, nuevaPosY, 0);
    }

    void Update()
    {
        if (esPermanente) return;
        if (!lavaHaCrecido) return; 

        // LÓGICA DE BAJAR (DESAPARECER)
        if (estaDescendiendo)
        {
            float alturaActual = visualLava.localScale.y * 2f; 
            float nuevaAltura = Mathf.MoveTowards(alturaActual, 0, velocidadCrecimiento * Time.deltaTime);
            
            SetAlturaCilindro(visualLava, nuevaAltura);

            if (luzLava != null)
            {
                if(alturaFinal > 0) 
                    luzLava.intensity = Mathf.Lerp(0, intensidadLuzMaxima, nuevaAltura / alturaFinal);
            }

            if (nuevaAltura <= 0.01f) Destroy(gameObject);
            return;
        }

        if (jugadorTarget != null)
        {
            Vector3 dir = transform.position - jugadorTarget.position;
            if (Vector3.Dot(jugadorTarget.forward, dir) < -5.0f)
            {
                estaDescendiendo = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (haciendoDano && !estaDescendiendo && other.CompareTag("Player"))
        {
            other.SendMessage("RecibirDano", daño, SendMessageOptions.DontRequireReceiver);
        }
    }
}