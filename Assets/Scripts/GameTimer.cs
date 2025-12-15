using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float timeLeft = 12f;
    private bool gameRunning = true;

    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("Timer Text is not assigned!");
        }
        else
        {
            Debug.Log("Timer Text found!");
            timerText.text = "TEST";
        }
    }

    void Update()
    {
        if (timeLeft <= 0)
        {
            gameRunning = false;
            timerText.text = "0.0s";
            return;
        }

        timeLeft -= Time.deltaTime;
        timerText.text = $"{timeLeft:F1}s";
    }
}
