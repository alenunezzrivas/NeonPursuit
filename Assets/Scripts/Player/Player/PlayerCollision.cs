using UnityEngine;
using Fusion;

public class PlayerCollision : NetworkBehaviour
{
    private PlayerRole role;

    private void Start()
    {
        role = GetComponent<PlayerRole>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRole otherRole = other.GetComponent<PlayerRole>();

        if (otherRole == null) return;

        if (role.IsHunter() && !otherRole.IsHunter())
        {
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(otherRole.Object);
            }
        }
    }
}