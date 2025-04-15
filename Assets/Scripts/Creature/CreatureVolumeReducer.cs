using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles reduction in creature pursuit volume while player has paused the game
/// </summary>
public class CreatureVolumeReducer : MonoBehaviour
{
    [SerializeField, Tooltip("Used to control volume level of audio source.")]
    private AudioSource _audioSource;

    // Update is called once per frame
    void Update()
    {
        // 25% volume while paused, otherwise 100%
        _audioSource.volume = (Time.timeScale == 0 ? 0.25f : 1f) * GameManager.GetSFXVolume();
    }
}
