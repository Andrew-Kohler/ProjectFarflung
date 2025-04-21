using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [Header("General Controls")]
    [Tooltip("Sibling interactable (switch on the other side of the door)")]
    public DoorInteractable Sibling;
    [HideInInspector]
    public BoxCollider Col; // Collider the player interacts with
    [Tooltip("Door element")]
    public PoweredDoor Element; // Door powered element
    [Tooltip("Door animator")]
    public Animator DoorAnim;
    [HideInInspector, Tooltip("Open state of the door")]
    public bool IsOpen;   // If the door is open, you can't double open it, silly
    [HideInInspector, Tooltip("If the door is broken (will not open AT ALL)")]
    public bool IsBroken;

    [Header("Display of Opening Requirements")]
    [SerializeField, Tooltip("Sprite for displaying which keycard is needed")]
    private SpriteRenderer _requiredKeycardSprite;
    [SerializeField, Tooltip("Sprite for displaying if electrics are in order")]
    private SpriteRenderer _elecOperableSprite;
    [SerializeField, Tooltip("Graphic information associated with keycards")]
    public List<Sprite> _keycardInfoSprites;
    [SerializeField, Tooltip("Image for electricity working")]
    private Sprite _elecOperableYes;
    [SerializeField, Tooltip("Image for electricity not working")]
    private Sprite _elecOperableNo;
    [SerializeField, Tooltip("Parent of indicator sprites")]
    private GameObject _indicatorParent;
    [SerializeField, Tooltip("Parent of E to interact")]
    private GameObject _interactPromptParent;

    // If a door is:
    // CLOSED and BROKEN:       This is never getting opened, ever.
    // CLOSED and NOT BROKEN:   Pretty normal door, honestly.
    // OPEN and BROKEN:         This is never closing, ever.
    // OPEN and NOT BROKEN:     Will close after a while, why would you do this?

    [HideInInspector]
    public float DoorCloseDistance;
    [HideInInspector]
    public bool IsActiveDoorCoroutine;
    [HideInInspector]
    public PoweredDoor.KeyType RequiredKey;
    
    
    new void Start()
    {
        base.Start();

        // Safety checks
        if (Element is null)
        {
            throw new System.Exception("Door cannot exist without being a PoweredDoor.");
        }

        // ensure that all values are properly identical to those stored in the PoweredDoor (keycard, isOpen, isBroken, closeDistance)
        // calling this here ensures 100% consistency regardless of start function execution order
        Element.InitializeInteractable();

        // Properly display the required key for this door
        switch (RequiredKey)
        {
            case PoweredDoor.KeyType.Default:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[0];
                break;
            case PoweredDoor.KeyType.Security:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[1];
                break;
            case PoweredDoor.KeyType.Janitor:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[2];
                break;
            case PoweredDoor.KeyType.Cargo:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[3];
                break;
            case PoweredDoor.KeyType.Engineering:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[4];
                break;
            case PoweredDoor.KeyType.Research:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[5];
                break;
            case PoweredDoor.KeyType.Medical:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[6];
                break;
            case PoweredDoor.KeyType.Command:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[7];
                break;
            case PoweredDoor.KeyType.MedCloset:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[8];
                break;
            case PoweredDoor.KeyType.DoNotDisturbBypass:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[9];
                break;
            case PoweredDoor.KeyType.ResearchDumbwaiter:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[10];
                break;
            case PoweredDoor.KeyType.NuclearGenerator:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[11];
                break;
            default:
                _requiredKeycardSprite.sprite = _keycardInfoSprites[0];
                break;
        }


        Col = this.GetComponent<BoxCollider>();

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
            if(Vector3.Distance(this.transform.position, _camTransform.position) > DoorCloseDistance)
            {
                Col.enabled = true;
                Sibling.Col.enabled = true;
                StartCoroutine(DoDoorChange());

                // door close SFX
                AudioManager.Instance.PlayDoorClose();
            }
        }

        // Updates the visual indicators of whether you can open the door
        IndicatorMonitor();
    }

    public override void InteractEffects()
    {
        if (Element.IsPowered() && !IsBroken && !IsActiveDoorCoroutine && !IsOpen) // If the zone of the door is powered and not deus ex machina disabled
        {
            if (RequiredKey.ToString() == "Default" || GameManager.Instance.SceneData.Keys.Contains(RequiredKey.ToString())) // If they have the key
            {
                Col.enabled = false;
                Sibling.Col.enabled = false;
                StartCoroutine(DoDoorChange()); // Change the door state

                // Open Door SFX
                AudioManager.Instance.PlayDoorOpen();
            }
            else
            {
                // Door locked sfx (missing keycard)
                AudioManager.Instance.PlayDoorLocked();
            }
        }
        else
        {
            // Door locked sfx (no power)
            AudioManager.Instance.PlayDoorLocked();
        }
    }

    // Pays attention to the graphics that help the player understand what they need for a door
    private void IndicatorMonitor()
    {
        if (RequiredKey.ToString() == "Default" || GameManager.Instance.SceneData.Keys.Contains(RequiredKey.ToString())) // If they have the key
            _requiredKeycardSprite.color = Color.green;
        else
            _requiredKeycardSprite.color = Color.red;


        if (Element.IsPowered() && !IsBroken && !IsActiveDoorCoroutine && !IsOpen)
        {
            _elecOperableSprite.color = Color.green;
            _elecOperableSprite.sprite = _elecOperableYes;

            if (RequiredKey.ToString() == "Default" || GameManager.Instance.SceneData.Keys.Contains(RequiredKey.ToString()))
            {
                _indicatorParent.SetActive(false);  // If both conditions are a yes, there's no need for this to be enabled
                _interactPromptParent.SetActive(true);
            }

            else
            {
                _indicatorParent.SetActive(true);
                _interactPromptParent.SetActive(false);
            }
                

        }
        else
        {
            _indicatorParent.SetActive(true);   // no power, so there will be an indicator
            _interactPromptParent.SetActive(false); // no longer able to interact (power was turned off)

            _elecOperableSprite.color = Color.red;
            _elecOperableSprite.sprite = _elecOperableNo;
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
