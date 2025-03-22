using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks for collision with light stun trigger to stun the creature.
/// </summary>
public class CreatureStunCheck : MonoBehaviour
{
    // we only care about enter due to how the trigger is enabled/disabled
    private void OnTriggerEnter(Collider other)
    {
        // make sure its not the player colliding with it
        if (other.gameObject.layer == LayerMask.NameToLayer("FlashlightStun"))
        {
            CreatureManager.Instance.TryStunCreature();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // TODO: any sequence involving the camera, creature, SFX, or anything else when the player is caught

            // set death realm variable
            GameManager.Instance.SceneData.IsInDeathRealm = true;

            // load death realm scene (no need for scene name due to 'Resume' functionality)
            if (!other.gameObject.TryGetComponent(out SceneTransitionRef player))
                throw new System.Exception("Player MUST contain PlayerPositionLoader in any non Death Realm scene");
            player.TransitionHandler.LoadScene("Resume");

            // save scene data to game data to prevent quitting to 'undie'
            GameManager.Instance.SaveSceneDataToGameData();
        }
    }
}
