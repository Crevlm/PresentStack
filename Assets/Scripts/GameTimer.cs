using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI scoreText;

    private float timeLeft = 12f;
    private bool gameRunning = true;
    private float highestPoint = 0f;

   

    void Update()
    {
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
