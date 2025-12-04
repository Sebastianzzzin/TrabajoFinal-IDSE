using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCamera : MonoBehaviour
{
    public Transform jugador;

    [Header("Top Down")]
    public Vector3 offsetTopDown = new Vector3(0, 15f, -8f);
    public Vector3 rotacionTopDown = new Vector3(60f, 0f, 0f);

    public float alturaMin = 5f;
    public float alturaMax = 25f;
    public float verticalSpeed = 10f;
    public float suavizado = 8f;

    private Gamepad gamepad;

    public void ForceSetup()
    {
        transform.rotation = Quaternion.Euler(rotacionTopDown);
    }

    void Update()
    {
        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            float vertical = 0f;

            if (gamepad.leftShoulder.isPressed) vertical -= 1f;
            if (gamepad.rightShoulder.isPressed) vertical += 1f;

            if (vertical != 0f)
            {
                offsetTopDown.y += vertical * verticalSpeed * Time.deltaTime;
                offsetTopDown.y = Mathf.Clamp(offsetTopDown.y, alturaMin, alturaMax);
            }
        }
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 pos = new Vector3(
            jugador.position.x + offsetTopDown.x,
            offsetTopDown.y,
            jugador.position.z + offsetTopDown.z
        );

        transform.position = Vector3.Lerp(transform.position, pos, suavizado * Time.deltaTime);
    }
}
