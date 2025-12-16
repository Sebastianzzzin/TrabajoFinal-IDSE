using UnityEngine;

public class ReboteLimites : MonoBehaviour
{
    [Header("Físicas del Rebote")]
    public float fuerzaRebote = 30f; 
    
    [Header("Efecto Visual UI")]
    public float fuerzaSacudidaUI = 20f; 
    public float duracionSacudida = 0.3f;

    private float cooldownRebote = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (cooldownRebote > 0) cooldownRebote -= Time.deltaTime;
    }

    void OnTriggerExit(Collider other)
    {
        // AHORA USA TU TAG CORRECTO: "Map Limit"
        if (other.CompareTag("Map Limit") && cooldownRebote <= 0)
        {
            AplicarRebote(other.transform.position);
        }
    }

    void AplicarRebote(Vector3 centroCupula)
    {
        cooldownRebote = 0.5f; 

        Vector3 direccionHaciaAdentro = (centroCupula - transform.position).normalized;
        
        // Matamos la velocidad (Compatible con Unity nuevo y viejo)
        #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
        #else
            rb.velocity = Vector3.zero; 
        #endif

        rb.AddForce(direccionHaciaAdentro * fuerzaRebote, ForceMode.VelocityChange);

        // Llamamos al Singleton (él solo se encarga de aparecerse)
        if (EfectoGolpeUI.Instance != null)
        {
            EfectoGolpeUI.Instance.Sacudir(duracionSacudida, fuerzaSacudidaUI);
        }
        
        Debug.Log("¡Rebote en Map Limit!");
    }
}