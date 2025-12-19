using System.Collections;
using UnityEngine;

public class LimiteVertical : MonoBehaviour
{
    [Header("Configuración Inicial")]
    public bool iniciarEnElLimiteIzquierdo = true;

    [Header("Límites en X (Paredes)")]
    public float limiteIzquierda = -21f; // AJUSTA ESTE NUMERO EN EL INSPECTOR
    public float limiteDerecha = 21f;

    [Header("Límites en Y (Techo/Suelo)")]
    public float limiteSuperiorY = 6f;
    public float limiteInferiorY = -3f;

    [Header("Límites en Z (Profundidad)")]
    public float limiteSuperiorZ = 1.5f;
    public float limiteInferiorZ = -1.5f;

    IEnumerator Start()
    {
        // Esperamos un instante para asegurarnos que la cámara ya se colocó en su sitio
        yield return new WaitForEndOfFrame();

        if (iniciarEnElLimiteIzquierdo)
        {
            Vector3 posInicial = transform.position;

            // Forzamos la posición X para que sea EXACTAMENTE el límite izquierdo
            posInicial.x = limiteIzquierda;

            transform.position = posInicial;
        }
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        // Mantener dentro de los límites
        pos.x = Mathf.Clamp(pos.x, limiteIzquierda, limiteDerecha);
        pos.y = Mathf.Clamp(pos.y, limiteInferiorY, limiteSuperiorY);
        pos.z = Mathf.Clamp(pos.z, limiteInferiorZ, limiteSuperiorZ);

        transform.position = pos;
    }
}