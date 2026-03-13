using System.Collections;
using TMPro;
using UnityEngine;

public enum ScreenTextType
{
    None,
    Title,
    Help
}

public class ScreenTextManager : MonoBehaviour
{
    public static ScreenTextManager Instance { get; private set; }
    public TextMeshProUGUI screenTextTitle;
    public TextMeshProUGUI screenTextHelp;
    public GameObject healthBarGO;
    public GameObject staminaBarGO;
    private AudioSource _audioSource;

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

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void ShowScreenText(string text, ScreenTextType type = ScreenTextType.None, float delay = 2f, AudioClip audioClip = null)
    {
        switch (type)
        {
            case ScreenTextType.Title:
                SetAllHUDElementsVisivility(false);
                screenTextTitle.text = text;
                break;
            case ScreenTextType.Help:
                screenTextHelp.text = text;
                break;
        }
        if (audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }

        StartCoroutine(CleanTextAfterDelay(delay, type));
    }

    IEnumerator CleanTextAfterDelay(float delay, ScreenTextType type = ScreenTextType.None)
    {
        yield return new WaitForSeconds(delay);
        switch (type)
        {
            case ScreenTextType.Title:
                screenTextTitle.text = "";
                break;
            case ScreenTextType.Help:
                screenTextHelp.text = "";
                break;
        }
        SetAllHUDElementsVisivility(true);
    }

    void SetAllHUDElementsVisivility(bool visible)
    {
        healthBarGO.SetActive(visible);
        staminaBarGO.SetActive(visible);
        Crosshair.SetCrosshairVisivility(visible);
    }
}
