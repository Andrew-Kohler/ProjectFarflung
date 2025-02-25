using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweredDoor : PoweredElement
{
    [Header("Door Interactables")]
    [SerializeField, Tooltip("Door panel components for enabling/disabling door interactions.")]
    private List<DoorInteractable> doorPanels;

    protected override void DisablePoweredElement()
    {
        for(int i = 0; i < doorPanels.Count; i++)
        {
            doorPanels[i].enabled = false;
        }
    }

    protected override void EnablePoweredElement()
    {
        for (int i = 0; i < doorPanels.Count; i++)
        {
            doorPanels[i].enabled = true;
        }
    }
}
