using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles a floating audio source to make sure it has the proper volume level.
/// Also reduces volume level while paused
/// </summary>
public class AudioSourceVolumeSetter : MonoBehaviour
{
    [SerializeField, Tooltip("Used to control volume level of audio source.")]
    private AudioSource _audioSource;

    private void Awake()
    {
        // 25% volume while paused, otherwise 100%
        // necessary to ensure that spatial audio is properly reduced by sliders even during scene enter transition
        _audioSource.volume = GameManager.GetSFXVolume(); // cannot be paused during scene transition
    }

    // Update is called once per frame
    void Update()
    {
        // 25% volume while paused, otherwise 100%
        _audioSource.volume = (Time.timeScale == 0 ? 0.25f : 1f) * GameManager.GetSFXVolume();
    }
}
