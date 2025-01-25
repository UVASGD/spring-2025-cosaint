using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

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
