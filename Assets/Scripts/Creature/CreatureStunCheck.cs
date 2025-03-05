using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks for collision with light stun trigger to stun the creature.
/// </summary>
public class CreatureStunCheck : MonoBehaviour
{
    private bool _isStunReady = false;

    // we only care about enter due to how the trigger is enabled/disabled
    private void OnTriggerEnter(Collider other)
    {
        // make sure its not the player colliding with it
        if (other.gameObject.layer == LayerMask.NameToLayer("FlashlightStun"))
        {
            CreatureManager.Instance.TryStunCreature();
        }
    }
}
