using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks for collision with light stun trigger to stun the creature.
/// </summary>
public class CreatureStunCheck : MonoBehaviour
{
    private bool _isStunReady = false;

    // Update is called once per frame
    void Update()
    {
        // attempt to stun creature every frame that it is in the light
        if (_isStunReady)
            CreatureManager.Instance.TryStunCreature();
    }

    private void OnTriggerEnter(Collider other)
    {
        // make sure its not the player colliding with it
        if (other.gameObject.layer == LayerMask.NameToLayer("FlashlightStun"))
        {
            _isStunReady = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // make sure its not the player colliding with it
        if (other.gameObject.layer == LayerMask.NameToLayer("FlashlightStun"))
        {
            _isStunReady = false;
        }
    }
}
