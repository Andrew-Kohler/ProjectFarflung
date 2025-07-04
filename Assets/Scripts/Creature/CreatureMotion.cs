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

    [Header("Particle Effects")]
    [SerializeField, Tooltip("Particle system that should play on spawn")]
    private ParticleSystem _spawnEffect;
    [SerializeField, Tooltip("Particle system that should play on idle bobbing")]
    private ParticleSystem _idleEffect;
    [SerializeField, Tooltip("Particle system that should play on stun")]
    private ParticleSystem _stunnedEffect;
    [SerializeField, Tooltip("Particle system that should play on all other motion")]
    private ParticleSystem _motionEffect;

    private bool _isCreatureActive = false;

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
        playerPos.y = transform.position.y; // don't track with player jumps, instead stay locked at the creature's y-level
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
            
            //updates creatures chase animation speed to be similar to its expected speed & trigger VFX
            _animator.SetFloat("speed", CreatureManager.Instance.CurrentSpeed);

            // VFX trailing effects - set y force (only the backwards force since it uses local coordinates)
            var particleMotion = _motionEffect.forceOverLifetime;
            particleMotion.y = new ParticleSystem.MinMaxCurve(CreatureManager.Instance.CurrentSpeed * -15);
        }

        // ANIMATIONS -------------------

        // stunning animation - takes priority!
        if (CreatureManager.Instance.IsStunned == true)
        {
            // start stun anim
            _animator.SetBool("isStunned", true);

            // stun VFX
            if (!_stunnedEffect.isPlaying)
                _stunnedEffect.Play();
                    
            // disable other anims
            _animator.SetBool("isMoving", false);
            _motionEffect.Stop();
            _idleEffect.Stop();

            // should not modify idle anim value to ensure it remains the same for creature stun during idle OR during chase
        }
        // moving animation (exiting idle)
        else if (CreatureManager.Instance.CurrentSpeed > 0f) {
            // start movement anim, speed handled above & VFX
            _animator.SetBool("isMoving", true);

            // motion VFX
            if (!_motionEffect.isPlaying)
                _motionEffect.Play();

            // disable other anims & VFX
            _animator.SetBool("isIdle", false);
            _idleEffect.Stop();
            _animator.SetBool("isStunned", false);
            _stunnedEffect.Stop();

        }
        // idle animation - lowest priority (only of not stunned and not moving)
        else if (CreatureManager.Instance.CurrentSpeed == 0f)
        {
            // start idle anim & VFX
            _animator.SetBool("isIdle", true);

            // idle VFX
            if (!_idleEffect.isPlaying)
                _idleEffect.Play();

            // disable other anims & VFX
            _animator.SetBool("isStunned", false);
            _stunnedEffect.Stop();
            _animator.SetBool("isMoving", false);
            _motionEffect.Stop();
        }
    }

    #region Spawning/Despawning
    /// <summary>
    /// activates object containing creature functionality AND model.
    /// spawns the creature at the FURTHEST spawn location from the player (to prevent spawning on top of player).
    /// </summary>
    public void SpawnCreature(Transform[] spawnLocations)
    {
        // spawn SFX
        AudioManager.Instance.PlayCreatureSpawn();

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

        // activate creature object (for functionality & anims)
        gameObject.SetActive(true);
        // activate creature motion script
        enabled = true;
        _isCreatureActive = true;

        // ALWAYS enter on spawn animation (plays VFX also)
        _animator.Play("Spawn");
        _spawnEffect.Play();
        // ensure proper starting of idle/turning/stun/chase logic
        _animator.SetBool("isIdle", true);
        _animator.SetBool("isMoving", false);
        _animator.SetBool("isStunned", false);
        _animator.SetBool("idleLeftTurn", true);
        _animator.SetBool("idleRightTurn", false);
        _animator.SetBool("isLooking", false);
        _animator.SetBool("isDespawned", false);
    }

    /// <summary>
    /// deactivates object containing creature functionality AND model.
    /// </summary>
    public void DespawnCreature()
    {
        // despawn SFX
        AudioManager.Instance.PlayCreatureDespawn();

        // allow creature to slowly approach 0 speed again
        CreatureManager.Instance.IsAggro = false;
        // turn off motion VFX
        _motionEffect.Stop();
        // deactivate creature motion script - freeze behavior
        enabled = false;
        _isCreatureActive = false;

        // trigger despawn animation & VFX
        _animator.SetBool("isDespawned", true);
        _spawnEffect.Play();
        StartCoroutine(RunDespawnAnimation());
    }

    // ensures proper visual disabling of object after despawn animation
    IEnumerator RunDespawnAnimation()
    {
        // wait for despawn anim to start
        // since despawn is the only animation playing in reverse - check for negative speed
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).speed < 0);
        
        // wait for duration of despawn anim
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
        
        // properly disables the visibility of the creature in addition to functionality
        // ONLY disables creature if creature was NOT re-enabled during this brief window
        if (enabled == false)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Whether the creature is currently active and executing movement behavior.
    /// </summary>
    public bool IsCreatureActive()
    {
        return _isCreatureActive;
    }
    #endregion
}
