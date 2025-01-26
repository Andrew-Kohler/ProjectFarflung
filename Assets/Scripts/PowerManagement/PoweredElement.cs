using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used as a wrapper for all types of things that can be powered and unpowered.
/// Handles zone-level and switch-level power which all powered elements may use.
/// </summary>
public abstract class PoweredElement : MonoBehaviour
{
    [SerializeField, Tooltip("Index of the light switch corresponding with the PoweredLight. -1 if there is no switch.")]
    protected int _switchIndex = -1;

    private bool _isSwitchOn;
    private bool _isZoneOn;

    private void Awake()
    {
        // Precondition: must be an appropriate switch index in range of list (or -1)
        if (_switchIndex < -1 || _switchIndex >= GameManager.Instance.SceneData.PowerSwitches.Length)
            throw new System.Exception("Invalid Switch Index: MUST be either -1 or within the range of the Power Switches game manager list.");

        // Determine starting switch state
        _isSwitchOn = true; // on by default, unless overriden by switch state
        if (_switchIndex != -1)
        {
            _isSwitchOn = GameManager.Instance.SceneData.PowerSwitches[_switchIndex];
        }
    }

    /// <summary>
    /// Called by the power zone to indicate that the zone is unpowered.
    /// </summary>
    public void PowerDownZone()
    {
        _isZoneOn = false;
        // light turns off no matter what

        DisablePoweredElement();
    }

    /// <summary>
    /// Called by the power zone to indicate that the zone is powered.
    /// </summary>
    public void PowerUpZone()
    {
        _isZoneOn = true;

        if (_isSwitchOn) // only enable element if switch is ALSO on
            EnablePoweredElement();
    }

    /// <summary>
    /// Called by the power switch to switch the switch's toggle state.
    /// </summary>
    public void FlipPowerSwitch()
    {
        // Precondition: can only flip power switch if there is one that exists
        if (_switchIndex == -1)
            throw new System.Exception("Cannot call FlipPowerSwitch function! This PoweredLight has no corresponding switch.");

        _isSwitchOn = !_isSwitchOn;

        // turn on light if zone was already on
        if (_isSwitchOn && _isZoneOn)
            EnablePoweredElement();
    }

    /// <summary>
    /// Returns whether the current element is powered or not.
    /// Useful for darkness checks (for the creature).
    /// </summary>
    /// <returns></returns>
    public bool IsPowered()
    {
        return _isSwitchOn && _isZoneOn;
    }

    /// <summary>
    /// Toggles element as visually/functionally powered.
    /// Does NOT handle checks for if it should be enabled.
    /// </summary>
    protected abstract void EnablePoweredElement();

    /// <summary>
    /// Toggles element as visually/functionally unpowered.
    /// Does NOT handle checks for if it should be disabled.
    /// </summary>
    protected abstract void DisablePoweredElement();
}
