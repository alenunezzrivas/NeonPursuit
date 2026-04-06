using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerSimple : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -40f;
    public float jumpForce = 7f;

    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        float moveAmount = move.magnitude * speed;

        // ANIMACIÓN
        animator.SetFloat("speed", moveAmount);

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -10f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
                animator.SetTrigger("jump");
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (move != Vector3.zero)
            transform.forward = move;
    }
}