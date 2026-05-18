using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InvectorAnimatorBridge : NetworkBehaviour
{
    [Header("Movement")]
    public float gravity = -30f;
    public float groundedForce = -5f;
    public float maxSlopeAngle = 50f;
    public float stepUpForce = 5f;

    [Header("Jump")]
    public float baseJumpForce = 2.5f;
    public float hunterJumpMultiplier = 2f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    [Header("Camera")]
    public Camera playerCamera;
    public float mouseSensitivity = 3f;
    public float mouseSmoothing = 6f;
    public float minPitch = -90f;
    public float maxPitch = 90f;

    [Networked] private float netSpeed { get; set; }
    [Networked] private float netHorizontal { get; set; }
    [Networked] private float netVertical { get; set; }
    [Networked] private NetworkBool netGrounded { get; set; }

    private CharacterController controller;
    private Animator animator;
    private PlayerRole role;

    private float yVelocity;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool wasJumpPressed;
    private bool prevGrounded;
    private bool jumpTriggered;

    private float yaw;
    private float pitch;
    private float targetYaw;
    private float targetPitch;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        role = GetComponent<PlayerRole>();

        if (!Object.HasInputAuthority) return;

        Debug.Log("Local player spawned");

        yaw = transform.eulerAngles.y;
        targetYaw = yaw;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerCamera != null)
            playerCamera.nearClipPlane = 0.01f;

        DisableSceneCamera();
        HideLocalPlayerModel();
    }

    void DisableSceneCamera()
    {
        var camGO = GameObject.Find("Camera");
        if (camGO != null)
        {
            Debug.Log("Disabling scene camera: " + camGO.name);
            camGO.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Scene camera 'Camera' not found to disable");
        }
    }

    void HideLocalPlayerModel()
    {
        var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers)
            r.gameObject.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerInputData input))
        {
            Move(input);

            if (Object.HasStateAuthority && role != null && input.ready && !role.ready)
                role.ready = true;
        }

        if (Object.HasInputAuthority)
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void Move(PlayerInputData input)
    {
        Vector3 move = transform.forward * input.move.y + transform.right * input.move.x;
        if (move.magnitude > 1f) move.Normalize();

        float speed = role != null ? role.GetSpeed() : 5f;

        if (controller.isGrounded)
        {
            if (yVelocity < 0) yVelocity = groundedForce;
            coyoteTimer = coyoteTime;

            if (move.magnitude > 0.01f)
            {
                speed = ProjectSpeedOnSlope(speed, move);
            }
        }
        else
        {
            coyoteTimer -= Runner.DeltaTime;
        }

        if (input.jump && !wasJumpPressed)
            jumpBufferTimer = jumpBufferTime;
        wasJumpPressed = input.jump;
        jumpBufferTimer -= Runner.DeltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            float jumpForce = role != null ? role.GetJumpForce() : baseJumpForce;
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            jumpTriggered = true;
        }

        yVelocity += gravity * Runner.DeltaTime;

        Vector3 velocity = move * speed;
        velocity.y = yVelocity;
        controller.Move(velocity * Runner.DeltaTime);

        if (move.magnitude > 0.01f && !Object.HasInputAuthority)
        {
            transform.forward = Vector3.Lerp(
                transform.forward,
                move,
                10f * Runner.DeltaTime
            );
        }

        netSpeed = new Vector2(input.move.x, input.move.y).magnitude;
        netHorizontal = input.move.x;
        netVertical = input.move.y;
        netGrounded = controller.isGrounded;
    }

    float ProjectSpeedOnSlope(float speed, Vector3 move)
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out var hit, 1.5f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > 0 && angle <= maxSlopeAngle)
            {
                Vector3 slopeDir = Vector3.ProjectOnPlane(move, hit.normal).normalized;
                if (slopeDir.magnitude > 0.01f)
                    return speed;
            }
        }
        return speed;
    }

    public override void Render()
    {
        if (!Object.HasInputAuthority)
        {
            if (animator != null)
            {
                animator.SetFloat("InputMagnitude", netSpeed);
                animator.SetFloat("InputHorizontal", netHorizontal);
                animator.SetFloat("InputVertical", netVertical);
                animator.SetBool("IsGrounded", netGrounded);
            }
            return;
        }

        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Debug.Log($"[DEBUG] Pos: {transform.position}, Grounded: {controller.isGrounded}, " +
                      $"Role: {role?.role}, Speed: {(role != null ? role.GetSpeed() : 5f)}, " +
                      $"Y: {yVelocity:F2}, Cam: {(playerCamera != null && playerCamera.gameObject.activeInHierarchy)}");
        }

        HandleCamera();

        if (animator != null)
        {
            animator.SetFloat("InputMagnitude", netSpeed);
            animator.SetFloat("InputHorizontal", netHorizontal);
            animator.SetFloat("InputVertical", netVertical);
            animator.SetBool("IsGrounded", controller.isGrounded);

            if (!prevGrounded && controller.isGrounded)
                animator.SetTrigger("Land");
            prevGrounded = controller.isGrounded;

            if (jumpTriggered)
            {
                animator.SetTrigger("Jump");
                jumpTriggered = false;
            }
        }
    }

    void HandleCamera()
    {
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return;

        var mouseDelta = mouse.delta.ReadValue();
        targetYaw += mouseDelta.x * mouseSensitivity * 0.1f;
        targetPitch -= mouseDelta.y * mouseSensitivity * 0.1f;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        yaw = Mathf.Lerp(yaw, targetYaw, Time.deltaTime * mouseSmoothing);
        pitch = Mathf.Lerp(pitch, targetPitch, Time.deltaTime * mouseSmoothing);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
