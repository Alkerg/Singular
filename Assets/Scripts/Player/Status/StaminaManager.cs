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
        autoRecoverStaminaCoroutine = StartCoroutine(AutoRecoverStamina(3f, 4f));
    }

    public void RecoverStamina(float amount)
    {
        _currentStamina += amount;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);
        OnStaminaChanged?.Invoke(_currentStamina);
    }

    IEnumerator AutoRecoverStamina(float initialDelay, float timeToFill)
    {
        yield return new WaitForSeconds(initialDelay);
        float elapsedTime = 0f;
        float startingStamina = _currentStamina;
        while (elapsedTime < timeToFill)
        {
            elapsedTime += Time.deltaTime;
            _currentStamina = Mathf.Lerp(startingStamina, _maxStamina, elapsedTime / timeToFill);
            OnStaminaChanged?.Invoke(_currentStamina);
            yield return null;
        }
    }


}
