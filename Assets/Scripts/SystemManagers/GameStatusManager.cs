using UnityEngine;
using UnityEngine.InputSystem;

public class GameStatusManager : MonoBehaviour
{
    public GameObject HUDCanvas;
    public GameObject victoryMenu;
    public GameObject defeatMenu;
    public static int enemiesCount;
    private PlayerInput _playerInput;
    private HealthManager _healthManager;
    private bool victoryAchieved = false;
    void Start()
    {
        Time.timeScale = 1f;  
        _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();  
        _healthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>();
        _healthManager.OnPlayerDeath += HandlePlayerDeath;
        enemiesCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("Total of enemies in scene: " + enemiesCount);
    }
    public void HandlePlayerDeath()
    {
        Time.timeScale = 0.8f;
        _playerInput.SwitchCurrentActionMap("UI");
        HUDCanvas.SetActive(false);
        defeatMenu.SetActive(true);
    }

    void Update()
    {
        if (enemiesCount <= 0 && !victoryAchieved)
        {
            victoryAchieved = true;
            Time.timeScale = 0.8f;
            _playerInput.SwitchCurrentActionMap("UI");
            HUDCanvas.SetActive(false);
            victoryMenu.SetActive(true);
        }
    }
}
