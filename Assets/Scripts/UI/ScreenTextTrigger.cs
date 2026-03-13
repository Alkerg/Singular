using System;
using UnityEngine;

public class ScreenTextTrigger : MonoBehaviour
{
    public string textToShow;
    public ScreenTextType textType;
    public float displayDuration = 2f;
    public AudioClip audioClip;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ScreenTextManager.Instance.ShowScreenText(textToShow, textType, displayDuration, audioClip);
            gameObject.SetActive(false);
        }
    }
}
