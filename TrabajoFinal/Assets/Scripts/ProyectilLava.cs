using UnityEngine;

public class ProyectilLava : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 25f;      // Velocidad del disparo
    public float tiempoDeVida = 5f;    // Distancia (Tiempo antes de desaparecer)

    [Header("Daño")]
    public int daño = 20;              // Cuánto baja de la barra de vida

    void Start()
    {
        // El proyectil se autodestruye después de X segundos para no llenar la memoria
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Mueve el proyectil hacia adelante constantemente
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificamos si chocamos con Goku (que debe tener el tag "Player")
        if (other.CompareTag("Player"))
        {
            // Buscamos TU script específico "PlayerStats"
            PlayerStats statsGoku = other.GetComponent<PlayerStats>();

            if (statsGoku != null)
            {
                // Llamamos a TU función exacta "RecibirDano"
                statsGoku.RecibirDano(daño);

                // (Opcional) Mensaje en consola para confirmar
                Debug.Log("¡Freezer acertó el disparo!");
            }

            // El disparo desaparece al impactar
            Destroy(gameObject);
        }
    }
}