using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    public void LeaveComment()
    {
        Application.OpenURL("https://jacobfm.itch.io/high-noon");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
