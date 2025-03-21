using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player collision detection and death sequence intiation based on creature contact within the death realm.
/// </summary>
public class CreatureFinalDeath : MonoBehaviour
{
    [Header("Death Sequence")]
    [SerializeField, Tooltip("Creature reference used to disable creature motion after touching player.")]
    private CreatureMotion _creature;
    [SerializeField, Tooltip("Animator to activate to start the death sequence.")]
    private Animator _anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // TODO: any sequence involving the camera, creature, SFX, or anything else when the player is caught

            // freeze the player and creature
            GameManager.Instance.PlayerEnabled = false;
            _creature.enabled = false;

            // start death animations in scene
            _anim.SetTrigger("Activate");

            // scene transition and data clearing is handled by animation event... nothing else here
        }
    }
}
