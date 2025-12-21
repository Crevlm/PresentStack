using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;

    private int highScore;

    [Header("Game References")]
    [SerializeField] private PickupController pickupController;
    [SerializeField] private GameTimer gameTimer;

    public enum GameState
    {
        WaitingToStart,
        Countdown,
        Playing,
        GameOver
    }

    public GameState currentState { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentState = GameState.WaitingToStart;

        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        // Disable pickup until game starts
        if (pickupController != null)
            pickupController.enabled = false;
    }

    public void OnStartButtonPressed()
    {
        if (currentState == GameState.WaitingToStart)
        {
            StartCoroutine(CountdownSequence());
        }
    }

    IEnumerator CountdownSequence()
    {
        currentState = GameState.Countdown;

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        // 3
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        // 2
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        // 1
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        // GO!
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);

        // Start the game
        StartGame();
    }

    void StartGame()
    {
        currentState = GameState.Playing;

        // Enable pickup
        if (pickupController != null)
            pickupController.enabled = true;

        // Start the timer
        if (gameTimer != null)
            gameTimer.StartTimer();
    }

    public void EndGame(int finalScore)
    {
        currentState = GameState.GameOver;

        // Disable pickup
        if (pickupController != null)
            pickupController.enabled = false;

        // Check and save high score
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        // Show end game panel
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score: {finalScore}";

            if (highScoreText != null)
                highScoreText.text = $"High Score: {highScore}";
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}