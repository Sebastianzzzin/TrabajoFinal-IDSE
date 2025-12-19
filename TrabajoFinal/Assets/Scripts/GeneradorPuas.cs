using UnityEngine;
using System.Collections;

public class GeneradorPuas : MonoBehaviour
{
     public GameObject puaPrefab; // El prefab que preparamos arriba
    public Transform centroEsfera; // El objeto vacío en el centro
    public float velocidadCrecimiento = 2f;
    public float largoMaximo = 3f;

    void Update()
    {
        // Ejemplo: Disparar con la tecla Espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CrearPua();
        }
    }

    void CrearPua()
    {
        // 1. Obtener una dirección aleatoria desde el centro
        Vector3 direccionAleatoria = Random.onUnitSphere; 
        
        // 2. Posición inicial (en la superficie de la esfera)
        // Ajusta el radio según el tamaño de tu esfera
        float radioEsfera = transform.localScale.x / 2;
        Vector3 posicionSpawn = centroEsfera.position + (direccionAleatoria * radioEsfera);

        // 3. Instanciar y rotar
        GameObject nuevaPua = Instantiate(puaPrefab, posicionSpawn, Quaternion.identity);
        
        // Hacer que la púa "mire" hacia afuera del centro
        nuevaPua.transform.up = direccionAleatoria;

        // 4. Iniciar el crecimiento
        StartCoroutine(CrecerPua(nuevaPua.transform));
    }

    IEnumerator CrecerPua(Transform puaTransform)
    {
        Vector3 escalaInicial = new Vector3(0.2f, 0f, 0.2f); // Empezamos con altura 0
        puaTransform.localScale = escalaInicial;

        float tiempo = 0;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadCrecimiento;
            
            // Escalamos solo el eje Y (el largo de la púa)
            float nuevoLargo = Mathf.Lerp(0, largoMaximo, tiempo);
            puaTransform.localScale = new Vector3(escalaInicial.x, nuevoLargo, escalaInicial.z);
            
            yield return null;
        }
    }
}
