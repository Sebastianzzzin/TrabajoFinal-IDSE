using UnityEngine;

public class ProyectilLava : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 25f;      // Velocidad del disparo
    public float tiempoDeVida = 5f;    // Distancia (Tiempo antes de desaparecer)

    [Header("Da�o")]
    public int dao = 20;              // Cunto baja de la barra de vida

    void Start()
    {
        // El proyectil se autodestruye despu�s de X segundos para no llenar la memoria
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
            // Buscamos TU script espec�fico "PlayerStats"
            PlayerStats statsGoku = other.GetComponent<PlayerStats>();

            if (statsGoku != null)
            {
                // Llamamos a TU funci�n exacta "RecibirDano"
                statsGoku.RecibirDano(dao);

                // (Opcional) Mensaje en consola para confirmar
                Debug.Log("�Freezer acert� el disparo!");
            }

            // El disparo desaparece al impactar
            Destroy(gameObject);
        }
    }
}