using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles implementation of functional light enabling and disabling.
/// </summary>
public class PoweredLight : PoweredElement
{
    [Header("Light")]
    [SerializeField, Tooltip("Light component for enabling/disabling light source.")]
    private Light _light;

    protected override void DisablePoweredElement()
    {
        _light.enabled = false;
    }

    protected override void EnablePoweredElement()
    {
        _light.enabled = true;
    }
}
