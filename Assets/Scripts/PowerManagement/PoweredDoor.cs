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
    [SerializeField, Tooltip("Whether the door is open on scene start")]
    private bool IsOpen;
    [SerializeField, Tooltip("Whether the door is broken (beyond any power or electrical repair)")]
    private bool IsBroken;
    [SerializeField, Tooltip("Distance the player needs to go from a door before it autocloses")]
    private float _doorCloseDistance = 6f;

    [Header("References")]
    [SerializeField, Tooltip("Used to replace door with broken variant.")]
    private GameObject _brokenDoorPrefab;

    private void Awake()
    {
        // replace door with BrokenOpenDoorPrefab
        if (IsOpen && IsBroken)
        {
            // spawn new broken door at exact same location as current door
            Instantiate(_brokenDoorPrefab, transform.position, transform.rotation, transform.parent);

            Destroy(this.gameObject);
        }
    }

    public void InitializeInteractable() 
    {
        // Set the key type on each of the door panels
        for (int i = 0; i < doorPanels.Count; i++)
        {
            doorPanels[i].RequiredKey = this.RequiredKey;
            doorPanels[i].IsOpen = this.IsOpen;
            doorPanels[i].IsBroken = this.IsBroken;
            doorPanels[i].DoorCloseDistance = this._doorCloseDistance;
        }
    }

    protected override void DisablePoweredElement()
    {
        // does nothing
        // interactor script reads powered state directly to determine functionality
    }

    protected override void EnablePoweredElement()
    {
        // does nothing
        // interactor script reads powered state directly to determine functionality
    }
}
