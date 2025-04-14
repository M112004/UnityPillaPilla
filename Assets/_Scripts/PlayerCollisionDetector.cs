using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerCollisionDetector : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we collided with an AI enemy
        EnemyAIStateMotor enemyAI = collision.gameObject.GetComponent<EnemyAIStateMotor>();
        if (enemyAI == null)
        {
            enemyAI = collision.gameObject.GetComponentInParent<EnemyAIStateMotor>();
        }

        if (enemyAI != null)
        {
            Debug.Log($"Player detected collision with AI: {collision.gameObject.name}");

            if (gameManager != null)
            {
                gameManager.HandleTagCollision();
            }
            else
            {
                Debug.LogError("GameManager reference is missing in PlayerCollisionDetector!");
            }
        }
    }
}