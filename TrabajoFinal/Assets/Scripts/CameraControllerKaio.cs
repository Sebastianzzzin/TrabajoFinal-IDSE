using UnityEngine;

public class CamaraControllerKaio : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform target;       // El Player (Goku)
    public Transform planetaCentro; // El mismo objeto que pusiste en el Player

    [Header("Configuración")]
    public float distancia = 6.0f;
    public float altura = 2.0f;
    public float sensibilidadX = 200.0f;
    public float sensibilidadY = 150.0f;
    public float limiteMinY = -30f; // Mirar hacia abajo
    public float limiteMaxY = 60f;  // Mirar hacia arriba
    public float velocidadSuavizado = 10f;

    public LayerMask capasColision;

    private float rotacionX = 0f;
    private float rotacionY = 20f; // Empezar un poco elevada
    private Vector2 inputRecibido;
    private float distanciaActual;

    void Start()
    {
        distanciaActual = distancia;
        if (!target || !planetaCentro) 
            Debug.LogWarning("¡Asigna Target y PlanetaCentro en la cámara!");
    }

    public void RecibirInput(Vector2 input)
    {
        inputRecibido = input;
    }

    void LateUpdate()
    {
        if (!target || !planetaCentro) return;

        // 1. Procesar Input
        rotacionX += inputRecibido.x * sensibilidadX * Time.deltaTime;
        rotacionY -= inputRecibido.y * sensibilidadY * Time.deltaTime;
        rotacionY = Mathf.Clamp(rotacionY, limiteMinY, limiteMaxY);

        // 2. Definir "Arriba" local (La normal de la superficie en la posición del jugador)
        Vector3 worldUp = (target.position - planetaCentro.position).normalized;

        // 3. Calcular la rotación deseada
        //    Primero, creamos la rotación local basada en el input (como si estuvieramos en plano)
        Quaternion rotacionInput = Quaternion.Euler(rotacionY, rotacionX, 0);

        //    Luego, necesitamos una rotación que alinee el vector UP global con el worldUp del planeta
        //    Esto es un poco truco matemático:
        //    Miramos desde el centro del planeta hacia el target.
        Quaternion orientacionPlaneta = Quaternion.LookRotation(target.forward, worldUp);

        // 4. Posición deseada sin colisión
        //    Calculamos la posición relativa al target usando la rotación input, 
        //    pero todo transformado por la orientación del planeta.
        Vector3 direccionNegativa = new Vector3(0, 0, -distancia);
        //    Añadimos un offset de altura local
        Vector3 offsetAltura = new Vector3(0, altura, 0);

        //    La magia: Rotamos el vector "atrás" por el input, y luego alineamos todo al planeta
        Vector3 posicionFinal = target.position + (orientacionPlaneta * rotacionInput * (direccionNegativa + offsetAltura));

        // 5. Colisiones (SphereCast)
        //    Lanzamos rayo desde el jugador hacia la cámara
        Vector3 direccionHaciaCamara = posicionFinal - target.position;
        RaycastHit hit;
        if (Physics.SphereCast(target.position, 0.2f, direccionHaciaCamara.normalized, out hit, distancia, capasColision))
        {
            // Si choca, acortamos la distancia
            posicionFinal = target.position + (direccionHaciaCamara.normalized * hit.distance);
        }

        // 6. Aplicar Transform
        //    Suavizamos el movimiento
        transform.position = Vector3.Lerp(transform.position, posicionFinal, Time.deltaTime * velocidadSuavizado);

        //    La cámara siempre debe mirar al jugador, pero manteniendo el "Up" del planeta
        //    para no marearnos
        transform.LookAt(target.position + (orientacionPlaneta * Vector3.up * altura), worldUp);
    }
}