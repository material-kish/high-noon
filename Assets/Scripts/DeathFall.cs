using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathFall : MonoBehaviour
{

    [SerializeField] float deathHeight;

    void DeathFallFunction()
    {
        if (gameObject.transform.position.y <= deathHeight)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Update()
    {
        DeathFallFunction();
    }
}