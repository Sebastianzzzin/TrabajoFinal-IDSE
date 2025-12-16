using UnityEngine;

public class ParallaxFondo : MonoBehaviour
{
    public Transform camara;        // Cámara a seguir
    public float fuerzaParallax = 0.2f; // Entre 0.05 y 0.3 para efecto suave

    private Vector3 posicionAnteriorCamara;

    void Start()
    {
        if (camara == null)
            camara = Camera.main.transform;

        posicionAnteriorCamara = camara.position;
    }

    void LateUpdate()
    {
        Vector3 movimientoCamara = camara.position - posicionAnteriorCamara;

        // El fondo se mueve pero más lento
        transform.position += new Vector3(
            movimientoCamara.x * fuerzaParallax,
            movimientoCamara.y * fuerzaParallax,
            0 // No modifica Z
        );

        posicionAnteriorCamara = camara.position;
    }
}
