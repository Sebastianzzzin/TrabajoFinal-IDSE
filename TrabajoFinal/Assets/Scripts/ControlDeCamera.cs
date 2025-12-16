using UnityEngine;

public class ControlDeCamara : MonoBehaviour
{
    [Header("Objeto a seguir")]
    public Transform jugador;

    [Header("Offset respecto al jugador")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Suavizado del movimiento")]
    public float suavizado = 6f;

    void LateUpdate()
    {
        if (jugador == null)
            return;

        // Solo sigue X y Y. Z siempre se queda fija como el offset.
        Vector3 posicionObjetivo = new Vector3(
            jugador.position.x + offset.x,
            jugador.position.y + offset.y,
            offset.z // Z fijo
        );

        transform.position = Vector3.Lerp(
            transform.position,
            posicionObjetivo,
            suavizado * Time.deltaTime
        );
    }
}


