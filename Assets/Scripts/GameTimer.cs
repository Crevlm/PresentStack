using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI scoreText;
    public Button startButton;

    private float timeLeft = 12f;
    private bool gameRunning = false;
    private float highestPoint = 0f;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartTimer);
        }
    }


    public void StartTimer()
    {
        gameRunning = true;
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!gameRunning) return;


        if (timeLeft <= 0)
        {
            gameRunning = false;
            timerText.text = "0.0s";
            ShowFinalScore();
            return;
        }

        timeLeft -= Time.deltaTime;
        timerText.text = $"{timeLeft:F1}s";

        if (gameRunning)
        {
            UpdateHeight();
        }

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

        highestPoint = Mathf.Max(highestPoint, maxHeight);
        heightText.text = $"Height: {highestPoint:F1}m";
    }

    void ShowFinalScore()
    {
        int finalScore = Mathf.RoundToInt(highestPoint * 100);
        scoreText.text = $"Score: {finalScore}";
    }

}
