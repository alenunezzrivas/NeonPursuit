using UnityEngine;
using Fusion;
using Invector.vCharacterController;

public class PlayerMovement : NetworkBehaviour
{
    private vThirdPersonController controller;
    private Transform cam;

    public float rotationSpeed = 10f;

    public override void Spawned()
    {
        controller = GetComponent<vThirdPersonController>();

        if (Object.HasInputAuthority)
        {
            cam = Camera.main.transform;
        }
        else
        {
            enabled = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out PlayerInputData data)) return;
        if (controller == null) return;

        // 🔥 Movimiento
        Vector3 inputDirection = new Vector3(data.horizontal, 0, data.vertical);
        controller.input = inputDirection;
        controller.UpdateMoveDirection();

        // 🔥 Sprint
        controller.Sprint(data.sprint);

        // 🔥 Jump
        if (data.jump)
        {
            controller.Jump();
        }

        // 🔥 ROTACIÓN CON CÁMARA (CLAVE)
        if (cam != null)
        {
            Vector3 forward = cam.forward;
            forward.y = 0;

            if (forward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
            }
        }

        // 🔥 Invector core
        controller.ControlLocomotionType();
        controller.ControlRotationType();
    }
}