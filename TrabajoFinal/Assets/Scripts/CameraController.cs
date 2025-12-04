using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform jugador;        // Goku
    public Transform puntoNariz;     // Objeto en la nariz

    [Header("Modo Top Down FIFA")]
    public Vector3 offsetTopDown = new Vector3(0, 15f, -8f);
    public Vector3 rotacionTopDown = new Vector3(60f, 0f, 0f);

    [Header("Suavizado")]
    public float suavizado = 8f;

    private bool primeraPersona = false;

    void Start()
    {
        ActivarTopDown();
    }

    void Update()
    {
        // Cambiar cámara con F5 o botón del mando (Start)
        if (Input.GetKeyDown(KeyCode.F5) || Input.GetButtonDown("Submit"))
        {
            primeraPersona = !primeraPersona;

            if (primeraPersona)
                ActivarPrimeraPersona();
            else
                ActivarTopDown();
        }
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        if (!primeraPersona)
        {
            Vector3 posicionDeseada = new Vector3(
                jugador.position.x + offsetTopDown.x,
                offsetTopDown.y,
                jugador.position.z + offsetTopDown.z
            );

            transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        }
        else
        {
            // PRIMERA PERSONA desde la NARIZ
            transform.position = puntoNariz.position;
            transform.rotation = puntoNariz.rotation;
        }
    }

    void ActivarTopDown()
    {
        transform.rotation = Quaternion.Euler(rotacionTopDown);
    }

    void ActivarPrimeraPersona()
    {
        if (puntoNariz != null)
        {
            transform.position = puntoNariz.position;
            transform.rotation = puntoNariz.rotation;
        }
    }
}
