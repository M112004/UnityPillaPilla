using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerCollisionDetector : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void OnCollisionEnter(Collision collision)
    {
        EnemyAIStateMotor enemyAI = collision.gameObject.GetComponent<EnemyAIStateMotor>();
        if (enemyAI == null)
        {
            enemyAI = collision.gameObject.GetComponentInParent<EnemyAIStateMotor>();
        }

        if (enemyAI != null)
        {

            if (gameManager != null)
            {
                gameManager.HandleTagCollision();
            }
            else
            {
                Debug.LogError("GameManager reference missing");
            }
        }
    }
}