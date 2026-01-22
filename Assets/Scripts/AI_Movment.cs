using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.2f;

    [Header("Timing")]
    [SerializeField] private Vector2 walkTimeRange = new Vector2(3f, 6f);
    [SerializeField] private Vector2 waitTimeRange = new Vector2(5f, 7f);

    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleCheckDistance = 1f;
    [SerializeField] private LayerMask obstacleLayers;

    private Animator animator;

    private float walkTimer;
    private float waitTimer;

    private bool isWalking;
    private Vector3 moveDirection;

    void Start()
    {
        animator = GetComponent<Animator>();

        ResetWalkTimer();
        ResetWaitTimer();
        ChooseDirection();
    }

    void Update()
    {
        if (isWalking)
            Walk();
        else
            Wait();
    }

    private void Walk()
    {
        animator.SetBool("isRunning", true);

        // Obstacle detection
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, moveDirection, obstacleCheckDistance, obstacleLayers))
        {
            StopWalking();
            ChooseDirection();
            return;
        }

        walkTimer -= Time.deltaTime;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (walkTimer <= 0f)
            StopWalking();
    }

    private void Wait()
    {
        animator.SetBool("isRunning", false);

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
            ChooseDirection();
    }

    private void StopWalking()
    {
        isWalking = false;
        ResetWaitTimer();
    }

    private void ChooseDirection()
    {
        isWalking = true;
        ResetWalkTimer();

        int randomDir = Random.Range(0, 4);

        switch (randomDir)
        {
            case 0: moveDirection = Vector3.forward; break;
            case 1: moveDirection = Vector3.right; break;
            case 2: moveDirection = Vector3.left; break;
            case 3: moveDirection = Vector3.back; break;
        }

        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    private void ResetWalkTimer()
    {
        walkTimer = Random.Range(walkTimeRange.x, walkTimeRange.y);
    }

    private void ResetWaitTimer()
    {
        waitTimer = Random.Range(waitTimeRange.x, waitTimeRange.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.2f, moveDirection * obstacleCheckDistance);
    }
}
