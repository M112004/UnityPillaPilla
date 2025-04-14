using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Added TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float initialFleeTime = 30f;
    [SerializeField] private float comboTimeWindow = 5f;
    [SerializeField] private int pointsPerEnemyCaught = 100;
    [SerializeField] private int pointsPerSurvivalInterval = 100;
    [SerializeField] private float survivalPointsInterval = 5f;

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float spawnAreaWidth = 10f;
    [SerializeField] private float spawnAreaHeight = 10f;
    [SerializeField] private float minDistanceFromPlayer = 5f;

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText; // Changed to TMP_Text
    [SerializeField] private TMP_Text phaseText; // Changed to TMP_Text
    [SerializeField] private TMP_Text timerText; // Changed to TMP_Text
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText; // Changed to TMP_Text

    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool isGameOver = false;

    private int currentScore = 0;
    private int comboCount = 0;
    private float comboTimer = 0f;
    private float gameTimer = 0f;
    private float survivalPointsTimer = 0f;
    private bool isFleePhase = true;
    private List<GameObject> enemies = new List<GameObject>();
    private Transform playerTransform;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find player in scene
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Hide game over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        // Spawn initial enemies
        SpawnEnemies();

        // Start the game in flee phase
        StartCoroutine(InitialFleePhase());

        // Initialize UI
        UpdateUI();
    }

    private void Update()
    {
        if (isPaused || isGameOver)
            return;

        // Update timers
        gameTimer += Time.deltaTime;

        // Handle combo timer
        if (isFleePhase && comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboCount = 0;
            }
        }

        // Handle survival points in seek phase
        if (!isFleePhase)
        {
            survivalPointsTimer += Time.deltaTime;
            if (survivalPointsTimer >= survivalPointsInterval)
            {
                AddScore(pointsPerSurvivalInterval);
                survivalPointsTimer = 0f;
            }
        }

        // Update UI
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update score
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;

        // Update phase text
        if (phaseText != null)
            phaseText.text = isFleePhase ? "CHASE THEM!" : "RUN AWAY!";

        // Update timer
        if (timerText != null)
        {
            if (isFleePhase)
            {
                int timeLeft = Mathf.CeilToInt(initialFleeTime - gameTimer);
                timerText.text = "Phase ends in: " + timeLeft + "s";
            }
            else
            {
                int survivalTime = Mathf.FloorToInt(gameTimer - initialFleeTime);
                timerText.text = "Survived: " + survivalTime + "s";
            }
        }
    }

    private IEnumerator InitialFleePhase()
    {
        isFleePhase = true;
        SetAllEnemiesState(AIState.Flee);

        yield return new WaitForSeconds(initialFleeTime);

        isFleePhase = false;
        SetAllEnemiesState(AIState.Seek);
    }

    private void SpawnEnemies()
    {
        // Clear any existing enemies
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        enemies.Clear();

        // Spawn new enemies
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || playerTransform == null)
            return;

        // Generate random position
        Vector3 spawnPosition;
        int attempts = 0;

        do
        {
            float x = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2);
            float z = Random.Range(-spawnAreaHeight / 2, spawnAreaHeight / 2);
            float y = 3f; // Altura desde donde caerán los enemigos
            spawnPosition = new Vector3(x, y, z);
            attempts++;
        }
        while (Vector3.Distance(new Vector3(spawnPosition.x, 0, spawnPosition.z), playerTransform.position) < minDistanceFromPlayer && attempts < 30);

        // Instantiate enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Buscar el componente en el objeto hijo
        EnemyAIStateMotor motor = enemy.GetComponentInChildren<EnemyAIStateMotor>();

        if (motor == null)
        {
            Debug.LogError("EnemyAIStateMotor component not found on enemy or its children!");
            Destroy(enemy);
            return;
        }

        // Asegúrate de que el Rigidbody use gravedad para la caída
        Rigidbody rb = motor.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;

            // Una vez que toque el suelo, desactivaremos la gravedad
            StartCoroutine(DisableGravityAfterLanding(rb));
        }

        // Set target to player
        motor.target = playerTransform;
        motor.stateEnum = isFleePhase ? AIState.Flee : AIState.Seek;

        // Update state accordingly
        if (isFleePhase)
        {
            AIFleeState fleeState = motor.GetComponent<AIFleeState>();
            if (fleeState != null)
            {
                motor.ChangeState(fleeState);
            }
        }
        else
        {
            AISeekState seekState = motor.GetComponent<AISeekState>();
            if (seekState != null)
            {
                motor.ChangeState(seekState);
            }
        }

        enemies.Add(enemy);
    }

    // Corrutina para desactivar la gravedad después de que el enemigo toque el suelo
    private IEnumerator DisableGravityAfterLanding(Rigidbody rb)
    {
        if (rb == null) yield break;

        // Espera a que la posición Y esté cerca de 0 (o el nivel del suelo)
        float groundLevel = 0f; // Ajusta esto según la altura real de tu plano
        float threshold = 0.1f;

        while (rb != null && Mathf.Abs(rb.transform.position.y - groundLevel) > threshold)
        {
            yield return null;
        }

        // Una vez en el suelo, desactivamos la gravedad y fijamos restricciones
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Elimina movimiento vertical
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            // Asegúrate de que la posición Y sea exactamente la del suelo
            Vector3 pos = rb.transform.position;
            rb.transform.position = new Vector3(pos.x, groundLevel, pos.z);
        }
    }

    private void SetAllEnemiesState(AIState state)
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            EnemyAIStateMotor motor = enemy.GetComponent<EnemyAIStateMotor>();
            if (motor != null)
            {
                motor.stateEnum = state;

                if (state == AIState.Flee)
                {
                    motor.ChangeState(enemy.GetComponent<AIFleeState>());
                }
                else
                {
                    motor.ChangeState(enemy.GetComponent<AISeekState>());
                }
            }
        }
    }

    public void EnemyCaught(GameObject enemy)
    {
        if (!isFleePhase) return;

        // Remove enemy from list
        enemies.Remove(enemy);

        // Handle combo
        comboTimer = comboTimeWindow;
        comboCount++;

        // Calculate score
        int points = pointsPerEnemyCaught * comboCount;
        AddScore(points);

        // Display floating score text (optional implementation)
        Debug.Log($"Caught enemy! +{points} points (x{comboCount} combo)");

        // Destroy enemy
        Destroy(enemy);

        // Spawn new enemy
        SpawnEnemy();
    }

    public void PlayerCaught()
    {
        if (isFleePhase) return;

        isGameOver = true;

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + currentScore;
        }

        Debug.Log("Game Over! Final score: " + currentScore);
    }

    private void AddScore(int points)
    {
        currentScore += points;
    }

    public void RestartGame()
    {
        // Reset game state
        currentScore = 0;
        comboCount = 0;
        comboTimer = 0f;
        gameTimer = 0f;
        survivalPointsTimer = 0f;
        isFleePhase = true;
        isGameOver = false;

        // Hide game over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Respawn enemies
        SpawnEnemies();

        // Start flee phase
        StartCoroutine(InitialFleePhase());

        // Update UI
        UpdateUI();
    }
}