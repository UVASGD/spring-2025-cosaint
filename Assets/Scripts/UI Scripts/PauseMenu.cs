using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    Canvas pauseCanvas;
    bool isPaused = false;

    void Start()
    {
        pauseCanvas = GameObject.Find("Pause Canvas").GetComponent<Canvas>();
        pauseCanvas.enabled = false;
    }

  
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();

            } else 
            {
                PauseGame();
            }

        }
        
    }

        public void PauseGame(){
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseCanvas.enabled = true;
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Paused Game");


    }

    public void ResumeGame(){

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseCanvas.enabled = false;
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Resume Game");

        
    }

    


    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
