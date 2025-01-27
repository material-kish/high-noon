using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetFuel : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public GameObject fuelTankList;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.fuelLeft = 0;
            // set all tanks to active
            int children = fuelTankList.transform.childCount;
            for (int i = 0; i < children; i++)
            {
                fuelTankList.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
