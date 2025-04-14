using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Added for TextMeshPro support

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameTime = 60f; // Total game time in seconds
    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private Transform aiStartPosition;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the TMP text for timer
    [SerializeField] private TextMeshProUGUI gameOverText; // Reference to the TMP text for game over message
    [SerializeField] private TextMeshProUGUI winnerText;

    [Header("Object References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject aiEnemy;

    private EnemyAIStateMotor aiStateMotor;
    private float remainingTime;
    private bool gameRunning = false;
    private bool announcedTime = false;
    private bool collisionHandled = false;

    void Start()
    {
        // Initialize the game
        remainingTime = gameTime;
        gameRunning = true;

        // Hide game over text at the start
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            winnerText.gameObject.SetActive(false);
        }

        // Get the EnemyAIStateMotor component from aiEnemy
        if (aiEnemy != null)
        {
            aiStateMotor = aiEnemy.GetComponent<EnemyAIStateMotor>();

            if (aiStateMotor != null)
            {
                aiStateMotor.stateEnum = AIState.Seek;
                Debug.Log("Game started! AI is seeking the player.");
            }
            else
            {
                Debug.LogError("EnemyAIStateMotor component not found on aiEnemy!");
            }
        }
        else
        {
            Debug.LogError("AI Enemy reference is missing!");
        }

        // Initialize the timer display
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!gameRunning) return;

        // Update game timer
        remainingTime -= Time.deltaTime;

        // Update the timer display
        UpdateTimerDisplay();

        // Check if we need to announce the time
        int timeToAnnounce = Mathf.FloorToInt(remainingTime);
        if (timeToAnnounce % 5 == 0 && timeToAnnounce > 0)
        {
            if (!announcedTime)
            {
                Debug.Log($"Time remaining: {timeToAnnounce} seconds");
                announcedTime = true;
            }
        }
        else
        {
            announcedTime = false;
        }

        // Check if game is over
        if (remainingTime <= 0)
        {
            EndGame();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    // Call this when the player and AI collide
    public void HandleTagCollision()
    {
        if (!gameRunning || aiStateMotor == null || collisionHandled) return;

        // Set flag to prevent multiple collision handling
        collisionHandled = true;

        // Switch roles
        if (aiStateMotor.stateEnum == AIState.Seek)
        {
            aiStateMotor.stateEnum = AIState.Flee;
            Debug.Log("Player was tagged! AI is now fleeing.");
        }
        else
        {
            aiStateMotor.stateEnum = AIState.Seek;
            Debug.Log("AI was tagged! AI is now seeking.");
        }

        // Reset the collision handled flag after a short delay
        // (to prevent immediate re-collision detection)
        StartCoroutine(ResetCollisionFlag());
    }

    private IEnumerator ResetCollisionFlag()
    {
        // Wait a short time to prevent immediate collision detection
        yield return new WaitForSeconds(0.5f);
        collisionHandled = false;
    }

    private void EndGame()
    {
        gameRunning = false;

        // Stop all movement
        FreezeAllMovement();

        // Determine the winner based on AI state
        string winnerMessage;
        if (aiStateMotor != null && aiStateMotor.stateEnum == AIState.Flee)
        {
            winnerMessage = "AI wins!";
            Debug.Log(winnerMessage);
        }
        else
        {
            winnerMessage = "Player wins!";
            Debug.Log(winnerMessage);
        }

        // Display game over message on UI
        if (gameOverText != null)
        {
            timerText.gameObject.SetActive(false);
            gameOverText.gameObject.SetActive(true);
            winnerText.gameObject.SetActive(true);
            winnerText.text = winnerMessage;
        }
    }

    private void FreezeAllMovement()
    {
        // Freeze player movement
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
                playerRb.isKinematic = true; // Prevent physics from affecting the player
            }

            // Disable player's movement script
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }

        // Freeze AI movement
        if (aiEnemy != null)
        {
            Rigidbody aiRb = aiEnemy.GetComponent<Rigidbody>();
            if (aiRb != null)
            {
                aiRb.velocity = Vector3.zero;
                aiRb.isKinematic = true; // Prevent physics from affecting the AI
            }

            // Disable AI's behavior components
            AIBehaviour aiBehavior = aiEnemy.GetComponent<AIBehaviour>();
            if (aiBehavior != null)
            {
                aiBehavior.enabled = false;
            }

            if (aiStateMotor != null)
            {
                aiStateMotor.enabled = false;
            }
        }
    }
}