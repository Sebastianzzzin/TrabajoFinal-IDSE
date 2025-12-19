using UnityEngine;

public class ZonaSeguraController : MonoBehaviour
{
    private Transform goku;
    private GameObject generador;

    // Distancia del radio del domo (Ajustable en el inspector)
    public float radioDeProteccion = 15f;

    void Start()
    {
        // Buscamos a Goku y al Generador al inicio
        GameObject objGoku = GameObject.Find("Goku");
        if (objGoku != null) goku = objGoku.transform;

        generador = GameObject.Find("GeneradorDeAtaques");
    }

    void Update()
    {
        if (goku == null) return;

        // MATEMÁTICA PURA: Calculamos la distancia entre el Domo y Goku
        float distancia = Vector3.Distance(transform.position, goku.position);

        // Si la distancia es menor al radio (Goku está dentro)
        if (distancia < radioDeProteccion)
        {
            // 1. APAGAR EL GENERADOR (Si sigue prendido)
            if (generador != null && generador.activeSelf)
            {
                generador.SetActive(false);
                Debug.Log(">>> ZONA SEGURA: Generador Apagado <<<");
            }

            // 2. MATAR A CUALQUIER GINYU QUE EXISTA
            // Buscamos si hay un Ginyu vivo en este instante
            GinyuIA enemigo = FindObjectOfType<GinyuIA>();
            if (enemigo != null)
            {
                Destroy(enemigo.gameObject);
                Debug.Log(">>> ZONA SEGURA: Ginyu desintegrado <<<");
            }
        }
        else
        {
            // Opcional: Si sales del domo, ¿quieres que vuelvan a atacar?
            // Si es así, descomenta las siguientes lineas:
            /*
            if (generador != null && !generador.activeSelf)
            {
                generador.SetActive(true);
            }
            */
        }
    }

    // Esto dibuja una bola roja/verde en el editor para que veas el tamaño del área
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Verde transparente
        Gizmos.DrawSphere(transform.position, radioDeProteccion);
    }
}