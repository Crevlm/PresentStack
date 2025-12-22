using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private string gameSceneName = "GameScene";


    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        // Allows quit to work in the Editor for testing
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}
