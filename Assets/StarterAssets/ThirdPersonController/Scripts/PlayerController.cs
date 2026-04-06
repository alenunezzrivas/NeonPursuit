using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [Header("Cámara")]
    public Transform cameraPivot;
    public float mouseSensitivity = 100f;

    [Header("Animator")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canJump;

    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        Move();
        ApplyGravity();
        HandleJump();
        UpdateAnimator();
    }

    void Look()
    {
        if (cameraPivot == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 100;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * 100;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (groundCheck == null) return;

        // 🔥 detección de suelo estable
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // evita flotar y parpadeos en escaleras
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded)
        {
            canJump = true;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canJump = false;
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speedValue = horizontalVelocity.magnitude;

        // 🔥 parámetros correctos para Starter Assets
        animator.SetFloat("Speed", speedValue);
        animator.SetFloat("MotionSpeed", speedValue);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("FreeFall", !isGrounded);
    }

    // 🔥 evita error de AnimationEvent
    public void OnLand()
    {
    }
}