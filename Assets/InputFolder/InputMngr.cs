using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMngr : MonoBehaviour
{
    public GameObject w;
    
    public GameObject a;
    
    public GameObject s;
    
    public GameObject d;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            w.SetActive(false);
        }
        else w.SetActive(true);

        if (Input.GetKey(KeyCode.A))
        {
            a.SetActive(false);
        }
        else a.SetActive(true);

        if (Input.GetKey(KeyCode.S))
        {
            s.SetActive(false);
        }
        else s.SetActive(true);

        if (Input.GetKey(KeyCode.D))
        {
            d.SetActive(false);
        }
        else d.SetActive(true);
    }
}
