using UnityEngine;
using Fusion;

public class NetworkAnimatorSync : NetworkBehaviour
{
    private Animator animator;

    [Networked] private float speed { get; set; }
    [Networked] private bool isJumping { get; set; }
    [Networked] private bool isSprinting { get; set; }

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            speed = animator.GetFloat("InputMagnitude");
            isJumping = animator.GetBool("IsJumping");
            isSprinting = animator.GetBool("IsSprinting");
        }
        else
        {
            animator.SetFloat("InputMagnitude", speed);
            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsSprinting", isSprinting);
        }
    }
}