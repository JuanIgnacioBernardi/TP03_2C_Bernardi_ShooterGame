using System.Collections;
using UnityEngine;
public class SplashScreenController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float displayTime = 3f;
    private void Start()
    {
        StartCoroutine(SplashRoutine());
    }
    private IEnumerator SplashRoutine()
    {
        // Wait for the Bootstrapper to finish initializing
        yield return null;

        // Display the splash screen for the configured time
        yield return new WaitForSeconds(displayTime);

        // Start loading the MainMenu in the background
        GameBootstrapper.Instance.SceneManager.GoToMainMenu();

        // Wait for the scene to be ready and activate it
        yield return new WaitUntil(() => GameBootstrapper.Instance.SceneManager.IsSceneReady());
        GameBootstrapper.Instance.SceneManager.ActivateScene();
    }
}