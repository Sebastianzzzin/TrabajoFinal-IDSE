using UnityEngine;

public class GeneradorAmbiental : MonoBehaviour
{
    [Header("Configuración General")]
    public GameObject prefabColumnaLava;
    public int cantidadDeColumnas = 50;

    [Header("Tamaño del Mapa")]
    public float radioDelMapa = 140f;

    [Header("Zona Segura (Donde NO aparecen)")]
    public float radioZonaSegura = 15f;
    [Tooltip("Posición X, Z de la zona segura relativa al centro del generador")]
    public Vector2 offsetZonaSegura = Vector2.zero; // X es izquierda/derecha, Y es adelante/atrás

    [Header("Variedad de Altura")]
    public float alturaMinima = 4f;
    public float alturaMaxima = 12f;

    [Header("Zonas Activas (Distribución Numpad)")]
    // 7 8 9
    // 4 5 6
    // 1 2 3
    public bool zona7 = true; public bool zona8 = true; public bool zona9 = true;
    public bool zona4 = true; public bool zona5 = true; public bool zona6 = true;
    public bool zona1 = true; public bool zona2 = true; public bool zona3 = true;

    void Start()
    {
        GenerarMapa();
    }

    void GenerarMapa()
    {
        int columnasGeneradas = 0;
        int intentos = 0;
        int maxIntentos = cantidadDeColumnas * 100; // Evitar bucle infinito

        while (columnasGeneradas < cantidadDeColumnas && intentos < maxIntentos)
        {
            intentos++;

            // 1. Generar punto aleatorio dentro del círculo
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioDelMapa;
            Vector3 posicionLocal = new Vector3(puntoAleatorio.x, 0, puntoAleatorio.y);

            // 2. Verificar si está dentro de la Zona Segura (Ahora es movible)
            // Convertimos el offsetZonaSegura a Vector3 (x, 0, y)
            Vector3 centroSeguroLocal = new Vector3(offsetZonaSegura.x, 0, offsetZonaSegura.y);
            
            if (Vector3.Distance(posicionLocal, centroSeguroLocal) < radioZonaSegura)
            {
                continue; // Está en zona segura, saltar
            }

            // 3. Verificar si está en una Zona del Numpad desactivada
            if (!EstaEnZonaActiva(posicionLocal))
            {
                continue; // La zona está apagada, saltar
            }

            // 4. Instanciar
            Vector3 posicionMundo = transform.position + posicionLocal;
            GameObject nuevaColumna = Instantiate(prefabColumnaLava, posicionMundo, Quaternion.identity);

            ColumnaLava script = nuevaColumna.GetComponent<ColumnaLava>();
            if (script != null)
            {
                script.esPermanente = true;
                script.alturaFinal = Random.Range(alturaMinima, alturaMaxima);
            }

            nuevaColumna.transform.SetParent(this.transform);
            columnasGeneradas++;
        }

        if (intentos >= maxIntentos)
        {
            Debug.LogWarning("No se pudieron generar todas las columnas. Revisa si has desactivado demasiadas zonas.");
        }
    }

    // Lógica para determinar si el punto cae en una casilla activada del Numpad
    bool EstaEnZonaActiva(Vector3 pos)
    {
        // El tamaño total del cuadrado que encierra el círculo es radio * 2
        // Dividimos en 3 partes
        float tercio = (radioDelMapa * 2) / 3f;
        float limiteIzquierdo = -tercio / 2f - tercio; // Matemáticas para centrar la grilla en (0,0)
        
        // Simplificamos coordenadas normalizadas de -1 a 1 aproximadamente para facilitar la grilla
        // Mapeamos:
        // Columna Izquierda (1,4,7): X < -tercio/2
        // Columna Derecha   (3,6,9): X > tercio/2
        // Columna Central   (2,5,8): El resto
        
        int columna = 0; // 0: Izq, 1: Centro, 2: Der
        if (pos.x < -radioDelMapa / 3f) columna = 0;
        else if (pos.x > radioDelMapa / 3f) columna = 2;
        else columna = 1;

        int fila = 0; // 0: Abajo, 1: Centro, 2: Arriba
        if (pos.z < -radioDelMapa / 3f) fila = 0;
        else if (pos.z > radioDelMapa / 3f) fila = 2;
        else fila = 1;

        // Mapeo al Numpad
        // Fila 2 (Arriba): 7, 8, 9
        // Fila 1 (Medio) : 4, 5, 6
        // Fila 0 (Abajo) : 1, 2, 3

        if (fila == 2 && columna == 0) return zona7;
        if (fila == 2 && columna == 1) return zona8;
        if (fila == 2 && columna == 2) return zona9;

        if (fila == 1 && columna == 0) return zona4;
        if (fila == 1 && columna == 1) return zona5;
        if (fila == 1 && columna == 2) return zona6;

        if (fila == 0 && columna == 0) return zona1;
        if (fila == 0 && columna == 1) return zona2;
        if (fila == 0 && columna == 2) return zona3;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        // 1. Dibujar el Límite Exterior
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDelMapa);

        // 2. Dibujar la Grilla del Numpad (Visual)
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        float tercio = (radioDelMapa * 2) / 3f;
        float linea = radioDelMapa / 3f;
        float tamano = radioDelMapa; 

        // Líneas Verticales
        Gizmos.DrawLine(transform.position + new Vector3(-linea, 0, -tamano), transform.position + new Vector3(-linea, 0, tamano));
        Gizmos.DrawLine(transform.position + new Vector3(linea, 0, -tamano), transform.position + new Vector3(linea, 0, tamano));
        // Líneas Horizontales
        Gizmos.DrawLine(transform.position + new Vector3(-tamano, 0, -linea), transform.position + new Vector3(tamano, 0, -linea));
        Gizmos.DrawLine(transform.position + new Vector3(-tamano, 0, linea), transform.position + new Vector3(tamano, 0, linea));

        // 3. Dibujar Zonas Activas/Inactivas
        // Recorremos las zonas para dibujar un cubo verde (activo) o rojo (inactivo) transparente
        DrawZoneGizmo(-1, -1, zona1); // 1
        DrawZoneGizmo( 0, -1, zona2); // 2
        DrawZoneGizmo( 1, -1, zona3); // 3
        DrawZoneGizmo(-1,  0, zona4); // 4
        DrawZoneGizmo( 0,  0, zona5); // 5
        DrawZoneGizmo( 1,  0, zona6); // 6
        DrawZoneGizmo(-1,  1, zona7); // 7
        DrawZoneGizmo( 0,  1, zona8); // 8
        DrawZoneGizmo( 1,  1, zona9); // 9

        // 4. Dibujar la Zona Segura (Móvil)
        Gizmos.color = Color.red;
        Vector3 centroSeguroMundo = transform.position + new Vector3(offsetZonaSegura.x, 0, offsetZonaSegura.y);
        Gizmos.DrawWireSphere(centroSeguroMundo, radioZonaSegura);
        // Una linea pequeña para ver donde está el centro seguro
        Gizmos.DrawLine(transform.position, centroSeguroMundo);
    }

    void DrawZoneGizmo(int colOffset, int rowOffset, bool activa)
    {
        float size = (radioDelMapa * 2) / 3f;
        float dist = size; // Distancia entre centros
        
        Vector3 centerLocal = new Vector3(colOffset * dist, 0, rowOffset * dist);
        Vector3 centerWorld = transform.position + centerLocal;

        if (activa) Gizmos.color = new Color(0, 1, 0, 0.1f); // Verde transparente
        else Gizmos.color = new Color(1, 0, 0, 0.3f);       // Rojo más visible

        Gizmos.DrawCube(centerWorld, new Vector3(size - 1, 1, size - 1));
    }
}