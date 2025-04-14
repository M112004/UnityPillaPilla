using UnityEngine;

public class SetupColliders : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject aiEnemy;

    void Start()
    {
        SetupPlayerCollider();
        SetupAICollider();
    }

    private void SetupPlayerCollider()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
            return;
        }

        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null)
        {
            playerCollider = player.GetComponentInChildren<Collider>();
        }

        if (playerCollider != null)
        {
            // Make sure the player has the correct tag
            player.tag = "Player";

            // Set up player collider for proper collision detection
            playerCollider.isTrigger = false;

            Debug.Log($"Player collider set up: {playerCollider.name}");
        }
        else
        {
            Debug.LogError("No collider found on player or its children!");
        }
    }

    private void SetupAICollider()
    {
        if (aiEnemy == null)
        {
            Debug.LogError("AI Enemy reference is missing!");
            return;
        }

        Collider aiCollider = aiEnemy.GetComponent<Collider>();
        if (aiCollider == null)
        {
            aiCollider = aiEnemy.GetComponentInChildren<Collider>();
        }

        if (aiCollider != null)
        {
            // Set up AI collider for proper collision detection
            aiCollider.isTrigger = false;

            Debug.Log($"AI collider set up: {aiCollider.name}");
        }
        else
        {
            Debug.LogError("No collider found on AI Enemy or its children!");
        }
    }
}