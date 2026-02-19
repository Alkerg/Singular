using UnityEngine;
using UnityEngine.UI;

public class StaminaBarManager : MonoBehaviour
{
    private Slider _staminaSlider;
    [SerializeField] private StaminaManager _staminaManager;

    private void OnEnable()
    {
        _staminaSlider = GetComponent<Slider>();
        _staminaManager.OnStaminaChanged += UpdateStaminaBar;
    }

    private void OnDisable()
    {
        _staminaManager.OnStaminaChanged -= UpdateStaminaBar;       
    }

    private void UpdateStaminaBar(float currentStamina)
    {
        _staminaSlider.value = currentStamina/_staminaManager._maxStamina;
    }
}
