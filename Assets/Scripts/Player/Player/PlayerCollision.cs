using UnityEngine;
using Fusion;

public class PlayerCollision : NetworkBehaviour
{
    private PlayerRole role;

    private void Start()
    {
        role = GetComponent<PlayerRole>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!Object.HasStateAuthority) return;
        if (role == null) return;
        if (!role.IsHunter()) return;

        PlayerRole otherRole = hit.gameObject.GetComponent<PlayerRole>();
        if (otherRole == null) return;
        if (otherRole.IsHunter()) return;

        Runner.Despawn(otherRole.Object);
    }
}