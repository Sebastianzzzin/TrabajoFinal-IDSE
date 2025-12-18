using System.Collections;
using UnityEngine;

public class GinyuIA : MonoBehaviour
{
    public GameObject rayoCambio;
    public Animator animador;

    private AudioSource audioGinyu;
    private Transform objetivoGoku;
    private LineRenderer lineaVisual;

    void Start()
    {
        objetivoGoku = GameObject.Find("Goku").transform;
        lineaVisual = rayoCambio.GetComponent<LineRenderer>();
        audioGinyu = GetComponent<AudioSource>();

        // 1. EL GRITO (Inmediato al nacer)
        if (audioGinyu != null)
        {
            audioGinyu.PlayOneShot(audioGinyu.clip, 1.0f);
        }

        StartCoroutine(Atacar());
    }

    IEnumerator Atacar()
    {
        // ====================================================
        // FASE 1: LA ADVERTENCIA (Aquí hacemos el cambio)
        // ====================================================
        // Antes era 0.5f. Lo subimos a 1.5f o 2.0f.
        // Esto le da tiempo de gritar "¡CAMBIO!" antes de hacer nada.
        float tiempoApuntar = 1.5f;

        while (tiempoApuntar > 0)
        {
            if (objetivoGoku != null)
            {
                // Te sigue con la mirada terroríficamente
                transform.LookAt(objetivoGoku.position + Vector3.up * 1.5f);
            }
            tiempoApuntar -= Time.deltaTime;
            yield return null;
        }

        // ====================================================
        // FASE 2: BLOQUEO DE OBJETIVO (Ya no te sigue)
        // ====================================================
        animador.Play("mixamo.com", 0, 0f);

        Vector3 puntoFinalDisparo = Vector3.zero;
        if (objetivoGoku != null)
        {
            Vector3 direccion = (objetivoGoku.position + Vector3.up * 1.0f) - rayoCambio.transform.position;
            puntoFinalDisparo = rayoCambio.transform.position + (direccion.normalized * 60f);
        }

        // Esperamos a que abra la boca (sincronizado con la animación)
        yield return new WaitForSeconds(0.3f);

        // ====================================================
        // FASE 3: DISPARO (El rayo sale ahora sí)
        // ====================================================
        rayoCambio.transform.SetParent(null); // Divorcio del rayo
        rayoCambio.transform.LookAt(puntoFinalDisparo);

        rayoCambio.SetActive(true);

        lineaVisual.SetPosition(0, rayoCambio.transform.position);
        lineaVisual.SetPosition(1, puntoFinalDisparo);

        yield return new WaitForSeconds(2.0f); // Duración del rayo

        Destroy(rayoCambio);
        Destroy(gameObject);
    }
}