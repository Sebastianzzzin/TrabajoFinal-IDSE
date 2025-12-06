using UnityEngine;
using UnityEngine.UI;

public class PulseEffect : MonoBehaviour
{
    private Image imagenGlow;
    public float velocidad = 2f;
    public float minAlpha = 0.1f; // Casi transparente
    public float maxAlpha = 0.6f; // Bastante visible

    void Start()
    {
        imagenGlow = GetComponent<Image>();
    }

    void Update()
    {
        // Matemáticas de onda senoidal para hacer un ping-pong suave
        float alpha = Mathf.PingPong(Time.time * velocidad, maxAlpha - minAlpha) + minAlpha;
        
        // Aplicamos el nuevo alpha sin cambiar el color base
        Color c = imagenGlow.color;
        c.a = alpha;
        imagenGlow.color = c;
    }
}