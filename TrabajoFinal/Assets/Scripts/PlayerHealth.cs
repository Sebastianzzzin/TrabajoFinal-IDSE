using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int vidaMaxima = 100;
    public int vidaActual;

    public Slider barraVida;

    void Start()
    {
        vidaActual = vidaMaxima;  // Fuerza siempre a 100 al iniciar
        ActualizarBarra();
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            RecibirDano(20);
            Debug.Log("Jugador ha recibido dano por colision con obstulo.");
        }
    }

    void RecibirDano(int dano)
    {
        vidaActual -= dano;

        if (vidaActual < 0)
            vidaActual = 0;

        ActualizarBarra();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void ActualizarBarra()
    {
        if (barraVida != null)
            barraVida.value = vidaActual;
    }

    void Morir()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

