using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagCollisionHandler : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string targetTag = "Player"; 

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