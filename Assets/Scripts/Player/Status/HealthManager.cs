using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class HealthManager : MonoBehaviour
{
    public float _maxHealth = 100f;
    public float _currentHealth { get; private set; }
    public event Action<float> OnHealthChanged;
    public event Action OnPlayerDeath;


    void Start()
    {
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
