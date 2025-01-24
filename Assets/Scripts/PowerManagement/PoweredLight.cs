using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality pertaining to a light element that can be toggled on/off in realtime.
/// </summary>
public class PoweredLight : PoweredElement
{
    [SerializeField, Tooltip("Index of the light switch corresponding with the PoweredLight. -1 if there is no switch.")]
    private int SwitchIndex = -1;
    [SerializeField, Tooltip("Light component for enabling/disabling light source.")]
    private Light _light;

    private bool _isSwitchOn;
    private bool _isZoneOn;

    private void Awake()
    {
        // Precondition: must be an appropriate switch index in range of list (or -1)
        if (SwitchIndex < -1 || SwitchIndex >= GameManager.Instance.SceneData.PowerSwitches.Length)
            throw new System.Exception("Invalid Switch Index: MUST be either -1 or within the range of the Power Switches game manager list.");

        // Determine starting switch state
        _isSwitchOn = true; // on by default, unless overriden by switch state
        if (SwitchIndex != -1)
        {
            _isSwitchOn = GameManager.Instance.SceneData.PowerSwitches[SwitchIndex];
        }
    }

    override public void PowerDownZone()
    {
        _isZoneOn = false;
        // light turns off no matter what
        _light.enabled = false;
    }

    override public void PowerUpZone()
    {
        _isZoneOn = true;

        if (_isSwitchOn) // only enable light if switch is ALSO on
            _light.enabled = true;
    }

    override public void FlipPowerSwitch()
    {
        // Precondition: can only flip power switch if there is one that exists
        if (SwitchIndex == -1)
            throw new System.Exception("Cannot call FlipPowerSwitch function! This PoweredLight has no corresponding switch.");

        _isSwitchOn = !_isSwitchOn;

        // turn on light if zone was already on
        if (_isSwitchOn && _isZoneOn)
            _light.enabled = true;
    }
}
