using UnityEngine;

public class SecurityController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 120f;

    public float jumpForce = 2f;
    public float gravity = -20f;

    private Animator animator;
    private CharacterController controller;
    private Vector3 velocity;

    // 🔥 MEJORAS
    private float coyoteTime = 0.15f;
    private float coyoteTimer;

    private float jumpBufferTime = 0.15f;
    private float jumpBufferTimer;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 🎮 ROTACIÓN Q/E
        float rotation = 0f;
        if (Input.GetKey(KeyCode.Q)) rotation = -1f;
        if (Input.GetKey(KeyCode.E)) rotation = 1f;

        transform.Rotate(Vector3.up * rotation * rotationSpeed * Time.deltaTime);

        // 🧠 MOVIMIENTO RELATIVO
        Vector3 move = transform.forward * v + transform.right * h;

        // 📊 SPEED PARA ANIMATOR
        float currentSpeed = move.magnitude;
        animator.SetFloat("Speed", currentSpeed);

        // 🟢 GROUND CHECK MEJORADO
        if (controller.isGrounded)
        {
            coyoteTimer = coyoteTime;
            if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        animator.SetBool("IsGrounded", controller.isGrounded);

        // 🟡 BUFFER DE SALTO
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // 🦘 SALTO MEJORADO
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("Jump");

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // 🌍 GRAVEDAD
        velocity.y += gravity * Time.deltaTime;

        // 🚶 MOVIMIENTO FINAL
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
}