using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VueloTeclado3D : MonoBehaviour
{
    [Header("Ajustes de Vuelo")]
    public float velocidadVuelo = 25f;
    public float velocidadVerticalPura = 15f;
    public float suavizadoGiro = 10f;

    [Header("Sensibilidad de Cámara")]
    public float sensibilidadMouse = 1f;

    [Header("Referencias")]
    public Transform camaraPrincipal;
    // Referencia al script de la cámara para enviarle datos
    private CamaraGTA scriptCamara;

    private Rigidbody rb;
    private float inputHorizontal;
    private float inputVertical;
    private float inputSubirBajar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (camaraPrincipal == null) camaraPrincipal = Camera.main.transform;

        // Buscamos el script de la cámara para poder hablar con él
        if (camaraPrincipal != null)
        {
            scriptCamara = camaraPrincipal.GetComponent<CamaraGTA>();
        }

        // Bloquear el mouse para que no se salga de la ventana mientras giras la vista
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. LEER INPUT DE MOVIMIENTO (WASD)
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        inputSubirBajar = 0f;
        if (Input.GetKey(KeyCode.Space)) inputSubirBajar = 1f;
        else if (Input.GetKey(KeyCode.LeftShift)) inputSubirBajar = -1f;

        // 2. LEER INPUT DE CÁMARA (Mouse) Y ENVIARLO AL SCRIPT CamaraGTA
        if (scriptCamara != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

            // Aquí es donde ocurre la conexión:
            scriptCamara.RecibirInput(new Vector2(mouseX, mouseY));
        }
    }

    void FixedUpdate()
    {
        if (camaraPrincipal == null) return;

        Vector3 camFrontal = camaraPrincipal.forward;
        Vector3 camDerecha = camaraPrincipal.right;

        camFrontal.y = 0f; camFrontal.Normalize();
        camDerecha.y = 0f; camDerecha.Normalize();

        Vector3 direccionMovimiento = (camFrontal * inputVertical) + (camDerecha * inputHorizontal);
        Vector3 velocidadFinal = direccionMovimiento.normalized * velocidadVuelo;
        velocidadFinal.y += inputSubirBajar * velocidadVerticalPura;

        rb.MovePosition(rb.position + velocidadFinal * Time.fixedDeltaTime);

        if (direccionMovimiento.magnitude > 0.1f)
        {
            Quaternion nuevaRotacion = Quaternion.LookRotation(direccionMovimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, nuevaRotacion, suavizadoGiro * Time.fixedDeltaTime);
        }
    }
}