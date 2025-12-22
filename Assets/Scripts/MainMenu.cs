using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;
using FMOD;
using System;
public class MainMenu : MonoBehaviour
{

    [SerializeField] private string gameSceneName = "GameScene";

    private const string ConfirmEventPath = "event:/SFX/UI_Confirm";
    private const string ReturnEventPath = "event:/SFX/UI_Select";

  

    public void StartGame()
    {
        RuntimeManager.PlayOneShot(ConfirmEventPath);
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        RuntimeManager.PlayOneShot(ReturnEventPath);
        Application.Quit();
#if UNITY_EDITOR
        // Allows quit to work in the Editor for testing
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}
