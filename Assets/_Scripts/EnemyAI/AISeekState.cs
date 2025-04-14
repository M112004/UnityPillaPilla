using UnityEngine;

public class AISeekState : BaseState
{
    [SerializeField] private float seekMaxSpeed = 5.5f;
    [SerializeField] private float steeringSpeed = 3f;
    [SerializeField] private float detectionRadius = 15f;

    private Vector3 seekDirection;
    private Vector3 randomDirection;
    private float directionChangeTimer;

    public override void Construct()
    {
        Debug.Log($"{gameObject.name} is entering Seek State");
        randomDirection = GetRandomDirection();
        directionChangeTimer = Random.Range(1f, 3f);
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Flee)
            m_enemyAIStateMotor.ChangeState(GetComponent<AIFleeState>());
    }

    public override void UpdateState()
    {
        // If no target, just wander randomly
        if (m_enemyAIStateMotor.target == null)
        {
            WanderRandomly();
            return;
        }

        // Calculate direction to player
        seekDirection = (m_enemyAIStateMotor.target.position - transform.position).normalized;

        // Look in the direction of movement
        if (seekDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(seekDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, steeringSpeed * Time.deltaTime);
        }
    }

    public override void FixedUpdateState()
    {
        Vector3 moveDirection;

        // Determine movement direction based on player existence
        if (m_enemyAIStateMotor.target != null)
        {
            moveDirection = seekDirection;
        }
        else
        {
            moveDirection = randomDirection;
        }

        // Apply movement
        if (m_enemyAIStateMotor.rb != null)
        {
            m_enemyAIStateMotor.rb.velocity = moveDirection * seekMaxSpeed;
        }
        else
        {
            // Fallback if no rigidbody
            transform.position += moveDirection * seekMaxSpeed * Time.fixedDeltaTime;
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
        Debug.Log($"{gameObject.name} is exiting Seek State");
    }
}