using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("--- BARRAS PRINCIPALES ---")]
    public Image healthBarFill;       
    public Image turboBarFill;        
    public GameObject portraitGoku;   

    [Header("--- COMBUSTIBLE (KI) ---")]
    public GameObject[] fuelSlots;    // Los 5 slots de siempre

    [Header("--- CARGAS DE TURBO (Flechas Verdes) ---")]
    // Arrastra aquí tus objetos "Slot", "Slot (1)", etc. de Turbo Charges
    public GameObject[] turboChargeIcons; 

    [Header("--- IMÁGENES DE VIDAS ---")]
    // Arrastra aquí tus objetos "1", "2", "3" que están dentro de VidasPanel
    public GameObject[] vidaImages; 

    // --- ACTUALIZAR BARRAS (Igual que antes) ---
    public void ActualizarVida(int actual, int maximo)
    {
        float porcentaje = (float)actual / maximo;
        healthBarFill.fillAmount = porcentaje;
    }

    public void ActualizarTurbo(float actual, float maximo)
    {
        turboBarFill.fillAmount = actual / maximo;
    }

    // --- NUEVO: SISTEMA DE COMBUSTIBLE GLOBAL (Celeste/Amarillo) ---
    public void ActualizarCombustible(float valorActual, float valorMaximo)
    {
        float mitadTanque = valorMaximo / 2f;
        float porcentajeGlobalAmarillo = Mathf.Clamp01(valorActual / mitadTanque);
        float porcentajeGlobalCeleste = Mathf.Clamp01((valorActual - mitadTanque) / mitadTanque);

        int totalBarras = fuelSlots.Length;
        float pasoPorBarra = 1f / totalBarras; 

        for (int i = 0; i < totalBarras; i++)
        {
            Image[] capas = fuelSlots[i].GetComponentsInChildren<Image>();
            if (capas.Length < 3) continue;

            float inicioBarra = i * pasoPorBarra;
            
            // Amarillo
            float llenadoAmarillo = (porcentajeGlobalAmarillo - inicioBarra) / pasoPorBarra;
            capas[1].fillAmount = Mathf.Clamp01(llenadoAmarillo);

            // Celeste
            float llenadoCeleste = (porcentajeGlobalCeleste - inicioBarra) / pasoPorBarra;
            capas[2].fillAmount = Mathf.Clamp01(llenadoCeleste);
        }
    }

    // --- NUEVO: ACTUALIZAR CARGAS DE TURBO (Flechas) ---
    public void ActualizarCargasTurbo(int cantidadActual)
    {
        // Recorremos todas las flechas
        for (int i = 0; i < turboChargeIcons.Length; i++)
        {
            // Si el índice es menor que la cantidad que tenemos, se enciende.
            // Ejemplo: Tengo 3 cargas. i=0(On), i=1(On), i=2(On), i=3(Off)...
            if (i < cantidadActual)
            {
                turboChargeIcons[i].SetActive(true);
            }
            else
            {
                turboChargeIcons[i].SetActive(false);
            }
        }
    }

    // --- NUEVO: ACTUALIZAR IMÁGENES DE VIDAS ---
    public void ActualizarImagenVidas(int vidasRestantes)
    {
        // Primero apagamos TODAS las imágenes (1, 2 y 3)
        foreach (GameObject img in vidaImages)
        {
            img.SetActive(false);
        }

        // Ahora encendemos solo la que corresponde
        // Asumimos que en el array: Element 0 = Imagen "1", Element 1 = Imagen "2", Element 2 = Imagen "3"
        // Como vidasRestantes es 1, 2 o 3, restamos 1 para obtener el índice.
        
        int index = vidasRestantes - 1;

        if (index >= 0 && index < vidaImages.Length)
        {
            vidaImages[index].SetActive(true);
        }
    }
}