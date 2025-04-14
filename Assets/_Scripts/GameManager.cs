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
        remainingTime = gameTime;
        gameRunning = true;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            winnerText.gameObject.SetActive(false);
        }

        if (aiEnemy != null)
        {
            aiStateMotor = aiEnemy.GetComponent<EnemyAIStateMotor>();

            if (aiStateMotor != null)
            {
                aiStateMotor.stateEnum = AIState.Seek;
            }
            else
            {
                Debug.LogError("EnemyAIStateMotor component not found on aiEnemy");
            }
        }
        else
        {
            Debug.LogError("AI Enemy reference is missing");
        }

        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!gameRunning) return;

        remainingTime -= Time.deltaTime;

        UpdateTimerDisplay();

        int timeToAnnounce = Mathf.FloorToInt(remainingTime);
        if (timeToAnnounce % 5 == 0 && timeToAnnounce > 0)
        {
            if (!announcedTime)
            {
                Debug.Log($"Time remaining: {timeToAnnounce}");
                announcedTime = true;
            }
        }
        else
        {
            announcedTime = false;
        }

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
            timerText.text = $"Tiempo: {minutes:00}:{seconds:00}";
        }
    }

    public void HandleTagCollision()
    {
        if (!gameRunning || aiStateMotor == null || collisionHandled) return;

        collisionHandled = true;

        if (aiStateMotor.stateEnum == AIState.Seek)
        {
            aiStateMotor.stateEnum = AIState.Flee;
            Debug.Log("Player pillado.");
        }
        else
        {
            aiStateMotor.stateEnum = AIState.Seek;
            Debug.Log("IA pillada.");
        }

        StartCoroutine(ResetCollisionFlag());
    }

    private IEnumerator ResetCollisionFlag()
    {
        yield return new WaitForSeconds(0.5f);
        collisionHandled = false;
    }

    private void EndGame()
    {
        gameRunning = false;

        FreezeAllMovement();

        string winnerMessage;
        if (aiStateMotor != null && aiStateMotor.stateEnum == AIState.Flee)
        {
            winnerMessage = "AI gana!";
            Debug.Log(winnerMessage);
        }
        else
        {
            winnerMessage = "Jugador gana!";
            Debug.Log(winnerMessage);
        }

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
        if (player != null)
        {
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
                playerRb.isKinematic = true; 
            }

            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }

        if (aiEnemy != null)
        {
            Rigidbody aiRb = aiEnemy.GetComponent<Rigidbody>();
            if (aiRb != null)
            {
                aiRb.velocity = Vector3.zero;
                aiRb.isKinematic = true; 
            }

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