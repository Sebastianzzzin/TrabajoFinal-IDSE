using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("--- REFERENCIAS UI ---")]
    public Image healthBarFill;       // Barra Verde
    public Image turboBarFill;        // Barra Azul (Turbo)
    public TextMeshProUGUI livesText; // Texto Vidas
    public GameObject portraitGoku;   // La cara de Goku (Opcional, para animarla si quieres)

    [Header("--- COMBUSTIBLE (Sistema de Capas) ---")]
    public GameObject[] fuelSlots;    // Arrastra los padres (Slot_Fondo)
    // El script buscará automáticamente dentro: 
    // Hijo 0: Fondo Gris, Hijo 1: Amarillo, Hijo 2: Celeste

    [Header("--- ESFERAS DEL DRAGON ---")]
    public Image[] dragonBallSlots;   // Los huecos grises en la UI
    public Sprite[] dragonBallSprites;// Tus 7 PNGs de las esferas

    public void ActualizarVida(int actual, int maximo)
    {
        float porcentaje = (float)actual / maximo;
        healthBarFill.fillAmount = porcentaje;
    }

    public void ActualizarTurbo(float actual, float maximo)
    {
        turboBarFill.fillAmount = actual / maximo;
    }

    public void ActualizarVidas(int vidas)
    {
        livesText.text = "" + vidas.ToString();
    }

 public void ActualizarCombustible(float valorActual, float valorMaximo)
    {
        // 1. Calculamos los porcentajes globales de cada color
        // El amarillo representa la PRIMERA mitad del tanque (0% a 50%)
        // El celeste representa la SEGUNDA mitad del tanque (50% a 100%)
        
        float mitadTanque = valorMaximo / 2f;

        // Cálculo Amarillo: Llenamos de 0 a mitadTanque.
        // Si valorActual > mitadTanque, esto dará > 1 (que Clamp01 recorta a 1, o sea lleno)
        float porcentajeGlobalAmarillo = Mathf.Clamp01(valorActual / mitadTanque);

        // Cálculo Celeste: Llenamos solo lo que sobrepase la mitad.
        float porcentajeGlobalCeleste = Mathf.Clamp01((valorActual - mitadTanque) / mitadTanque);

        // 2. Distribuimos ese porcentaje entre las barritas individuales
        int totalBarras = fuelSlots.Length;
        
        // Cuánto porcentaje del total representa una sola barrita (ej: 5 barras = 0.2 cada una)
        float pasoPorBarra = 1f / totalBarras; 

        for (int i = 0; i < totalBarras; i++)
        {
            Image[] capas = fuelSlots[i].GetComponentsInChildren<Image>();
            if (capas.Length < 3) continue;

            Image capaAmarilla = capas[1];
            Image capaCeleste = capas[2];

            // --- MATEMÁTICA DE DISTRIBUCIÓN ---
            // Queremos saber cuánto de este bloque 'i' debe llenarse basado en el global.
            // Fórmula: (Global - InicioDelBloque) / TamañoDelBloque
            
            float inicioBarra = i * pasoPorBarra; // ej: 0, 0.2, 0.4...

            // Llenado AMARILLO para esta barra
            float llenadoAmarillo = (porcentajeGlobalAmarillo - inicioBarra) / pasoPorBarra;
            capaAmarilla.fillAmount = Mathf.Clamp01(llenadoAmarillo);

            // Llenado CELESTE para esta barra
            float llenadoCeleste = (porcentajeGlobalCeleste - inicioBarra) / pasoPorBarra;
            capaCeleste.fillAmount = Mathf.Clamp01(llenadoCeleste);
        }
    }

    public void RecolectarEsfera(int numeroEsfera)
    {
        int index = numeroEsfera - 1; // Array empieza en 0
        if (index >= 0 && index < dragonBallSlots.Length)
        {
            dragonBallSlots[index].sprite = dragonBallSprites[index];
            dragonBallSlots[index].color = Color.white; // Quitar el gris
        }
    }
}