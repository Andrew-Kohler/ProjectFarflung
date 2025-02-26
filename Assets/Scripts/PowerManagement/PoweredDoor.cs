using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweredDoor : PoweredElement
{
    [Header("Door Interactables")]
    [SerializeField, Tooltip("Door panel components for enabling/disabling door interactions.")]
    private List<DoorInteractable> doorPanels;
    [SerializeField, Tooltip("The key associated with this door (if none, leave Default)")]
    public enum KeyType { Default, Security, Janitor, Cargo, Engineering, Research, Medical, Command, MedCloset, DoNotDisturbBypass, ResearchDumbwaiter, NuclearGenerator }
    public KeyType RequiredKey;

    private void Start() // Set the key type on each of the door panels
    {
        for (int i = 0; i < doorPanels.Count; i++)
        {
            doorPanels[i].RequiredKey = this.RequiredKey;
        }
    }

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
