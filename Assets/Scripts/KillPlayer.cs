using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillPlayer : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Obstacle")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
