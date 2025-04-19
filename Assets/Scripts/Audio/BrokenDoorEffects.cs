using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the function to trigger SFX and particle VFX for broken door.
/// </summary>
public class BrokenDoorEffects : MonoBehaviour
{
    [SerializeField, Tooltip("Used to trigger SFX")]
    private AudioSource _audio;

    public void TriggerEffects()
    {
        _audio.PlayOneShot(_audio.clip);
    }
}
