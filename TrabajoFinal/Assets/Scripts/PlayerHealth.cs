using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int vidaMaxima = 100;
    public int vidaActual;

    public Slider barraVida;

    public float tiempoInmunidad = 1f;
    bool esInmune = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        barraVida.maxValue = vidaMaxima;
        barraVida.value = vidaActual;
    }

    // SI EL OBST�CULO ES TRIGGER
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            IntentarDanio();
        }
    }

    // SI EL OBST�CULO ES COLLISION NORMAL
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

        vidaActual -= 20;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        barraVida.value = vidaActual;

        if (vidaActual <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(InmunidadTemporal());
        }
    }

    System.Collections.IEnumerator InmunidadTemporal()
    {
        esInmune = true;
        yield return new WaitForSeconds(tiempoInmunidad);
        esInmune = false;
    }
}



