using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used as a wrapper for all types of things that can be powered and unpowered.
/// </summary>
public abstract class PoweredElement : MonoBehaviour
{
    /// <summary>
    /// Called by the power zone to indicate that the zone is powered.
    /// </summary>
    public abstract void PowerUpZone();

    /// <summary>
    /// Called by the power zone to indicate that the zone is unpowered.
    /// </summary>
    public abstract void PowerDownZone();

    /// <summary>
    /// Called by the power switch to switch the switch's toggle state.
    /// </summary>
    public abstract void FlipPowerSwitch();
}
