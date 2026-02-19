using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    public GameObject pauseMenu;
    public GameObject HUDCanvas;
    public bool isPaused = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
            Debug.Log("Pause:" + isPaused);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        HUDCanvas.SetActive(true);
        _playerInput.SwitchCurrentActionMap("Player");
        isPaused = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        HUDCanvas.SetActive(false);
        pauseMenu.SetActive(true);
        _playerInput.SwitchCurrentActionMap("UI");
        isPaused = true;
    }
}
