using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f;

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        // mover hacia delante/atrás
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);

        // rotar suavemente
        transform.Rotate(Vector3.up * turn * rotationSpeed * Time.deltaTime);
    }
}