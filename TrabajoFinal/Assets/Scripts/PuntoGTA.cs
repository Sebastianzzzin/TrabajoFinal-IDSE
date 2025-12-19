using UnityEngine;

public class PuntoGTA : MonoBehaviour
{
    // Velocidad de giro para que se vea "vivo"
    public float velocidadGiro = 50f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, velocidadGiro * Time.deltaTime, 0);
    }

    // Detectamos si alguien entra en el cilindro
    void OnTriggerEnter(Collider other)
    {
        // Verificamos si es Goku
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Misión Terminada! / Encontraste la esfera del dragon.");

            // Aquí ira el evento
            

            // Opcional: Apagar el marcador para que no se active dos veces
            gameObject.SetActive(false);
        }
    }
}
