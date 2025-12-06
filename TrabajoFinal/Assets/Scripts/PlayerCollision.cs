using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private DamageFlashScreen screenFlash;

    void Start()
    {
        screenFlash = FindObjectOfType<DamageFlashScreen>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Goku chocó con una nube!");
            if (screenFlash != null)
                screenFlash.Flash();
        }
    }
}

