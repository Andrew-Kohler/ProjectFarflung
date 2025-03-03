using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles detection of player & corresponding creature spawning.
/// </summary>
public class CreatureZone : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Corresponding PoweredLight that must be off for the creture to be able to spawn.")]
    private PoweredLight _correspondingLight;

    [Header("References")]
    [SerializeField, Tooltip("Used to spawn/despawn creature functionality.")]
    private CreatureMotion _creature;
    [SerializeField, Tooltip("Parent of spawn locations for the creature.")]
    private GameObject _spawnLocationsParent;
    [SerializeField, Tooltip("Parent of colliders used to fetch colliders on start.")]
    private GameObject _collidersParent;

    private Transform[] _spawnLocations;
    private CreatureZoneCollider[] _colliders;
    private bool _isPlayerContained = false;

    private void Awake()
    {
        // Precondition: at least one spawn location
        _spawnLocations = new Transform[_spawnLocationsParent.transform.childCount];
        for (int i = 0; i < _spawnLocations.Length; i++)
            _spawnLocations[i] = _spawnLocationsParent.transform.GetChild(i);
        if (_spawnLocations.Length == 0)
            throw new System.Exception("Invalid Creature Zone Configuration: MUST have at least one spawn location transform.");

        // Precondition: at least one collider
        _colliders = _collidersParent.GetComponentsInChildren<CreatureZoneCollider>();
        if (_colliders.Length == 0)
            throw new System.Exception("Invalid Creature Zone Configuration: MUST have at least one associated CreatureZoneCollider contained in Colliders object.");
    }

    // Update is called once per frame
    void Update()
    {
        // update player contained state
        // true if ANY SINGLE collider contains the player still
        bool newContainedState = false;
        foreach (CreatureZoneCollider coll in _colliders)
        {
            if (coll.IsPlayerContained)
            {
                newContainedState = true;
                break;
            }
        }
        _isPlayerContained = newContainedState;

        // no functionality unless player is contained AND the light is powered off
        // no light also works - it just means that only player leaving the zone can prevent creature spawning
        if (!_isPlayerContained || (_correspondingLight is not null && _correspondingLight.IsPowered()))
        {
            // only call despawn functionality once per despawn
            if (_creature.IsCreatureActive())
                _creature.DespawnCreature();

            // deactivate active zone player leaves current zone - either keeps creature gone or respawns it in new region
            if (CreatureManager.Instance.ActiveZone == this)
                CreatureManager.Instance.DetachCurrentZone();

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
}
