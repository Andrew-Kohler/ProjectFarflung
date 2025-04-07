using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class WireBoxInteractable : Interactable
{
    [Header("Wire Box Specifics")]
    [SerializeField, Tooltip("The box")]
    private WireBoxHandler _wirebox;
    [SerializeField, Tooltip("The wire manager")]
    private WireManager _wireManager;
    [SerializeField, Tooltip("The node manager")]
    private NodeManager _nodeManager;
    [SerializeField, Tooltip("The camera aimed at the box")]
    private CinemachineVirtualCamera _wireboxCam;
    [SerializeField, Tooltip("The box animator")]
    private Animator _wireboxAnim;

    private CinemachineVirtualCamera _mainCam;
    private bool _inUse = false;        // If player uses this wirebox (to prevent constantly reactivating exit keybind)
    private BoxCollider _col;
    private bool _lockColState = false;

    private bool _initialInteractiongOngoing = false; // Used to lock the player out of interrupting the animation

    public static event OnLockedInteraction onLockedInteractionWirebox;

    #region Controls Bindings
    private void OnEnable()
    {
        InputSystem.actions.FindAction("Interact").started += ContextReenablePlayer;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Interact").started -= ContextReenablePlayer;
    }

    private void ContextReenablePlayer(InputAction.CallbackContext context)
    {
        ReenablePlayer(false);
    }
    #endregion

    new void Start()
    {
        if (GameManager.Instance.SceneData.FixedWireBoxes.Contains(_wirebox.IdentifierName))
        {
            Destroy(gameObject);
        }

        base.Start();

        _wireboxAnim.speed = 0f;
        _col = GetComponent<BoxCollider>();
    }

    new void Update()
    {
        // do not process enable/disable wire box interactable logic if currently in the box
        if (_lockColState)
            return;

        // disable wire box if wire box is not in a lit zone
        if (!_wirebox.LightZone.IsPowered())
            _col.enabled = false;
        else
            _col.enabled = true;
    }

    public override void InteractEffects()
    {
        StartCoroutine(DoWireboxInteract());
    }

    public void ReenablePlayer(bool final)
    {
        if (_inUse && !_initialInteractiongOngoing)
        {
            StartCoroutine(DoReenablePlayer(final));
        }
    }

    private IEnumerator DoWireboxInteract()
    {
        _initialInteractiongOngoing = true;
        if (_mainCam == null)
        {
            _mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        }

        _wireboxAnim.ResetTrigger("BreakerOpen");
        _wireboxAnim.ResetTrigger("BreakerClose");

        HideVFX(); // Hide the interaction VFX

        GameManager.Instance.PlayerEnabled = false; // Disable the player

        Cursor.visible = true;                      // Free the cursor
        Cursor.lockState = CursorLockMode.None;

        onLockedInteractionWirebox?.Invoke(true); // Invoke the event to let the HUD know what's up

        // Configure the camera
        _wireboxCam.gameObject.SetActive(true);
        _mainCam.gameObject.SetActive(false);

        _col.enabled = false;
        _lockColState = true; // prevent collider from being re-enabled

        _wireboxAnim.speed = 1f;
        _wireboxAnim.SetTrigger("BreakerOpen"); // Play the animation

        yield return new WaitForEndOfFrame();

        _inUse = true;  // Tell the box it's being used

        yield return new WaitForSeconds(1.5f);
        // delay ensures text does not enable clipping through box lid early
        _wirebox.EnablePuzzle(); // Enable the puzzle

        yield return new WaitForSeconds(1.2f);
        _initialInteractiongOngoing = false;
    }

    private IEnumerator DoReenablePlayer(bool final)
    {
        // disable puzzle when it is closed
        _wirebox.DisablePuzzle();

        _wireboxAnim.SetTrigger("BreakerClose");

        _mainCam.gameObject.SetActive(true);
        _wireboxCam.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        onLockedInteractionWirebox?.Invoke(false);

        _inUse = false;

        if (_wireManager.GetSelectedWire() != null) // Deselect a wire, if one is held
            _wireManager.DeselectWire(_wireManager.GetSelectedWire());

        _nodeManager.DestroyCurrentWire();

        yield return new WaitForEndOfFrame();
        GameManager.Instance.PlayerEnabled = true;

        yield return new WaitForSeconds(1.5f);        // Only reenable interaction after the animation ends
        if (!final)
        {
            _col.enabled = true;
            _lockColState = false; // allow collider properly update to match light zone again
        }

        /*yield return new WaitForSeconds(2f);
        if (final)  // If the player completed the puzzle, they shan't do it again
            Destroy(gameObject);*/
    }

}
