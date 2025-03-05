using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

// To Do
// Reset wirebox if you back out of it halfway though the puzzle
// Figure out why the nodes don't work

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

    private CinemachineVirtualCamera _mainCam;
    private bool _inUse = false;        // If player uses this wirebox (to prevent constantly reactivating exit keybind)

    public static event OnLockedInteraction onLockedInteractionWirebox;

    private void OnEnable()
    {
        InputSystem.actions.FindAction("Interact").started += context => ReenablePlayer(false);
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Interact").started -= context => ReenablePlayer(false);
    }

    new void Start()
    {
        if (GameManager.Instance.SceneData.FixedWireBoxes.Contains(_wirebox.IdentifierName))
        {
            Destroy(gameObject);
        }

        base.Start();
        _mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    }

    new void Update()
    {

    }

    public override void InteractEffects()
    {
        StartCoroutine(DoWireboxInteract());
    }

    public void ReenablePlayer(bool final)
    {
        if (_inUse)
        {
            StartCoroutine(DoReenablePlayer(final));
        }
    }

    private IEnumerator DoWireboxInteract()
    {
        HideVFX(); // Hide the interaction VFX

        GameManager.Instance.PlayerEnabled = false; // Disable the player

        Cursor.visible = true;                      // Free the cursor
        Cursor.lockState = CursorLockMode.None;

        onLockedInteractionWirebox?.Invoke(true); // Invoke the event to let the HUD know what's up

        // Configure the camera
        _wireboxCam.gameObject.SetActive(true);
        _mainCam.gameObject.SetActive(false);

        _wirebox.EnablePuzzle(); // Enable the puzzle

        yield return new WaitForEndOfFrame();

        _inUse = true;  // Tell the box it's being used
    }

    private IEnumerator DoReenablePlayer(bool final)
    {
        _mainCam.gameObject.SetActive(true);
        _wireboxCam.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        onLockedInteractionWirebox?.Invoke(false);

        _inUse = false;

        if(_wireManager.GetSelectedWire() != null) // Deselect a wire, if one is held
            _wireManager.DeselectWire(_wireManager.GetSelectedWire());
        //_nodeManager.DeselectFirstNode();
        //_wirebox.DisablePuzzle(); // Disable the puzzle

        yield return new WaitForEndOfFrame();
        GameManager.Instance.PlayerEnabled = true;

        if (final)  // If the player completed the puzzle, they shan't do it again
            Destroy(gameObject);
    }

}
