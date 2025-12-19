using UnityEngine;

public class EsporaPulsante : MonoBehaviour
{
    [Header("Tamaños")]
    [Tooltip("El tamaño más pequeño al que llegará la nube")]
    public Vector3 escalaMinima = new Vector3(4f, 6f, 6f);

    [Tooltip("El tamaño más grande al que llegará la nube")]
    public Vector3 escalaMaxima = new Vector3(6f, 8f, 8f);

    [Header("Velocidad")]
    [Tooltip("Qué tan rápido se infla y desinfla")]
    public float velocidadPalpito = 1.5f;

    // Un número aleatorio para que si pones muchas nubes, no se muevan todas idénticas
    private float offsetTiempo;

    void Start()
    {
        // Generamos un desfase aleatorio al inicio
        offsetTiempo = Random.Range(0f, 10f);
    }

    void Update()
    {
        // Mathf.Sin nos da un valor que sube y baja suavemente como una ola.
        // Lo ajustamos para que vaya de 0 a 1.
        float factor = (Mathf.Sin(Time.time * velocidadPalpito + offsetTiempo) + 1.0f) / 2.0f;

        // "Lerp" significa que buscamos un punto intermedio entre el mínimo y el máximo
        // basado en el factor calculado arriba.
        transform.localScale = Vector3.Lerp(escalaMinima, escalaMaxima, factor);
    }
}