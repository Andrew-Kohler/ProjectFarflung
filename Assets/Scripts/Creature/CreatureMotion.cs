using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// Controls motion of creature object by tracking rotation towards the player and accounting for speed ramp.
/// </summary>
public class CreatureMotion : MonoBehaviour
{
    [SerializeField, Tooltip("Distince within which creature will begin moving towards the player.")]
    private float _aggroDistance;
    [SerializeField, Tooltip("Rotation lerping speed to face the player.")]
    private float _rotationSharpness;
    [SerializeField, Tooltip("Animator for the models motion.")]
    private Animator _animator;


    // Update is called once per frame
    void Update()
    {
        // activate aggro (speed ramping)
        // (1) activate aggro in range
        // (2) ALSO activate aggro if creature does not have 0 speed (not fully calmed)
        if (!CreatureManager.Instance.IsAggro && (Vector3.Distance(CreatureManager.Instance.PlayerTransform.position, transform.position) < _aggroDistance
            || CreatureManager.Instance.CurrentSpeed > 0f))
        {
            CreatureManager.Instance.IsAggro = true;
            
        }

      

        // rotation lerping - ALWAYS active
        Vector3 playerPos = CreatureManager.Instance.PlayerTransform.position;
        playerPos.y = 0; // don't track with player jumps
        Vector3 dirToPlayer = playerPos - transform.position;

        // LERP ROTATION
        Quaternion goalRot = transform.rotation;
        goalRot.SetLookRotation(dirToPlayer, Vector3.up);
        // smooth motion - also scaled by creature move speed factor
        transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, 1f - Mathf.Exp(-_rotationSharpness * Time.deltaTime * (1f + CreatureManager.Instance.CurrentSpeed)));

        // motion lerping
        if (CreatureManager.Instance.IsAggro)
        {
            // LERP MOTION
            // slower speed when not looking directly at player
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            float angleFactor = math.remap(0, 180, 1, 0, angle); // 0 degree difference = max speed; 180 degree difference = no speed
            transform.position += transform.forward * angleFactor * CreatureManager.Instance.CurrentSpeed * Time.deltaTime;
            //updates creatures animation speed to be similar to its expected speed
            _animator.SetFloat("speed", CreatureManager.Instance.CurrentSpeed);

            _animator.SetBool("isStunned", false);
            _animator.SetBool("isMoving", true);
        }

        //manage other animations
        if (CreatureManager.Instance.CurrentSpeed > 0f) {
            //clear all idle animation triggers
            _animator.SetBool("isIdle", false);
            _animator.SetBool("idleLeftTurn", false);
            _animator.SetBool("idleRightTurn", false);

            //start movement anim, speed handled above
            _animator.SetBool("isMoving", true);
        } 
        else if (CreatureManager.Instance.CurrentSpeed == 0f && _animator.GetBool("isMoving") == true && CreatureManager.Instance.IsStunned == false) //triggers from moving to idling if speed goes down too much
        {
            _animator.SetBool("isMoving", false);
            _animator.SetBool("isIdle", true);
        }
        if (CreatureManager.Instance.IsStunned == true) //handles stunning trigger
        {
            _animator.SetBool("isStunned", true);
            _animator.SetBool("isMoving", false);
        }
    }

    #region Spawning/Despawning
    /// <summary>
    /// activates object containing creature functionality AND model.
    /// spawns the creature at the FURTHEST spawn location from the player (to prevent spawning on top of player).
    /// </summary>
    public void SpawnCreature(Transform[] spawnLocations)
    {
        // determine farthest spawn location
        Transform player = CreatureManager.Instance.PlayerTransform;
        Transform farthestOption = spawnLocations[0];
        for (int i = 1; i < spawnLocations.Length; i++)
        {
            if (Vector3.Distance(player.position, spawnLocations[i].position) > Vector3.Distance(player.position, farthestOption.position))
                farthestOption = spawnLocations[i];
        }

        // move/rotate object
        transform.position = farthestOption.position;

        // set creature to face player when initially spawned
        Vector3 playerPos = CreatureManager.Instance.PlayerTransform.position;
        playerPos.y = 0; // don't track with player jumps
        Vector3 dirToPlayer = playerPos - transform.position;
        Quaternion goalRot = transform.rotation;
        goalRot.SetLookRotation(dirToPlayer, Vector3.up);
        transform.rotation = goalRot;

        gameObject.SetActive(true);

        // TODO: smoother spawning with behavior delay and spawning animation
        _animator.SetBool("isIdle", true);
        _animator.SetBool("isStunned", false);
        _animator.SetBool("idleLeftTurn", true);
        _animator.SetBool("idleRightTurn", false);
    }

    /// <summary>
    /// deactivates object containing creature functionality AND model.
    /// </summary>
    public void DespawnCreature()
    {
        // allow creature to slowly approach 0 speed again
        CreatureManager.Instance.IsAggro = false;

        // TODO: smoother despawning with behavior freeze and despawn animation

        //trigger despawn
        _animator.SetBool("isDespawned", true);
        //CreatureManager.Instance.CurrentSpeed = 0;
        StartCoroutine (RunDespawnAnimation(_animator));
    }

    IEnumerator RunDespawnAnimation(Animator animator) {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length  + 1f);
        gameObject.SetActive(false);
    }


    /// <summary>
    /// Whether the creature is currently active and executing movement behavior.
    /// </summary>
    public bool IsCreatureActive()
    {
        return gameObject.activeSelf;
    }
    #endregion
}
