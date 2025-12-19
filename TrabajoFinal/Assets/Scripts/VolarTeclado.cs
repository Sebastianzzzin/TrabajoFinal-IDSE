using UnityEngine;

public class VolarTeclado : MonoBehaviour
{
    // Variable para controlar la velocidad desde el editor
    public float velocidad = 5f;

    // Referencia al componente físico
    private Rigidbody miRigidbody;
    void Start()
    {
        miRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Leemos las teclas (WASD o Flechas)
        // Horizontal = A/D o Izquierda/Derecha
        // Vertical = W/S o Arriba/Abajo
        float movimientoX = Input.GetAxis("Horizontal");
        float movimientoY = Input.GetAxis("Vertical");

        // Calculamos el vector de movimiento (X, Y, 0 en Z)
        Vector3 direccionVuelo = new Vector3(movimientoX, movimientoY, 0);

        // Movemos el objeto usando su velocidad física
        // Usamos direccionVuelo * velocidad
        miRigidbody.linearVelocity = direccionVuelo * velocidad;
    }
    void OnTriggerEnter(Collider other)
    {
        // Preguntamos: "¿Lo que toqué tiene la etiqueta 'Enemigo'?"
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("¡PERDISTE! Goku chocó con la nube.");

            // OPCIONAL: Reiniciar el nivel automáticamente
            // Para que esto funcione, necesitas agregar: using UnityEngine.SceneManagement; 
            // al principio de todo tu script.
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // Por ahora, destruyamos a Goku para simular que murió
            Destroy(this.gameObject);
        }
    }
}
