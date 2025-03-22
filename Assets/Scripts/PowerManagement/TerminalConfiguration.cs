using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for important terminal configuration data.
/// Accessed by other scripts to interpret this data
/// </summary>
public class TerminalConfiguration : MonoBehaviour
{
    [Tooltip("Terminal index, used for terminal unlocking AND knowing where to zoom in on (puzzle vs screen).")]
    public int ZoneIndex;
    [Tooltip("Scene reference to the power system for retrieving data to display.")]
    public PowerSystem PowerSystem;
    [Tooltip("Transform for spawning in front of terminal upon scene load / resume.")]
    public Transform SpawnPos;

    private void Awake()
    {
        // Precondition: linked to power system
        if (PowerSystem is null)
            throw new System.Exception("Terminal Configuration MUST have a reference to a PowerSystem in the scene.");

        // Precondition: proper zone index
        if (ZoneIndex < 0 || ZoneIndex >= GameManager.Instance.SceneData.PoweredZones.Length || ZoneIndex >= GameManager.Instance.SceneData.TerminalUnlocks.Length)
            throw new System.Exception("Invalid Terminal Index: must be in range of powered zones list in game manager.");

        // Precondition: assigned spawn pos
        if (SpawnPos is null)
            throw new System.Exception("Invalid Terminal Spawn Pos: no reference found.");
    }
}
