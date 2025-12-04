using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 10f;
    public float velocidadSubida = 8f;
    public float rotacionVelocidad = 80f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Movimiento adelante y atrás
        float moverVertical = Input.GetAxis("Vertical");
        float rotar = Input.GetAxis("Horizontal");

        // Mantener la velocidad Y actual
        Vector3 velocidadActual = rb.linearVelocity;

        // Movimiento horizontal (adelante / atrás)
        Vector3 movimiento = transform.forward * moverVertical * velocidad;

        // Aplicar movimiento horizontal y conservar Y
        rb.linearVelocity = new Vector3(movimiento.x, velocidadActual.y, movimiento.z);

        // Rotación
        transform.Rotate(Vector3.up * rotar * rotacionVelocidad * Time.fixedDeltaTime);

        // SUBIR (solo mientras se presiona)
        if (Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, velocidadSubida, rb.linearVelocity.z);
        }
        // BAJAR (solo mientras se presiona)
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -velocidadSubida, rb.linearVelocity.z);
        }
        else
        {
            // Si no presiona nada, se queda flotando
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
    }
}
