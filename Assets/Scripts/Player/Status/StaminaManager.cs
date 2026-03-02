using UnityEngine;
using System;
using System.Collections;

public class StaminaManager : MonoBehaviour
{
    public float _maxStamina = 100f;
    public float _currentStamina { get; private set; }
    public event Action<float> OnStaminaChanged;
    private Coroutine autoRecoverStaminaCoroutine;

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
        if (autoRecoverStaminaCoroutine != null)
        {
            StopCoroutine(autoRecoverStaminaCoroutine);
        }
        autoRecoverStaminaCoroutine = StartCoroutine(AutoRecoverStamina(10f, 3f, 1f));
    }

    public void RecoverStamina(float amount)
    {
        _currentStamina += amount;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        OnStaminaChanged?.Invoke(_currentStamina);
    }

    IEnumerator AutoRecoverStamina(float recoverAmount,float initialDelay, float delay)
    {
        yield return new WaitForSeconds(initialDelay);
        while (_currentStamina < _maxStamina)
        {
            RecoverStamina(recoverAmount);
            yield return new WaitForSeconds(delay);
        }
    }


}
