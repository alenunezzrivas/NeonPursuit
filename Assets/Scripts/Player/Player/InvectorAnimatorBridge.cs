using Fusion;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InvectorAnimatorBridge : NetworkBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private PlayerRole role;

    private float yVelocity;

    public float gravity = -30f;
    public float groundedForce = -5f;

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

        // SUELO
        if (controller.isGrounded && yVelocity < 0)
            yVelocity = groundedForce;

        // SALTO AJUSTADO
        if (Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {
            float jumpForce = role.GetJumpForce();
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);

            animator.SetTrigger("Jump");
        }

        // GRAVEDAD
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