using UnityEngine;
using UnityEngine.InputSystem;

public class RoundSystem : MonoBehaviour
{
    public static RoundSystem Instance { get; private set; }

    [Header("Players")]
    [SerializeField]
    private PlayerController playerOneController;

    [SerializeField]
    private PlayerController playerTwoController;

    [Header("Player Reset Points")]
    [SerializeField]
    private Transform playerOneResetPoint;

    [SerializeField]
    private Transform playerTwoResetPoint;

    [Header("Ball")]
    [SerializeField]
    private BallController ballController;

    [SerializeField]
    private Rigidbody2D ballRb;

    [Header("Serve Points")]
    [SerializeField]
    private Transform playerOneServePoint;

    [SerializeField]
    private Transform playerTwoServePoint;

    [Header("Serve")]
    [SerializeField]
    private Player firstServer = Player.playerOne;

    private BubblePowerUpController bubblePowerUpController;
    private Player currentServer;
    private bool waitingForServe = false;
    private bool isPaused = false;

    public bool WaitingForServe
    {
        get { return waitingForServe; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        bubblePowerUpController = FindFirstObjectByType<BubblePowerUpController>();
    }

    private void Start()
    {
        currentServer = firstServer;
        ResetRound();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard.pKey.wasPressedThisFrame &&
            !waitingForServe)
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }

    public void EndRound(Player winner)
    {
        bool gameEnded = ScoreSystem.Instance.AddScore(winner);
        if (gameEnded)
            return;
        currentServer = winner;
        ResetRound();
    }

    private void ResetRound()
    {
        isPaused = false;

        if (bubblePowerUpController != null)
        {
            bubblePowerUpController.ResetPowerUp();
        }

        waitingForServe = true;
        playerOneController.ResetPlayer(playerOneResetPoint.position);
        playerTwoController.ResetPlayer(playerTwoResetPoint.position);
        ballController.ResetBall();
        if (currentServer == Player.playerOne)
        {
            ballRb.transform.position = playerOneServePoint.position;
            ballRb.position = playerOneServePoint.position;
        }
        else
        {
            ballRb.transform.position = playerTwoServePoint.position;
            ballRb.position = playerTwoServePoint.position;
        }
        ballRb.linearVelocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        Physics2D.SyncTransforms();
        Time.timeScale = 0f;
    }

    public bool TryResumeRound(Player player)
    {
        if (!waitingForServe)
            return false;

        if (player != currentServer)
            return false;

        waitingForServe = false;
        isPaused = false;
        Time.timeScale = 1f;

        return true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            isPaused = false;
            Time.timeScale = 1f;
        }
    }
}
