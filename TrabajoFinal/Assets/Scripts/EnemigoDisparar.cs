using UnityEngine;

public class EnemigoDisparar : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject prefabProyectil; // Tu rayo/lava
    public Transform puntoDeSalida;    // La punta del dedo ("PuntaDedo")
    public Transform objetivo;         // ARRASTRA A GOKU AQUÍ (Nuevo)

    [Header("Configuración")]
    public float tiempoEntreDisparos = 1.5f;
    private float cronometro;

    void Update()
    {
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreDisparos)
        {
            Disparar();
            cronometro = 0;
        }
    }

    void Disparar()
    {
        if (prefabProyectil != null && puntoDeSalida != null && objetivo != null)
        {
            // 1. Creamos el proyectil en la punta del dedo
            GameObject nuevoDisparo = Instantiate(prefabProyectil, puntoDeSalida.position, Quaternion.identity);

            // 2. MAGIA MATEMÁTICA: Hacemos que SOLO el disparo mire a Goku
            // Esto calcula la diagonal exacta desde el dedo (arriba) hasta Goku (abajo)
            nuevoDisparo.transform.LookAt(objetivo.position + Vector3.up);
            // (El "+ Vector3.up" es para apuntarle al pecho y no a los pies)
        }
    }
}
