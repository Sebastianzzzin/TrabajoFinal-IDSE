using UnityEngine;

public class LimiteVertical : MonoBehaviour
{
    [Header("Límites en X (izquierda / derecha)")]
    public float limiteIzquierda = -21f;  // un poco menos que el inicio
    public float limiteDerecha = 21f;     // un poco más que el templo

    [Header("Límites en Y (arriba / abajo)")]
    public float limiteSuperiorY = 6f;    // más arriba del templo
    public float limiteInferiorY = -3f;   // un poco más abajo de goku

    [Header("Límites en Z (profundidad)")]
    public float limiteSuperiorZ = 1.5f;
    public float limiteInferiorZ = -1.5f;

    void Update()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, limiteIzquierda, limiteDerecha);
        pos.y = Mathf.Clamp(pos.y, limiteInferiorY, limiteSuperiorY);
        pos.z = Mathf.Clamp(pos.z, limiteInferiorZ, limiteSuperiorZ);

        transform.position = pos;
    }
}







