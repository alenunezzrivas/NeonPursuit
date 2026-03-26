using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        Search
    }

    public State currentState;

    public Transform player;
    private NavMeshAgent agent;

    public Transform[] patrolPoints;
    private int patrolIndex;

    public float detectionRange = 10f;
    public float searchTime = 5f;

    private float searchTimer;

    private Vector3 lastKnownPlayerPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Patrol;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:

                Patrol();

                if (distance < detectionRange)
                {
                    currentState = State.Chase;
                }

                break;

            case State.Chase:

                Chase();

                if (distance > detectionRange)
                {
                    lastKnownPlayerPosition = player.position;
                    currentState = State.Search;
                    searchTimer = searchTime;

                    agent.SetDestination(lastKnownPlayerPosition);
                }

                break;

            case State.Search:

                Search();

                break;
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPatrolPoint();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);
    }

    void Search()
    {
        searchTimer -= Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (searchTimer <= 0)
            {
                currentState = State.Patrol;
                GoToNextPatrolPoint();
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);

        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}