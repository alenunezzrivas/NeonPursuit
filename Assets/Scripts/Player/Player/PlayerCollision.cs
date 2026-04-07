using UnityEngine;
using Fusion;

public class PlayerCollision : NetworkBehaviour
{
    private PlayerRole role;

    private void Start()
    {
        role = GetComponent<PlayerRole>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PlayerRole other = hit.gameObject.GetComponent<PlayerRole>();

        if (other == null) return;

        // Si yo soy hunter y toco runner → eliminar
        if (role.IsHunter() && !other.IsHunter())
        {
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(other.Object);
            }
        }
    }
}