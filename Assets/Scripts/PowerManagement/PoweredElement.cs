using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used as a wrapper for all types of things that can be powered and unpowered.
/// Handles zone-level and switch-level power which all powered elements may use.
/// </summary>
public abstract class PoweredElement : MonoBehaviour
{
    [Tooltip("Power draw while this element is powered.")]
    public int PowerDraw;

    [Header("OPTIONAL - Power Switch")]
    [SerializeField, Tooltip("Index of the light switch corresponding with the PoweredLight. -1 if there is no switch.")]
    protected string _switchIdentifier = null;
    [SerializeField, Tooltip("Whether the default state of this element's light switch is off. Flips meaning of stored string in game manager for this switch.")]
    private bool _isSwitchOffByDefault;

    [Header("OPTIONAL - Busted Wire Box")]
    [SerializeField, Tooltip("Used to determine if associated busted power box is fixed for power determination.")]
    private string _wireBoxIdentifier;

    private bool _isSwitchOn;
    private bool _isZoneOn;

    private void OnEnable()
    {
        // ensure element updates when wire box is fixed
        if (!_wireBoxIdentifier.Equals(""))
            WireBoxHandler.WireBoxFixed += UpdatePowerState;
    }

    private void OnDisable()
    {
        if (!_wireBoxIdentifier.Equals(""))
            WireBoxHandler.WireBoxFixed -= UpdatePowerState;
    }

    private void Awake()
    {
        // Precondition: only off switch by default possible if it even has a switch
        if (_isSwitchOffByDefault && _switchIdentifier is null)
            throw new System.Exception("Incorrect PoweredElement configuration. Cannot have switch that is OFF by default if it has no switch.");

        // Determine starting switch state
        _isSwitchOn = true; // on by default, unless overriden by switch state
        if (_switchIdentifier is not null)
        {
            _isSwitchOn = !GameManager.Instance.SceneData.PowerSwitches.Contains(_switchIdentifier);
            if (_isSwitchOffByDefault) _isSwitchOn = !_isSwitchOn; // flip if necessary
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

        // ensures elements are properly turned off for starting configuration (mainly for edge case where busted wire box is not fixed)
        UpdatePowerState();
    }

    /// <summary>
    /// Called by the power switch to switch the switch's toggle state.
    /// </summary>
    public void FlipPowerSwitch()
    {
        // Precondition: can only flip power switch if there is one that exists
        if (_switchIdentifier is null)
            throw new System.Exception("Cannot call FlipPowerSwitch function! This PoweredLight has no corresponding switch.");

        _isSwitchOn = !_isSwitchOn;

        // Modifies game manager data
        if ((_isSwitchOn && !_isSwitchOffByDefault) || (!_isSwitchOn && _isSwitchOffByDefault)) // account for flipped state
            GameManager.Instance.SceneData.PowerSwitches.Remove(_switchIdentifier); // remove from list of off-switches
        else
            GameManager.Instance.SceneData.PowerSwitches.Add(_switchIdentifier); // add to list of off-switches

        UpdatePowerState();
    }

    public void UpdatePowerState()
    {
        if (IsPowered()) // only enable element if ALL power conditions are also met
            EnablePoweredElement();
        else
            DisablePoweredElement();
    }

    /// <summary>
    /// Returns whether the current element is powered or not.
    /// Useful for darkness checks (for the creature).
    /// </summary>
    /// <returns></returns>
    public bool IsPowered()
    {
        // Requirements for an element to receive power:
        // (1) Power Switch on (through light switch)
        // (2) Zone Power toggled on (through terminal) - if any
        // (3) Associated busted wire box is repaired   - if any
        return _isSwitchOn && _isZoneOn && (_wireBoxIdentifier.Equals("") || GameManager.Instance.SceneData.FixedWireBoxes.Contains(_wireBoxIdentifier));
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
