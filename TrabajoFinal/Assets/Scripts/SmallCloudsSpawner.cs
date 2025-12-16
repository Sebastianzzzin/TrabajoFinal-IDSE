using UnityEngine;

public class SmallCloudsSpawner : MonoBehaviour
{
    public GameObject cloudPrefab;
    public int numberOfClouds = 10;

    // Rango horizontal entre start (X de Goku) y posición del templo
    public float minX = -15f;
    public float maxX = 15f;

    // Altura en la que aparecerán (Y)
    public float minY = -2f;
    public float maxY = 4f;

    // Z debe coincidir con la Z del jugador/escenario
    public float spawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < numberOfClouds; i++)
        {
            float yPos = Random.Range(minY, maxY);

            // Evitar que salgan muchas a la misma altura
            if (i % 2 == 0)
                yPos += 1f;
            else
                yPos -= 1f;

            Vector3 pos = new Vector3(
                Random.Range(minX, maxX),
                yPos,
                spawnZ
            );

            GameObject cloud = Instantiate(cloudPrefab, pos, Quaternion.identity, transform);

            // Variación random
            MovingCloud mc = cloud.GetComponent<MovingCloud>();
            if (mc != null)
            {
                mc.speed = Random.Range(0.8f, 2.5f);
                mc.moveRange = Random.Range(0.6f, 2.0f);
            }

            float scale = Random.Range(0.3f, 0.6f);
            cloud.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}

