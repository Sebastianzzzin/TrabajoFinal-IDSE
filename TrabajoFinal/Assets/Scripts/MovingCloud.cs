using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    public float speed = 1.8f;     // velocidad de oscilaci�n
    public float moveRange = 1.2f; // rango de movimiento (en X)
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * moveRange;
        transform.position = new Vector3(startPos.x + offset, transform.position.y, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aqu� puedes poner reacci�n: da�o, reiniciar, empujar, etc.
            Debug.Log("Goku chocado por nube m�vil");
        }
    }
}

