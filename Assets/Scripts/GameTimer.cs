using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI scoreText;
    public Button startButton;

    public float defaultTime = 12f;

    private float timeLeft = 12f;
    private bool gameRunning = false;
    private float highestPoint = 0f;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    void OnStartButtonClicked()
    {
        // Tell GameManager to start countdown
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartButtonPressed();
        }

        // Hide start button
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
    }

    public void StartTimer()
    {
        gameRunning = true;
    }

    public void ResetTimer()
    {
        timeLeft = defaultTime;
        timerText.text = $"Timer:{timeLeft:F1}s";
    }

    void Update()
    {
        if (!gameRunning) return;

        if (timeLeft <= 0)
        {
            gameRunning = false;
            timerText.text = "Timer: 00.00s";

            int finalScore = Mathf.RoundToInt(highestPoint * 100);

            // Tell GameManager game is over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame(finalScore);
            }

            return;
        }

        timeLeft -= Time.deltaTime;
        timerText.text = $"Timer:{timeLeft:F1}s";
        UpdateHeight();
    }

    void UpdateHeight()
    {
        GameObject[] presents = GameObject.FindGameObjectsWithTag("Present");
        float maxHeight = 0f;

        foreach (GameObject present in presents)
        {
            if (present.transform.position.y > maxHeight)
            {
                maxHeight = present.transform.position.y;
            }
        }

        highestPoint = maxHeight; // Just take current height, not the max over time
        heightText.text = $"Height: {highestPoint:F1}m";
    }
}