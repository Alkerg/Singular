using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void HideCrosshair()
    {
        Instance.gameObject.SetActive(false);
    }

    public static void ShowCrosshair()
    {
        Instance.gameObject.SetActive(true);
    }

    public static void SetCrosshairVisivility(bool isVisible)
    {
        Instance.gameObject.SetActive(isVisible);
    }
}
