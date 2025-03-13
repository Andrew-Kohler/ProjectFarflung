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
    private Light[] _lights;

    protected override void DisablePoweredElement()
    {
        foreach (Light elem in _lights)
            elem.enabled = false;
    }

    protected override void EnablePoweredElement()
    {
        foreach (Light elem in _lights)
            elem.enabled = true;
    }
}
