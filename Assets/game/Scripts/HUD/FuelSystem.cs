using UnityEngine;
using System;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{

    [SerializeField] private InputController inputController;
    [SerializeField] private Image imgProgress;
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 2f; // per second when gas pressed
    [SerializeField] private float idleConsumptionRate = 0.1f; // small drain even if not pressing gas

    [SerializeField] private float currentFuel;
    public float CurrentFuel => currentFuel;
    public float MaxFuel => maxFuel;

    public event Action<float> OnFuelChanged;
    public event Action OnFuelEmpty;

    private bool isOutOfFuel = false;

    private void Awake()
    {
        currentFuel = maxFuel;
    }

    private void Update()
    {
        // Always drain a small amount
        if (!inputController.gasPressed && !inputController.brakePressed)
        {
            HandleGasInput(idleConsumptionRate);
        }
        else if (inputController.gasPressed)
        {
            HandleGasInput(inputController._moveInput * fuelConsumptionRate);
        }
        else if (inputController.brakePressed)
        {
            HandleGasInput(fuelConsumptionRate / 5);
        }
    }

    public void HandleGasInput(float rate)
    {
        Debug.Log($"inputController.gasInput : {inputController._moveInput}");
        if (!isOutOfFuel)
        {
            ConsumeFuel(rate * Time.deltaTime);
        }
    }

    private void ConsumeFuel(float amount)
    {
        if (isOutOfFuel) return;

        currentFuel -= amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        //OnFuelChanged?.Invoke(currentFuel / maxFuel);

        if (currentFuel <= 0f)
        {
            isOutOfFuel = true;
            OnFuelEmpty?.Invoke();
        }
        DisplayFuel();
    }

    public void Refill(float amount)
    {
        currentFuel = 100;// Mathf.Clamp(currentFuel + amount, 0f, maxFuel);
        isOutOfFuel = false;
        DisplayFuel();
        //OnFuelChanged?.Invoke(currentFuel / maxFuel);
    }

    void DisplayFuel()
    {
        if (imgProgress == null) return;
        Debug.Log($"currentFuel : {currentFuel}");
        imgProgress.fillAmount = currentFuel / maxFuel;
    }
}
