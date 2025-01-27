using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for all power zones, for easy access (for instance, for the terminal)
/// Also handles proper initialization of all powered elements in the system
/// </summary>
public class PowerSystem : MonoBehaviour
{
    [Header("Capacity")]
    [SerializeField, Tooltip("Total maximum capacity of the power grid before a shutdown will occur.")]
    private int _capacity;

    [Header("Floor Configuration")]
    [SerializeField, Tooltip("Specifies which floor the current scene is for. 0 = hangar")]
    public int FloorNum;
    [SerializeField, Tooltip("List of objects pertaining to the hangar.")]
    private GameObject[] HangarObjects;
    [SerializeField, Tooltip("List of objects pertaining to floor 1.")]
    private GameObject[] Floor1Objects;
    [SerializeField, Tooltip("List of objects pertaining to floor 2.")]
    private GameObject[] Floor2Objects;
    [SerializeField, Tooltip("List of objects pertaining to floor 3.")]
    private GameObject[] Floor3Objects;

    [Header("Zones")]
    [Tooltip("Ordered list of powered zones, used to access power data.")]
    public PoweredZone[] PoweredZones;

    private void Awake()
    {
        // give all lights a chance to configure their data and power states properly, even if they will be disabled
        foreach (GameObject obj in Floor1Objects)
            obj.SetActive(true);
        foreach (GameObject obj in Floor2Objects)
            obj.SetActive(true);
        foreach (GameObject obj in Floor3Objects)
            obj.SetActive(true);
        foreach (GameObject obj in HangarObjects)
            obj.SetActive(true);
    }

    private void Start()
    {
        // must be in start to give lights a chance to actually update in their awake methods
        switch (FloorNum)
        {
            case 0: // 0 = hangar
                foreach (GameObject obj in Floor1Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in Floor2Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in Floor3Objects)
                    obj.SetActive(false);
                break;
            case 1:
                foreach (GameObject obj in Floor2Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in Floor3Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in HangarObjects)
                    obj.SetActive(false);
                break;
            case 2:
                foreach (GameObject obj in Floor1Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in Floor3Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in HangarObjects)
                    obj.SetActive(false);
                break;
            case 3:
                foreach (GameObject obj in Floor1Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in Floor2Objects)
                    obj.SetActive(false);
                foreach (GameObject obj in HangarObjects)
                    obj.SetActive(false);
                break;
            default:
                throw new System.Exception("Invalid Power System Setup. Must have a floor number between 1 and 3.");
        }
    }

    /// <summary>
    /// returns the current power consumption of all zones in the power grid.
    /// </summary>
    public int GetCurrentConsumption()
    {
        int currConsum = 0;
        foreach (PoweredZone zone in PoweredZones)
        {
            if (zone.IsPowered())
            {
                currConsum += zone.GetCurrentConsumption();
            }
        }
        return currConsum;
    }

    /// <summary>
    /// returns the capacity of the power grid
    /// </summary>
    /// <returns></returns>
    public int GetCapacity()
    {
        return _capacity;
    }

    /// <summary>
    /// Disables ALL zones in the entire grid (except locked zones such as command).
    /// Occurs when power grid exceeds capacity.
    /// </summary>
    public void ShutdownGrid()
    {
        for (int i = 0; i < GameManager.Instance.SceneData.PoweredZones.Length; i++)
        {
            // only shut down unlocked zones, otherwise they stay where they are (i.e. locked command stays on)
            if (GameManager.Instance.SceneData.TerminalUnlocks[i])
            {
                GameManager.Instance.SceneData.PoweredZones[i] = false;
            }

            // actually update zones to match new game manager data
            foreach (PoweredZone zone in PoweredZones)
                zone.UpdatePowerStates();
        }
    }

    // TODO: system for system exceeding capacity. Some checks in update? as well as a function to turn off ALL zones?
}
