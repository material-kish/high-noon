using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Slider slider;


    public void ShowFuel(float fuel)
    {
        slider.value = fuel;
    }

}
