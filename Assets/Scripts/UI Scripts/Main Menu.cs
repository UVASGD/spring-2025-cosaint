using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private AudioSource audioSource;

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
        SceneManager.LoadScene("Gameplay Scene");
        Debug.Log("Going to Gameplay Scene");
    }
}
