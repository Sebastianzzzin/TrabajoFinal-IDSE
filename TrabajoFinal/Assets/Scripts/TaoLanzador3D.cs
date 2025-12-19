using System.Collections;
using UnityEngine;

public class TaoLanzador3D : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject pilarRealPrefab; // Arrastra aquí el PREFAB de tu carpeta (el que sí tiene script)
    public GameObject pilarFalsoEnMano; // Arrastra el objeto que está dentro de la mano (el que acabamos de arreglar)
    public Transform puntoDeDisparo;   // Arrastra de nuevo el objeto de la mano (RightHand)

    [Header("Ajustes")]
    public float retardoLanzamiento = 0.8f; // Tiempo exacto para soltar el pilar (ajusta a prueba y error)

    void Start()
    {
        // Al nacer, empezamos la secuencia
        StartCoroutine(LanzarAtaque());
    }

    IEnumerator LanzarAtaque()
    {
        // 1. Aseguramos que se vea el pilar falso en la mano
        pilarFalsoEnMano.SetActive(true);

        // 2. Esperamos al momento justo de la animación donde estira el brazo
        // (La animación ya se reproduce sola por el Animator que pusimos antes)
        yield return new WaitForSeconds(retardoLanzamiento);

        // 3. ¡EL CAMBAZO!
        // Ocultamos el de la mano
        pilarFalsoEnMano.SetActive(false);

        // Creamos el pilar REAL (el que gira y mata) en la posición de la mano
        Instantiate(pilarRealPrefab, puntoDeDisparo.position, Quaternion.identity);

        // 4. Esperamos un poco y borramos a Tao Pai Pai
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
