using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField, Tooltip("Door element")]
    private PoweredDoor _element;
    [SerializeField, Tooltip("Door animator")]
    private Animator _doorAnim;
    [SerializeField, Tooltip("The key associated with this door (if none, leave blank)")] 
    private string _requiredKey;
    [SerializeField, Tooltip("Open state of the door")]
    private bool _isOpen;   // If the door is open, you can't double open it, silly
    [SerializeField, Tooltip("If the door is broken (will not open AT ALL)")]
    private bool _isBroken;

    // If a door is:
        // CLOSED and BROKEN:       This is never getting opened, ever.
        // CLOSED and NOT BROKEN:   Pretty normal door, honestly.
        // OPEN and BROKEN:         This is never closing, ever.
        // OPEN and NOT BROKEN:     Will close after a while, why would you do this?

    
    
    
    void Start()
    {
        if(_element is null)
        {
            throw new System.Exception("Door cannot exist without being a PoweredDoor.");
        }

        // Set the default state of the door
        _doorAnim.speed = 1f;
        if (_isOpen)
            _doorAnim.Play("Take 001", 0, .5f);
        else
            _doorAnim.Play("Take 001", 0, 0);
        _doorAnim.speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO
    // Door needs to close on its own after a little

    public override void InteractEffects()
    {
        if (_element.IsPowered() && !_isBroken) // If the zone of the door is powered and not deus ex machina disabled
        {
            if (_requiredKey == "" || GameManager.Instance.SceneData.Keys.Contains(_requiredKey)) // If they have the key
            {
                StartCoroutine(DoDoorChange()); // Change the door state
            }
        }
        else
        {
            // Place for SFX in the future
        }
        
    }

    private IEnumerator DoDoorChange()
    {
        _doorAnim.speed = 1f;
        if (_isOpen)
            _doorAnim.Play("Take 001", 0, 0);
        else
            _doorAnim.Play("Take 001", 0, .5f);

        yield return new WaitForSeconds(1.667f / 2);
        _doorAnim.speed = 0f;
        _isOpen = !_isOpen;
    }


}
