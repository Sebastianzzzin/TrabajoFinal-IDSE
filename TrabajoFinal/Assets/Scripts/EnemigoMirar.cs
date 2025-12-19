using UnityEngine;

public class EnemigoMirar : MonoBehaviour
{
    public Transform jugador;
    public float velocidadGiro = 3f; // Ajusta esto para que sea más lento o rápido

    void Update()
    {
        if (jugador == null) return;

        // Calculamos dirección hacia el jugador
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0; // Evita que la nave se incline hacia arriba/abajo (opcional)

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            // Slerp hace el giro suave
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);
        }
    }
}
