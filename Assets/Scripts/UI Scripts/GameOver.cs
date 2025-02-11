using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToStart()
    {
        SceneManager.LoadScene("Main Menu Scene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
