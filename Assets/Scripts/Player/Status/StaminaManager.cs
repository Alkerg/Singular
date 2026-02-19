using UnityEngine;
using System;

public class StaminaManager : MonoBehaviour
{
    public float _maxStamina = 100f;
    public float _currentStamina { get; private set; }
    public event Action<float> OnStaminaChanged;
    void Start()
    {
        _currentStamina = _maxStamina;
        OnStaminaChanged?.Invoke(_currentStamina);
    }

    public void TakeStamina(float amount)
    {
        _currentStamina -= amount;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        OnStaminaChanged?.Invoke(_currentStamina);
    }

    public void RecoverStamina(float amount)
    {
        _currentStamina += amount;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        OnStaminaChanged?.Invoke(_currentStamina);
    }


}
