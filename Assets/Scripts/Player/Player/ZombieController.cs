using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private CharacterController controller;

    private float currentSpeed;
    private bool isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // 🚫 NO moverse mientras ataca
        if (!isAttacking)
        {
            if (move > 0)
            {
                currentSpeed = isRunning ? runSpeed : walkSpeed;

                Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
                controller.Move(movement);

                float animSpeed = isRunning ? 3f : 1.5f;
                animator.SetFloat("Speed", animSpeed);
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }

            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(0, turn * rotationSpeed * 100f * Time.deltaTime, 0);
        }

    }

}