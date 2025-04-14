using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagCollisionHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string targetTag = "Player"; // The tag of the object we want to detect collisions with

    // Try both trigger and collision methods to ensure we catch the interaction
    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision.collider);
    }

    private void CheckCollision(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log($"Tag collision detected between {gameObject.name} and {other.gameObject.name}");

            // Notify the game manager that a tag occurred
            if (gameManager != null)
            {
                gameManager.HandleTagCollision();
            }
            else
            {
                Debug.LogError("GameManager reference is missing in TagCollisionHandler!");
            }
        }
    }
}