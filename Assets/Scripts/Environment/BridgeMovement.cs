using UnityEngine;

public class BridgeMovement : MonoBehaviour
{
    public Transform closedPoint;
    public Transform openPoint;

    public float speed = 2f;

    private bool opening = false;

    void Update()
    {
        Transform target = opening ? openPoint : closedPoint;

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }

    public void ToggleBridge()
    {
        opening = !opening;
    }
}