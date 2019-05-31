using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayAudioOnClick()
    {
        audioSource.Play();
    }
}
