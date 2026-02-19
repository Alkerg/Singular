using UnityEngine;
using UnityEngine.UI;

public class StatusBarManager : MonoBehaviour
{
    private Slider _healthSlider;
    [SerializeField] private HealthManager _healthManager;

    private void OnEnable()
    {
        _healthSlider = GetComponent<Slider>();
        _healthManager.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        _healthManager.OnHealthChanged -= UpdateHealthBar;       
    }

    private void UpdateHealthBar(float currentHealth)
    {
        _healthSlider.value = currentHealth/_healthManager._maxHealth;
    }
}
