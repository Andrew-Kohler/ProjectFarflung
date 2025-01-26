using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for all power zones, for easy access (for instance, for the terminal)
/// </summary>
public class PowerSystem : MonoBehaviour
{
    [Tooltip("Ordered list of powered zones, used to access power data.")]
    public PoweredZone[] PoweredZones;
}
