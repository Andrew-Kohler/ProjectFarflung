using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls motion of creature object by tracking rotation towards the player and accounting for speed ramp.
/// </summary>
public class CreatureMotion : MonoBehaviour
{
    [SerializeField, Tooltip("Rotation lerping speed to face the player.")]
    private float _rotationSharpness;

    // Update is called once per frame
    void Update()
    {
        // lerp rotation
        Vector3 dirToPlayer = CreatureManager.Instance.PlayerTransform.position - transform.position;
        Quaternion goalRot = transform.rotation;
        goalRot.SetLookRotation(dirToPlayer, Vector3.up);
        // smooth motion - also scaled by creature move speed factor
        transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, 1f - Mathf.Exp(-_rotationSharpness * Time.deltaTime * CreatureManager.Instance.CurrentSpeed));
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
        transform.rotation = farthestOption.rotation;

        gameObject.SetActive(true);

        // TODO: smoother spawning with behavior delay and spawning animation
    }

    /// <summary>
    /// deactivates object containing creature functionality AND model.
    /// </summary>
    public void DespawnCreature()
    {
        gameObject.SetActive(false);

        // TODO: smoother despawning with behavior freeze and despawn animation
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
