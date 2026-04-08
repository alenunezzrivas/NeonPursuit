using UnityEngine;
using Fusion;

[RequireComponent(typeof(CharacterController))]
public class InvectorAnimatorBridge : NetworkBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private PlayerRole role;

    private float yVelocity;
    public float gravity = -20f;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        role = GetComponent<PlayerRole>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * v + transform.right * h;

        float speed = role.GetSpeed();

        // GRAVEDAD
        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        // SALTO
        if (Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {
            yVelocity = Mathf.Sqrt(role.GetJumpForce() * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        yVelocity += gravity * Runner.DeltaTime;

        Vector3 velocity = move * speed;
        velocity.y = yVelocity;

        controller.Move(velocity * Runner.DeltaTime);

        // ROTACIÓN
        if (move != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(
                transform.forward,
                move,
                10f * Runner.DeltaTime
            );
        }

        // ANIMACIONES
        float animSpeed = new Vector2(h, v).magnitude;

        animator.SetFloat("InputHorizontal", h);
        animator.SetFloat("InputVertical", v);
        animator.SetFloat("InputMagnitude", animSpeed);
        animator.SetBool("IsGrounded", controller.isGrounded);
    }
}