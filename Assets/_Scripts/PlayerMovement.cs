using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float catchDistance = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject catchEffectPrefab;

    private Rigidbody rb;
    private float currentSpeed;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        // Skip if game is paused
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        HandleInput();
        HandleCatchingEnemies();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void HandleInput()
    {
        // Get input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Transform standard input to isometric direction
        Vector3 right = new Vector3(-1f, 0f, 1f).normalized; // Changed to fix left/right
        Vector3 forward = new Vector3(-1f, 0f, -1f).normalized;

        // Calculate movement based on isometric axes
        moveDirection = (right * horizontalInput + forward * verticalInput).normalized;

        // Handle sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }

    private void Move()
    {
        if (moveDirection.magnitude >= 0.1f)
        {
            // Apply movement force
            rb.velocity = moveDirection * currentSpeed;

            // Optionally rotate to face movement direction
            if (rotationSpeed > 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Stop movement
            rb.velocity = Vector3.zero;
        }
    }

    private void HandleCatchingEnemies()
    {
        if (Input.GetButtonDown("Fire1")) // Left mouse click or similar
        {
            // Check for enemies within catch distance
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, catchDistance, enemyLayer);

            foreach (var hitCollider in hitColliders)
            {
                EnemyAIStateMotor enemyMotor = hitCollider.GetComponent<EnemyAIStateMotor>();
                if (enemyMotor != null && enemyMotor.stateEnum == AIState.Flee)
                {
                    // Notify game manager (if it exists)
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EnemyCaught(hitCollider.gameObject);
                    }

                    // Show catch effect (optional)
                    if (catchEffectPrefab != null)
                    {
                        Instantiate(catchEffectPrefab, hitCollider.transform.position, Quaternion.identity);
                    }

                    break; // Only catch one enemy at a time
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if caught by enemy
        EnemyAIStateMotor enemyMotor = collision.gameObject.GetComponent<EnemyAIStateMotor>();
        if (enemyMotor != null && enemyMotor.stateEnum == AIState.Seek)
        {
            // Notify game manager (if it exists)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerCaught();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize catch range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}