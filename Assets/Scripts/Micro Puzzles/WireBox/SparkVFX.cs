using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles enabling/disabling of wirebox spark VFX
/// </summary>
public class SparkVFX : MonoBehaviour
{
    [SerializeField, Tooltip("The sparking particle system")]
    public ParticleSystem _sparkParticles;
    [SerializeField, Tooltip("Used to enable/disable sparking SFX.")]
    private AudioSource _audio;
    [SerializeField, Tooltip("Used to access wire box identifier and its corresponding light zone.")]
    private WireBoxHandler _wirebox;

    // Update is called once per frame
    void Update()
    {
        // Spark Condition, not in fixed box + area is activated
        // only play sparks when player has control (i.e. not in terminal OR wire box)
        if (!GameManager.Instance.SceneData.FixedWireBoxes.Contains(_wirebox.IdentifierName) && _wirebox.LightZone.IsPowered() && GameManager.Instance.PlayerEnabled)
        {
            // don't restart spark particles if already playing
            if (!_sparkParticles.isPlaying)
            {
                _sparkParticles.Play();
                _audio.Play();
            }
        }
        else
        {
            _sparkParticles.Stop();
            _audio.Stop();
        }
    }

    // TODO: add corresponding functionality for SFX enable/disable

    // TODO: make both SFX and VFX disable while in wire box puzzle also - too distracting
}
