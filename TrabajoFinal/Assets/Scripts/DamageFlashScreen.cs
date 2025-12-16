using UnityEngine;
using UnityEngine.UI;

public class DamageFlashScreen : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.3f;
    public Color flashColor = new Color(1, 0, 0, 0.6f); // Rojo con transparencia

    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        originalColor = flashImage.color;
    }

    public void Flash()
    {
        if (!isFlashing)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        isFlashing = true;
        flashImage.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        flashImage.color = originalColor;
        isFlashing = false;
    }
}

