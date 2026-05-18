using Fusion;
using UnityEngine;

public enum Role
{
    Player,
    Hunter
}

public class PlayerRole : NetworkBehaviour
{
    [Networked] public Role role { get; set; }
    [Networked] public NetworkBool ready { get; set; }
    [Networked] public NetworkBool roleInitialized { get; set; }

    public GameObject smoke;

    public float runnerSpeed = 6f;
    public float hunterSpeed = 4f;

    public float baseJumpForce = 2.5f;
    public float hunterJumpMultiplier = 2f;

    private Role previousRole;
    private bool hasAnnounced;

    public override void Spawned()
    {
        ApplyRoleVisuals();
    }

    public override void Render()
    {
        if (!Object.HasInputAuthority) return;

        ApplyRoleVisuals();

        if (hasAnnounced) return;
        if (!roleInitialized) return;
        if (previousRole == role) return;

        previousRole = role;
        hasAnnounced = true;

        if (role == Role.Hunter)
            RoleAnnouncement.Show("HUNTER", "Catch your enemy");
        else
            RoleAnnouncement.Show("RUNNER", "Escape the hunter");
    }

    void ApplyRoleVisuals()
    {
        if (smoke != null)
            smoke.SetActive(role == Role.Hunter);
    }

    public void SetRole(Role newRole)
    {
        if (Object.HasStateAuthority)
        {
            role = newRole;
            roleInitialized = true;
        }
    }

    public bool IsHunter()
    {
        return role == Role.Hunter;
    }

    public float GetSpeed()
    {
        return IsHunter() ? hunterSpeed : runnerSpeed;
    }

    public float GetJumpForce()
    {
        return IsHunter()
            ? baseJumpForce * hunterJumpMultiplier
            : baseJumpForce;
    }
}
