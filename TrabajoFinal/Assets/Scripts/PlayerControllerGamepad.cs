using UnityEngine;

public class PlayerControllerGamepad : MonoBehaviour
{
    public float velocidad = 8f;
    public float velocidadSubida = 6f;

    [Header("Rotación")]
    public float velocidadGiro = 10f;     // Suavidad del giro
    public float anguloParaAvanzar = 4f;  // Más tolerancia = sin vibración

    Rigidbody rb;

    Vector2 input;
    float rotacionYObjetivo;
    bool tieneObjetivo = false;
    bool rotacionCompleta = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        input = new Vector2(x, z);

        // SUBIR / BAJAR
        if (Input.GetButton("Jump"))
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, velocidadSubida, rb.linearVelocity.z);
        else if (Input.GetKey(KeyCode.LeftControl))
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -velocidadSubida, rb.linearVelocity.z);
        else
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // ROTACIÓN OBJETIVO
        if (input.magnitude > 0.2f)
        {
            tieneObjetivo = true;
            rotacionCompleta = false;

            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                if (input.y > 0)
                    rotacionYObjetivo = -180f;
                else
                    rotacionYObjetivo = 0f;
            }
            else
            {
                if (input.x > 0)
                    rotacionYObjetivo = -90f;
                else
                    rotacionYObjetivo = 90f;
            }
        }
        else
        {
            tieneObjetivo = false;
        }
    }

    void FixedUpdate()
    {
        if (!tieneObjetivo)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        Quaternion rotacionObjetivoQuat = Quaternion.Euler(0f, rotacionYObjetivo, 0f);

        // ROTACIÓN SUAVE SIN VIBRACIÓN
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacionObjetivoQuat,
            velocidadGiro * Time.fixedDeltaTime
        );

        float anguloRestante = Quaternion.Angle(transform.rotation, rotacionObjetivoQuat);

        if (anguloRestante <= anguloParaAvanzar)
        {
            rotacionCompleta = true;
        }

        if (rotacionCompleta)
        {
            // MOVIMIENTO POR MAPA
            Vector3 movimiento = new Vector3(
                input.x * velocidad,
                rb.linearVelocity.y,
                input.y * velocidad
            );

            rb.linearVelocity = movimiento;
        }
        else
        {
            // Mientras gira → congelado XZ
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
}













