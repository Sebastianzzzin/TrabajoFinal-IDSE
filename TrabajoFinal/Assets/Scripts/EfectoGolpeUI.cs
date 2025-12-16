using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))] // Esto obliga a que el panel tenga CanvasGroup
public class EfectoGolpeUI : MonoBehaviour
{
    public static EfectoGolpeUI Instance;

    private RectTransform rectTransform;
    private Vector2 posicionOriginal;
    private CanvasGroup canvasGroup; // Controla la visibilidad

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        posicionOriginal = rectTransform.anchoredPosition;
        
        // AL INICIAR: Lo hacemos invisible inmediatamente
        canvasGroup.alpha = 0f; 
    }

    public void Sacudir(float duracion, float fuerza)
    {
        StopAllCoroutines();
        StartCoroutine(RutinaSacudida(duracion, fuerza));
    }

    private IEnumerator RutinaSacudida(float duracion, float fuerza)
    {
        // 1. HACER VISIBLE EL PANEL (Alpha 1 = Opaco, 0.5 = Semi-transparente)
        canvasGroup.alpha = 1f; 

        float tiempo = 0;
        
        while (tiempo < duracion)
        {
            Vector2 desplazamiento = Random.insideUnitCircle * fuerza;
            rectTransform.anchoredPosition = posicionOriginal + desplazamiento;

            tiempo += Time.deltaTime;
            yield return null;
        }

        // 2. AL TERMINAR: Regresar al centro y hacerlo INVISIBLE otra vez
        rectTransform.anchoredPosition = posicionOriginal;
        canvasGroup.alpha = 0f; 
    }
}