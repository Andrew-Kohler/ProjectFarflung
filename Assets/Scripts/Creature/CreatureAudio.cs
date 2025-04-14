using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles idle and pursuit spatial SFX for the creature.
/// </summary>
public class CreatureAudio : MonoBehaviour
{
    [Header("Tuning")]
    [SerializeField, Tooltip("Audio Fade-Between Rate.")]
    private float _fadeBetweenRate;

    [Header("Sounds")]
    [SerializeField, Tooltip("Used to set sound to play.")]
    AudioSource _audioSource;
    [SerializeField, Tooltip("SFX for creature ambient.")]
    AudioClip _idleTrack;
    [SerializeField, Tooltip("SFX for creature pursuit.")]
    AudioClip _pursuitTrack;
    [SerializeField, Tooltip("Used to determine when creature has begun to despawn")]
    CreatureMotion _creature;

    private bool _prevCreatureActive = false;
    private AudioClip _currTrack = null;

    // Update is called once per frame
    void Update()
    {
        // fade out on despawn
        if (_prevCreatureActive && !_creature.IsCreatureActive())
            StartCoroutine(DoFadeOut());

        // skip processing if creature is inactive or despawning
        if (!_creature.IsCreatureActive())
            return;

        // play correct track
        if (_currTrack is null)
        {
            if (CreatureManager.Instance.IsAggro)
            {
                _audioSource.clip = _pursuitTrack;
                _audioSource.Play();
                _currTrack = _pursuitTrack;
            }
            else
            {
                _audioSource.clip = _idleTrack;
                _audioSource.Play();
                _currTrack = _idleTrack;
            }
        }
        else if ((CreatureManager.Instance.IsAggro && _currTrack is not null && _currTrack == _pursuitTrack)
            || (!CreatureManager.Instance.IsAggro && _currTrack is not null && _currTrack == _idleTrack))    // fade into correct track
        {
            float newVol = _audioSource.volume + (_fadeBetweenRate * Time.deltaTime);
            if (newVol > GameManager.GetSFXVolume())
                newVol = GameManager.GetSFXVolume();
            _audioSource.volume = newVol;
        }
        else if ((CreatureManager.Instance.IsAggro && _currTrack is not null && _currTrack == _idleTrack)
            || (!CreatureManager.Instance.IsAggro && _currTrack is not null && _currTrack == _pursuitTrack))   // fade out of incorrect track
        {
            float newVol = _audioSource.volume - (_fadeBetweenRate * Time.deltaTime);
            if (newVol < 0)
            {
                newVol = 0;
                _audioSource.Stop(); // allows new track to take over next frame
                _audioSource.clip = null;
                _currTrack = null;
            }
            _audioSource.volume = newVol;
        }

        _prevCreatureActive = _creature.IsCreatureActive();
    }

    private IEnumerator DoFadeOut()
    {
        while (_audioSource.volume > 0)
        {
            float newVol = _audioSource.volume - (_fadeBetweenRate * Time.deltaTime);
            if (newVol < 0)
            {
                newVol = 0;
                _audioSource.Stop(); // allows new track to take over next frame
                _audioSource.clip = null;
                _currTrack = null;
            }
            _audioSource.volume = newVol;

            yield return null;
        }
    }

    private void OnEnable()
    {
        _audioSource.volume = 0;    
    }

    private void OnDisable()
    {
        // ensure fully faded out by the time the creature is disabled
        StopAllCoroutines();
        _audioSource.Stop();
        _audioSource.clip = null;
        _currTrack = null;
    }
}
