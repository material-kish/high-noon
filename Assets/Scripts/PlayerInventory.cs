using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public const int FUEL_MAX = 100;
    public float fuelLeft;
    [SerializeField] int fuelPerTank;
    [SerializeField] float fuelMileage;
    [SerializeField] float boostFuelUsage;

    public InventoryUI inventoryUI;

    public void TankCollected()
    {
        fuelLeft += fuelPerTank;
        fuelLeft = Mathf.Clamp(fuelLeft, 0, FUEL_MAX);
    }

    public void FuelUsed(string clickType)
    {
        if (clickType == "up")
        {
            fuelLeft -= Time.deltaTime * fuelMileage;
        }
        if (clickType == "forward")
        {
            fuelLeft -= boostFuelUsage;
        }
        fuelLeft = Mathf.Clamp(fuelLeft, 0, FUEL_MAX);
    }

    private void Start()
    {
        fuelLeft = 0;
    }

    private void Update()
    { 
        inventoryUI.ShowFuel(fuelLeft);
    }
}
