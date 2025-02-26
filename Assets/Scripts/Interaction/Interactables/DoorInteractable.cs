using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [Header("General Controls")]
    [Tooltip("Sibling interactable (switch on the other side of the door)")]
    public DoorInteractable Sibling;
    [Tooltip("Door element")]
    public PoweredDoor Element; // Door powered element
    [Tooltip("Door animator")]
    public Animator DoorAnim;
    [Tooltip("Open state of the door")]
    public bool IsOpen;   // If the door is open, you can't double open it, silly
    [SerializeField, Tooltip("If the door is broken (will not open AT ALL)")]
    public bool IsBroken;

    // If a door is:
    // CLOSED and BROKEN:       This is never getting opened, ever.
    // CLOSED and NOT BROKEN:   Pretty normal door, honestly.
    // OPEN and BROKEN:         This is never closing, ever.
    // OPEN and NOT BROKEN:     Will close after a while, why would you do this?

    private float _doorCloseDistance = 6f;
    public bool IsActiveDoorCoroutine;
    public PoweredDoor.KeyType RequiredKey;
    
    
    new void Start()
    {
        base.Start();

        // Safety checks
        if (Element is null)
        {
            throw new System.Exception("Door cannot exist without being a PoweredDoor.");
        }

        // Set the default state of the door
        DoorAnim.speed = 1f;
        if (IsOpen)
            DoorAnim.Play("Take 001", 0, .5f);
        else
            DoorAnim.Play("Take 001", 0, 0);
        DoorAnim.speed = 0f;
    }

    new void Update()
    {
        // Update keeps track of player position and closes the door when they get too far away
        if (IsOpen && !IsActiveDoorCoroutine)
        {
            if(Vector3.Distance(this.transform.position, _camTransform.position) > _doorCloseDistance)
            {
                StartCoroutine(DoDoorChange());
            }
        }
    }

    public override void InteractEffects()
    {
        if (Element.IsPowered() && !IsBroken && !IsActiveDoorCoroutine) // If the zone of the door is powered and not deus ex machina disabled
        {
            if (RequiredKey.ToString() == "Default" || GameManager.Instance.SceneData.Keys.Contains(RequiredKey.ToString())) // If they have the key
            {
                StartCoroutine(DoDoorChange()); // Change the door state
            }
        }
        else
        {
            // Place for SFX in the future
        }
    }

    // Plays the door animation, and ensures that the door never tries to open & close at the same time, double open, etc.
    private IEnumerator DoDoorChange()
    {
        IsActiveDoorCoroutine = true;
        Sibling.IsActiveDoorCoroutine = true;
        DoorAnim.speed = 1f;
        if (!IsOpen)
            DoorAnim.Play("Take 001", 0, 0);
        else
            DoorAnim.Play("Take 001", 0, .5f);

        IsOpen = !IsOpen;
        Sibling.IsOpen = IsOpen; // Make sure both doors keep the same open state

        yield return new WaitForSeconds(1.667f / 2);
        DoorAnim.speed = 0f;
        IsActiveDoorCoroutine = false;
        Sibling.IsActiveDoorCoroutine = false;

    }


}
