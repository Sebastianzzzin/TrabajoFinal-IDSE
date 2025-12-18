using UnityEngine;
using System.Collections;

public class GeneradorPuasIrregular : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject puaPrefab;
    public Transform centroDelPlaneta; // Arrastra aquí tu objeto central
    public int cantidadDePuas = 50;
    
    [Tooltip("Distancia desde donde se dispara el rayo hacia el centro (debe ser mayor que el planeta)")]
    public float distanciaOrbital = 100f; 
    
    [Header("Filtros")]
    public LayerMask capaPlaneta; // Para asegurarnos que el rayo solo golpee al planeta

    [Header("Variación")]
    public float alturaMinima = 1f;
    public float alturaMaxima = 4f;
    public float velocidadCrecimiento = 3f;

    void Start()
    {
        if (centroDelPlaneta == null) centroDelPlaneta = transform; // Por si olvidas asignarlo
        GenerarPuasEnSuperficie();
    }

    void GenerarPuasEnSuperficie()
    {
        for (int i = 0; i < cantidadDePuas; i++)
        {
            // 1. Dirección aleatoria desde el centro
            Vector3 direccionSalida = Random.onUnitSphere;

            // 2. Nos posicionamos lejos, fuera del planeta
            Vector3 origenRayo = centroDelPlaneta.position + (direccionSalida * distanciaOrbital);

            // 3. Disparamos el rayo hacia el centro
            RaycastHit hit;
            // La dirección del rayo es opuesta a la de salida (hacia el centro)
            if (Physics.Raycast(origenRayo, -direccionSalida, out hit, distanciaOrbital + 10f, capaPlaneta))
            {
                // ¡Golpeamos la superficie! hit.point es la posición exacta en la malla irregular
                spawnearPua(hit.point, direccionSalida);
            }
        }
    }

    void spawnearPua(Vector3 posicion, Vector3 direccionDesdeCentro)
    {
        // Instanciar
        GameObject nuevaPua = Instantiate(puaPrefab, posicion, Quaternion.identity);
        
        // Hacemos la púa hija del planeta para mantener el orden
        nuevaPua.transform.parent = centroDelPlaneta;

        // ALINEACIÓN:
        // Opción A: Que apunte según el centro (como pediste) -> Estilo Explosión/Estrella
        nuevaPua.transform.up = direccionDesdeCentro;

        // Opción B: Que apunte según la inclinación del terreno (normal) -> Estilo Vegetación
        // nuevaPua.transform.up = hit.normal; // (Si prefieres esto, tendrías que pasar hit.normal a esta función)

        // Crecimiento
        float alturaFinal = Random.Range(alturaMinima, alturaMaxima);
        StartCoroutine(CrecerPua(nuevaPua.transform, alturaFinal));
    }

    IEnumerator CrecerPua(Transform pua, float alturaObjetivo)
    {
        float tiempo = 0;
        Vector3 escalaInicial = new Vector3(pua.localScale.x, 0f, pua.localScale.z);
        pua.localScale = escalaInicial;

        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadCrecimiento;
            float nuevaAltura = Mathf.Lerp(0f, alturaObjetivo, tiempo);
            pua.localScale = new Vector3(escalaInicial.x, nuevaAltura, escalaInicial.z);
            yield return null;
        }
    }
}