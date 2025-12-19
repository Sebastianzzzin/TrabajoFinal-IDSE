using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class VueloTeclado3D : MonoBehaviour
{
    [Header("Ajustes de Vuelo")]
    public float moveSpeed = 25f;
    public float verticalSpeed = 15f;
    public float rotationSpeed = 10f;

    [Header("Referencias")]
    public CamaraGTA scriptCamara;

    private Rigidbody rb;
    private bool puedeMoverse = true; // Igual que el de tu equipo

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Configuración física vital
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // <--- VITAL
    }

    // --- FUNCIÓN DE COMPATIBILIDAD CON TU EQUIPO ---
    public void RecibirImpactoRebote(float tiempoAturdimiento)
    {
        StartCoroutine(RutinaAturdimiento(tiempoAturdimiento));
    }

    private IEnumerator RutinaAturdimiento(float tiempo)
    {
        puedeMoverse = false;
        yield return new WaitForSeconds(tiempo);
        puedeMoverse = true;
    }

    void Update()
    {
        if (!puedeMoverse) return;

        // Leer Mouse para la cámara
        if (scriptCamara != null)
        {
            float mX = Input.GetAxis("Mouse X");
            float mY = Input.GetAxis("Mouse Y");
            scriptCamara.RecibirInput(new Vector2(mX, mY));
        }

        // Leer Teclado
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float subirBajar = 0;
        if (Input.GetKey(KeyCode.Space)) subirBajar = 1;
        if (Input.GetKey(KeyCode.LeftShift)) subirBajar = -1;

        MoverGoku(new Vector2(h, v), subirBajar);
    }

    void MoverGoku(Vector2 input, float vertical)
    {
        if (scriptCamara == null) return;

        // Direcciones relativas a la cámara
        Vector3 fwd = scriptCamara.transform.forward;
        Vector3 right = scriptCamara.transform.right;
        fwd.y = 0; right.y = 0;
        fwd.Normalize(); right.Normalize();

        Vector3 dir = (fwd * input.y + right * input.x).normalized;

        // Movimiento usando Rigidbody (Es más seguro para colisiones)
        // Si prefieres el estilo de tu equipo, cambia esto por: transform.position += ...
        Vector3 velHorizontal = dir * moveSpeed;
        Vector3 velVertical = Vector3.up * (vertical * verticalSpeed);

        rb.linearVelocity = velHorizontal + velVertical;

        // Rotación
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
        }
    }
}