using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the function to trigger SFX and particle VFX for broken door.
/// </summary>
public class BrokenDoorEffects : MonoBehaviour
{
    [SerializeField, Tooltip("Used to trigger SFX.")]
    private AudioSource _audio;
    [SerializeField, Tooltip("Used to trigger VFX.")]
    private ParticleSystem _particles;

    public void TriggerAudio()
    {
        // don't play audio if player is in wire box or terminal, it gets annoying
        if (GameManager.Instance.PlayerEnabled)
            _audio.PlayOneShot(_audio.clip);
    }

    public void TriggerParticles()
    {
        _particles.Play();
    }
}
