using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player detection within this collider.
/// This collider is one of a list of colliders that comprises the creature's spawn zone.
/// </summary>
public class CreatureZoneCollider : MonoBehaviour
{
    public bool IsPlayerContained { private set; get; } = false;

    private void OnTriggerEnter(Collider other)
    {
        // CreatureZone can ONLY collide with player - no need to check

        IsPlayerContained = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // CreatureZone can ONLY collide with player - no need to check

        IsPlayerContained = false;
    }
}
