using Fusion;
using System;
using UnityEngine;

public class BridgeMovement : NetworkBehaviour
{
    public Transform closedPoint;
    public Transform openPoint;

    public float speed = 2f;

    [Networked] private bool opening { get; set; }

    public override void FixedUpdateNetwork()
    {
        Transform target = opening ? openPoint : closedPoint;

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Runner.DeltaTime
            );
        }
    }

    public void ToggleBridge()
    {
        if (Object.HasStateAuthority)
        {
            opening = !opening;
        }
    }
}