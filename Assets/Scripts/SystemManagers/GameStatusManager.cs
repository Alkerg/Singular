using UnityEngine;

public class GameStatusManager : MonoBehaviour
{
    public GameObject HUDCanvas;
    public GameObject victoryMenu;
    public GameObject defeatMenu;
    private HealthManager _healthManager;
    void Start()
    {
        Time.timeScale = 1f;    
        _healthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>();
        _healthManager.OnPlayerDeath += HandlePlayerDeath;
    }
    public void HandlePlayerDeath()
    {
        Time.timeScale = 0f;
        HUDCanvas.SetActive(false);
        defeatMenu.SetActive(true);
    }
}
