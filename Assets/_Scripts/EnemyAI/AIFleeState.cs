using UnityEngine;

public class AIFleeState : BaseState
{
    [SerializeField] private float fleeMaxSpeed = 4f;
    [SerializeField] private float steeringSpeed = 2f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float minDistanceFromPlayer = 5f;

    private Vector3 fleeDirection;
    private Vector3 randomDirection;
    private float directionChangeTimer;

    public override void Construct()
    {
        Debug.Log($"{gameObject.name} is entering Flee State");
        randomDirection = GetRandomDirection();
        directionChangeTimer = Random.Range(1f, 3f);
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Seek)
            m_enemyAIStateMotor.ChangeState(GetComponent<AISeekState>());
    }

    public override void UpdateState()
    {
        // If no target, just wander randomly
        if (m_enemyAIStateMotor.target == null)
        {
            WanderRandomly();
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, m_enemyAIStateMotor.target.position);

        if (distanceToPlayer < detectionRadius)
        {
            // Calculate direction away from player
            fleeDirection = (transform.position - m_enemyAIStateMotor.target.position).normalized;

            // Look in the direction of movement
            if (fleeDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(fleeDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, steeringSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Player is far away, wander randomly
            WanderRandomly();
        }
    }

    public override void FixedUpdateState()
    {
        Vector3 moveDirection;

        // Determine movement direction based on player distance
        if (m_enemyAIStateMotor.target != null &&
            Vector3.Distance(transform.position, m_enemyAIStateMotor.target.position) < detectionRadius)
        {
            moveDirection = fleeDirection;
        }
        else
        {
            moveDirection = randomDirection;
        }

        // Apply movement
        if (m_enemyAIStateMotor.rb != null)
        {
            m_enemyAIStateMotor.rb.velocity = moveDirection * fleeMaxSpeed;
        }
        else
        {
            // Fallback if no rigidbody
            transform.position += moveDirection * fleeMaxSpeed * Time.fixedDeltaTime;
        }
    }

    private void WanderRandomly()
    {
        // Change direction periodically
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            randomDirection = GetRandomDirection();
            directionChangeTimer = Random.Range(1f, 3f);
        }

        // Look in the random direction
        if (randomDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, steeringSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetRandomDirection()
    {
        // Generate random direction in the XZ plane
        float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
    }

    public override void Destruct()
    {
        Debug.Log($"{gameObject.name} is exiting Flee State");
    }
}