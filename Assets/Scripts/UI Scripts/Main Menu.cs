using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("UI References")]
    public GameObject menuUI; // Reference to your Main Menu UI

    [Header("Video References")]
    public VideoPlayer menuVideoPlayer; // Background menu video
    public VideoPlayer cutsceneVideoPlayer; // Cutscene video player

    [Header("Scene Settings")]
    public string gameplaySceneName = "Gameplay Scene"; // Name of gameplay scene to load

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No AudioSource found on MainMenu GameObject!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void PlayGame()
    {
        // Hide menu UI
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }

        // Stop the menu background music
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Pause or stop the background menu video
        if (menuVideoPlayer != null)
        {
            menuVideoPlayer.Pause(); // or Stop() if you prefer
        }

        // Play the cutscene video
        if (cutsceneVideoPlayer != null)
        {
            cutsceneVideoPlayer.gameObject.SetActive(true); // Ensure it's visible
            cutsceneVideoPlayer.Play();
            cutsceneVideoPlayer.loopPointReached += OnCutsceneFinished;
        }
        else
        {
            Debug.LogWarning("Cutscene VideoPlayer not assigned!");
            // If no cutscene, immediately load gameplay scene
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    private bool isFading = false;

    private void OnCutsceneFinished(VideoPlayer vp)
    {
        if (!isFading)
        {
            isFading = true;
            FindFirstObjectByType<FadeToBlack>()?.StartFade();
            Invoke(nameof(LoadGameplayScene), 1.1f); 
        }
    }

    private void LoadGameplayScene()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

}