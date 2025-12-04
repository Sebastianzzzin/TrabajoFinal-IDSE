using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform puntoNariz;

    [Header("Comportamiento")]
    [Tooltip("Si está activado, la cámara se parenta a 'puntoNariz' y su posición local queda en (0,0,0).")]
    public bool parentToNose = true;

    [Tooltip("Si no parentToNose, usa seguimiento suave de posición/rotación.")]
    public bool smoothWhenUnparented = true;

    [Header("Suavizado (solo si no está parentada)")]
    public float suavizado = 10f;

    void Start()
    {
        // Si quieres que 'nazca' en el punto desde el inicio
        ForceSetup();
    }

    // Llamar cuando cambies de cámara para colocarla exactamente en la nariz
    public void ForceSetup()
    {
        if (puntoNariz == null) return;

        if (parentToNose)
        {
            // Parentar y resetear local transform -> la cámara "nace" exactamente en puntoNariz
            transform.SetParent(puntoNariz, false); // false = conservar transform relativo (usar local)
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Asegurarse de quitar parenting si estaba puesto antes
            transform.SetParent(null, true); // true = preservar transform en world
            transform.position = puntoNariz.position;
            transform.rotation = Quaternion.LookRotation(puntoNariz.forward, Vector3.up);
        }
    }

    void LateUpdate()
    {
        if (puntoNariz == null) return;

        if (parentToNose)
        {
            // Si está parentada, ya sigue posición exacta de puntoNariz.
            // Mantén localPosition/localRotation en cero por seguridad (evita offsets indeseados).
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            return;
        }

        // Si NO está parentada: seguimiento (posición instantánea o suavizada)
        if (smoothWhenUnparented)
        {
            transform.position = Vector3.Lerp(transform.position, puntoNariz.position, suavizado * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(puntoNariz.forward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, suavizado * Time.deltaTime);
        }
        else
        {
            transform.position = puntoNariz.position;
            transform.rotation = Quaternion.LookRotation(puntoNariz.forward, Vector3.up);
        }
    }
}
