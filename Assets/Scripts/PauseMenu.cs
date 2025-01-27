using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public NewPlayerMovement movementScript;
    public static bool pausePressed;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    public void PauseFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pausePressed = true;
            Debug.Log("Pause Pressed");
            //Debug.Log("pausePressed: " + pausePressed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("pause updating");
        if (pausePressed == true)
        {
            //Debug.Log("pausepressed is true");
            if (gameIsPaused)
            {
                //Debug.Log("resume attempted");
                ResumeGame();
            }
            else
            {
                //Debug.Log("Pause attempted");
                PauseGame();
            }
            pausePressed = false;
        }
        //Debug.Log("Timescale: " + Time.timeScale);
        //Debug.Log("pausePressed: " + pausePressed);
        //Debug.Log("gameIsPaused: " + gameIsPaused);
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Debug.Log("timescale reset");
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        //movementScript.enabled = true;
        movementScript.isPaused = false;
    }

    void PauseGame()
    {
        //movementScript.enabled = false;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        Debug.Log("gameIsPaused set to: " + gameIsPaused);
        movementScript.isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void BackToMenu()
    {
        //ResumeGame();
        Time.timeScale = 1f;
        gameIsPaused = false;
        Debug.Log("gameIsPaused set to: " + gameIsPaused);
        SceneManager.LoadScene("Main Menu");
    }
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
