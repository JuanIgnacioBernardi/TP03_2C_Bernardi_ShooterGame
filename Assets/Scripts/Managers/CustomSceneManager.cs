using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    private string _sceneMainMenu = "MainMenu";
    private string _sceneGameplay = "MainScene";

    private AsyncOperation _asyncLoad;
    public void Init()
    {
        
    }
    public void GoToMainMenu() => StartLoad(_sceneMainMenu);
    public void GoToGameplay() => StartLoad(_sceneGameplay);

    private void StartLoad(string sceneName)
    {
        _asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        _asyncLoad.allowSceneActivation = false;
    }

    // True when scene is ready
    public bool IsSceneReady() =>
        _asyncLoad != null && _asyncLoad.progress >= 0.9f;
    public void ActivateScene()
    {
        if (_asyncLoad != null)
            _asyncLoad.allowSceneActivation = true;
    }
}