using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles a floating audio source to make sure it has the proper volume level.
/// Also reduces volume level while paused
/// </summary>
public class AudioSourceVolumeSetter : MonoBehaviour
{
    const float LERP_SPEED = 2f;

    [SerializeField, Tooltip("Used to control volume level of audio source.")]
    private AudioSource _audioSource;

    private bool _isPaused = false;
    private float _prevTime;

    private void Awake()
    {
        // ensure volume can be changed, even during scene enter when timescale is 0
        // okay actually this is NOT solving the scene load sound bug but I swear it fixed it once and then not later - inconsistent?
        _audioSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

        // start at 0 to prevent weird sound on scene load
        _audioSource.volume = 0f;

        _prevTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        // 25% volume while paused, otherwise 100%
        // smooth lerping to prevent weird sound on scene load
        float goalVol = (_isPaused ? 0.25f : 1f) * GameManager.GetSFXVolume();
        float newVal = _audioSource.volume;
        if (newVal > goalVol)
        {
            newVal -= (Time.realtimeSinceStartup - _prevTime) * LERP_SPEED;
            if (newVal < goalVol)
                newVal = goalVol;
        }
        else if (newVal < goalVol)
        {
            newVal += (Time.realtimeSinceStartup - _prevTime) * LERP_SPEED;
            if (newVal > goalVol)
                newVal = goalVol;
        }
        _audioSource.volume = newVal;

        _prevTime = Time.realtimeSinceStartup;
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
