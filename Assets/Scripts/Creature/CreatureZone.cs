using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles detection of player & corresponding creature spawning.
/// </summary>
public class CreatureZone : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Spawn zone options for spawning the creature. The farthest location will be prioritized")]
    private Transform[] _spawnLocations;
    [SerializeField, Tooltip("Corresponding PoweredLight that must be off for the creture to be able to spawn.")]
    private PoweredLight _correspondingLight;

    [Header("References")]
    [SerializeField, Tooltip("Used to spawn/despawn creature functionality.")]
    private CreatureMotion _creature;

    private bool _isPlayerContained = false;

    private void Awake()
    {
        // Precondition: at least one spawn location
        if (_spawnLocations.Length == 0)
            throw new System.Exception("Invalid Creature Zone Configuration: MUST have at least one spawn location");
    }

    // Update is called once per frame
    void Update()
    {
        // no functionality unless player is contained AND the light is powered off
        if (!_isPlayerContained || _correspondingLight.IsPowered())
        {
            // only call despawn functionality once per despawn
            if (_creature.IsCreatureActive())
                _creature.DespawnCreature();

            return;
        }
        
        // activating creature in this zone - only if creature is not already active in another zone
        if (CreatureManager.Instance.ActiveZone is null)
        {
            CreatureManager.Instance.AttachCurrentZone(this);

            // functionally spawn creature
            _creature.SpawnCreature(_spawnLocations);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // CreatureZone can ONLY collide with player - no need to check

        _isPlayerContained = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // CreatureZone can ONLY collide with player - no need to check

        _isPlayerContained = false;

        // deactivate aggro if current zone is left - either keeps it gone or respawns it in new region
        if (CreatureManager.Instance.ActiveZone == this)
            CreatureManager.Instance.DetachCurrentZone();
    }
}
