using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float walkSpeed = 1.5f;

    [Header("Timing")]
    [SerializeField] private Vector2 walkTimeRange = new Vector2(3f, 6f);
    [SerializeField] private Vector2 waitTimeRange = new Vector2(2f, 4f);

    private NavMeshAgent agent;
    private Animator animator;

    private float walkTimer;
    private float waitTimer;

    private bool isWalking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = walkSpeed;
        agent.updateRotation = true;

        ResetWaitTimer();
    }

    void Update()
    {
        animator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);

        if (isWalking)
        {
            walkTimer -= Time.deltaTime;

            if (walkTimer <= 0f || agent.remainingDistance <= agent.stoppingDistance)
            {
                StopWalking();
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                StartWalking();
            }
        }
    }

    private void StartWalking()
    {
        Vector3 target = GetRandomNavMeshPosition(wanderRadius);

        agent.SetDestination(target);

        isWalking = true;
        walkTimer = Random.Range(walkTimeRange.x, walkTimeRange.y);
    }

    private void StopWalking()
    {
        isWalking = false;
        agent.ResetPath();
        ResetWaitTimer();
    }

    private void ResetWaitTimer()
    {
        waitTimer = Random.Range(waitTimeRange.x, waitTimeRange.y);
    }

    private Vector3 GetRandomNavMeshPosition(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
