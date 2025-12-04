using UnityEngine;

public class AutoMeshCollider : MonoBehaviour
{
    void Start()
    {
        AgregarCollidersRecursivo(transform);
        Debug.Log("Mesh Colliders generados en todos los subobjetos.");
    }

    void AgregarCollidersRecursivo(Transform objeto)
    {
        // Si el objeto tiene un MeshFilter (o sea, una malla)
        if (objeto.GetComponent<MeshFilter>() != null)
        {
            // Si no tiene ya un collider
            if (objeto.GetComponent<Collider>() == null)
            {
                MeshCollider meshCollider = objeto.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true; // MUY IMPORTANTE para Rigidbody
            }
        }

        // Recorremos todos los hijos, nietos, etc.
        foreach (Transform hijo in objeto)
        {
            AgregarCollidersRecursivo(hijo);
        }
    }
}
