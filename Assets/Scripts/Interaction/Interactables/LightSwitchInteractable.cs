using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchInteractable : Interactable
{
    [SerializeField, Tooltip("Associated light")] private PoweredLight _light;
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {

    }

    public override void InteractEffects()
    {
        _light.FlipPowerSwitch();
    }
}