using UnityEngine;

public class MovimientoPilar : MonoBehaviour
{
    public float velocidadAvance = 20f; // Qué tan rápido va hacia Goku
    public float velocidadGiro = 400f;  // Qué tan rápido da vueltas

    void Update()
    {
        // Mover hacia la izquierda
        transform.Translate(Vector3.left * velocidadAvance * Time.deltaTime, Space.World);

        // CORRECCIÓN DEL GIRO:
        // Al estar el objeto tumbado horizontalmente, el eje que lo atraviesa es el X (Rojo).
        // Giramos en X para hacer el efecto taladro.
        transform.Rotate(velocidadGiro * Time.deltaTime, 0, 0);
    }
}
