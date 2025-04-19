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

    private bool _isPaused = false;

    private void Awake()
    {
        // ensure volume can be changed, even during scene enter when timescale is 0
        _audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
    }

    // Update is called once per frame
    void Update()
    {
        // 25% volume while paused, otherwise 100%
        _audioSource.volume = (_isPaused ? 0.25f : 1f) * GameManager.GetSFXVolume();
    }

    private void OnEnable()
    {
        PauseControls.onPauseOpen += SetPause;
        PauseControls.onPauseClose += SetUnpause;
    }

    private void OnDisable()
    {
        PauseControls.onPauseOpen -= SetPause;
        PauseControls.onPauseClose -= SetUnpause;
    }

    private void SetPause()
    {
        _isPaused = true;
    }

    private void SetUnpause()
    {
        _isPaused = false;
    }
}
