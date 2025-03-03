using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class TerminalInteractable : Interactable
{
    [SerializeField, Tooltip("The terminal")] 
    private TerminalConfiguration _terminal;
    [SerializeField, Tooltip("The camera aimed at the terminal interface")]
    private CinemachineVirtualCamera _terminalCam;
    [SerializeField, Tooltip("The camera aimed at the light puzzle")]
    private CinemachineVirtualCamera _puzzleCam;

    private CinemachineVirtualCamera _mainCam;
    private bool _inUse = false;        // If player uses this terminal (to prevent constantly reactivating exit keybind)
    private bool _lockedOnUse = false;  // If the player uses this terminal and it's initially locked

    public static event OnLockedInteraction onLockedInteractionTerminal;

    private void OnEnable()
    {
        InputSystem.actions.FindAction("Interact").started += context => ReenablePlayer();
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Interact").started -= context => ReenablePlayer();
    }

    new void Start()
    {
        base.Start();
        _mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    }

    new void Update()
    {
        if (_lockedOnUse)
        {
            if (GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex])
            {
                _lockedOnUse = false;
                _terminalCam.gameObject.SetActive(true);
                _puzzleCam.gameObject.SetActive(false);
                
            }
        }
    }

    public override void InteractEffects()
    {
        HideVFX(); // Hide the interaction VFX

        GameManager.Instance.PlayerEnabled = false; // Disable the player

        Cursor.visible = true;                      // Free the cursor
        Cursor.lockState = CursorLockMode.None;

        _inUse = true;  // Tell the terminal it's being used

        onLockedInteractionTerminal?.Invoke(true); // Invoke the event to let the HUD know what's up

        // Configure the camera
        _mainCam.gameObject.SetActive(false);
        if (GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex]) // If this terminal is unlocked
        {
            _terminalCam.gameObject.SetActive(true);
        }
        else // If it isn't
        {
            _puzzleCam.gameObject.SetActive(true);
            _lockedOnUse = true;
        }
    }

    private void ReenablePlayer()
    {
        if (_inUse)
        {
            StartCoroutine(DoReenablePlayer());
        }
    }

    private IEnumerator DoReenablePlayer()
    {
        _mainCam.gameObject.SetActive(true);
        _puzzleCam.gameObject.SetActive(false);
        _terminalCam.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        onLockedInteractionTerminal?.Invoke(false);

        _inUse = false;

        yield return new WaitForEndOfFrame();
        GameManager.Instance.PlayerEnabled = true;
    }

}
