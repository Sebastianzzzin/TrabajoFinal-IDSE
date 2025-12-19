using UnityEngine;

public class GeneradorArena : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabColumna; // Tu obstáculo
    public int cantidad = 50;
    public float radioMapa = 140f;
    public float zonaSeguraCentro = 20f; // Espacio libre para la nave y el jugador

    void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector2 punto = Random.insideUnitCircle * radioMapa;

            // Si cae en el centro, lo empujamos hacia afuera
            if (punto.magnitude < zonaSeguraCentro)
            {
                punto = punto.normalized * (zonaSeguraCentro + 5f);
            }

            Vector3 posFinal = transform.position + new Vector3(punto.x, 0, punto.y);

            // Creamos la columna y la hacemos hija de este objeto para ordenar
            GameObject col = Instantiate(prefabColumna, posFinal, Quaternion.identity);
            col.transform.SetParent(this.transform);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioMapa);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zonaSeguraCentro);
    }
}
