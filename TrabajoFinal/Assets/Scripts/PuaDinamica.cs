using UnityEngine;
using System.Collections;

public class PuaDinamica : MonoBehaviour
{
    private ControladorPlaneta miPlaneta;
    private float alturaMaxima;
    private float velocidad;
    // Esta función la llamará el planeta al crear la púa para configurarla
    public void Inicializar(ControladorPlaneta planeta, float altura, float vel)
    {
        miPlaneta = planeta;
        alturaMaxima = altura;
        velocidad = vel;
        
        // Arrancar el ciclo de vida
        StartCoroutine(CicloDeVida());
    }

    IEnumerator CicloDeVida()
    {
        float tiempo = 0f;
        Vector3 escalaBase = transform.localScale;
        
        // --- FASE 1: CRECER ---
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidad;
            float alturaActual = Mathf.Lerp(0f, alturaMaxima, tiempo);
            transform.localScale = new Vector3(escalaBase.x, alturaActual, escalaBase.z);
            yield return null;
        }

        // (Opcional) Esperar un poquito arriba para que no sea tan frenético
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        // --- FASE 2: ENCOGER ---
        tiempo = 0f;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidad;
            // Lerp inverso: va de alturaMaxima a 0
            float alturaActual = Mathf.Lerp(alturaMaxima, 0f, tiempo);
            transform.localScale = new Vector3(escalaBase.x, alturaActual, escalaBase.z);
            yield return null;
        }

        // --- FASE 3: MUERTE Y RENACIMIENTO ---
        // Avisamos al planeta que cree una nueva púa en otro lado
        if (miPlaneta != null)
        {
            miPlaneta.SolicitarNuevaPua();
        }

        // Destruimos este objeto
        Destroy(gameObject);
    }
}