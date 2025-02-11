using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles detection of player & corresponding creature spawning.
/// </summary>
public class CreatureZone : MonoBehaviour
{
    private bool _isPlayerContained = false;

    // Update is called once per frame
    void Update()
    {
        // no functionality unless player is contained
        if (!_isPlayerContained)
        {
            // TODO: despawn creature if not already despawned
            Debug.Log("Despawn");

            return;
        }
        
        // activating creature in this zone - only if creature is not already active in another zone
        if (CreatureManager.Instance.ActiveZone is null)
        {
            CreatureManager.Instance.ActivateCreatureAggro(this);

            // TODO: spawna creature
            Debug.Log("Spawn");
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
            CreatureManager.Instance.DeactivateCreatureAggro();
    }
}
