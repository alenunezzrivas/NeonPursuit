using UnityEngine;
using Fusion;

public enum Role
{
    Player,
    Hunter
}

public class PlayerRole : NetworkBehaviour
{
    [Networked] public Role role { get; set; }

    public GameObject smoke;

    public float runnerSpeed = 6f;
    public float hunterSpeed = 4f;
    public float hunterJumpBoost = 1.5f;

    public override void Spawned()
    {
        ApplyRole();
    }

    public override void Render()
    {
        ApplyRole();
    }

    void ApplyRole()
    {
        if (smoke != null)
            smoke.SetActive(role == Role.Hunter);
    }

    public void SetRole(Role newRole)
    {
        if (Object.HasStateAuthority)
            role = newRole;
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
        return IsHunter() ? hunterJumpBoost * 5f : 5f;
    }
}