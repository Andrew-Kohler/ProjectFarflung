using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class TerminalInteractable : Interactable
{
    [Header("Terminal Specifics")]
    [SerializeField, Tooltip("The terminal")] 
    private TerminalConfiguration _terminal;
    [SerializeField, Tooltip("The animator")]
    private Animator _terminalAnim;
    [SerializeField, Tooltip("The camera aimed at the terminal interface")]
    private CinemachineVirtualCamera _terminalCam;
    [SerializeField, Tooltip("The camera aimed at the light puzzle")]
    private CinemachineVirtualCamera _puzzleCam;

    private CinemachineVirtualCamera _mainCam;
    private bool _inUse = false;        // If player uses this terminal (to prevent constantly reactivating exit keybind)
    private bool _lockedOnUse = false;  // If the player uses this terminal and it's initially locked

    private bool _initialInteractiongOngoing = false; // Used to lock the player out of interrupting the animation

    private BoxCollider _col;

    public static event OnLockedInteraction onLockedInteractionTerminal;

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
        ReenablePlayer();
    }
    #endregion

    new void Start()
    {
        base.Start();
        
        _terminalAnim.speed = 0f;
        _col = GetComponent<BoxCollider>();
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

                // terminal boot SFX
                AudioManager.Instance.PlayTerminalBoot();
            }
        }
    }

    public override void InteractEffects()
    {
        StartCoroutine(DoTerminalInteract());
    }

    private void ReenablePlayer()
    {
        if (_inUse && !_initialInteractiongOngoing)
        {
            StartCoroutine(DoReenablePlayer());
        }
    }

    private IEnumerator DoTerminalInteract()
    {
        // Save data at terminal open
        GameManager.Instance.SaveAtTerminal(_terminal.ZoneIndex);

        // restore flashlight battery
        GameManager.FlashlightCharge = 1f;

        _initialInteractiongOngoing = true;
        if(_mainCam == null)
        {
            _mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        }

        _terminalAnim.ResetTrigger("TerminalStandby");
        _terminalAnim.ResetTrigger("TerminalWake");
        _terminalAnim.ResetTrigger("TerminalSleep");

        HideVFX(); // Hide the interaction VFX

        GameManager.Instance.PlayerEnabled = false; // Disable the player

        Cursor.visible = true;                      // Free the cursor
        Cursor.lockState = CursorLockMode.None;

        onLockedInteractionTerminal?.Invoke(true); // Invoke the event to let the HUD know what's up

        // Configure the camera
        _mainCam.gameObject.SetActive(false);
        if (GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex]) // If this terminal is unlocked
        {
            _terminalCam.gameObject.SetActive(true);

            // terminal boot SFX
            AudioManager.Instance.PlayTerminalBoot();
        }
        else // If it isn't
        {
            _puzzleCam.gameObject.SetActive(true);
            _lockedOnUse = true;
        }

        // terminal open SFX - for BOTH light puzzle and terminal opening
        AudioManager.Instance.PlayTerminalOpen();

        _terminalAnim.speed = 1f;
        _terminalAnim.SetTrigger("TerminalWake");

        yield return new WaitForEndOfFrame();

        _inUse = true;  // Tell the terminal it's being used

        _col.enabled = false;

        yield return new WaitForSeconds(1.875f);
        _terminalAnim.SetTrigger("TerminalStandby"); // Once the start animation has played, enter standby
        _initialInteractiongOngoing = false;
    }

    private IEnumerator DoReenablePlayer()
    {
        // Save data on terminal exit
        GameManager.Instance.SaveAtTerminal(_terminal.ZoneIndex);

        _mainCam.gameObject.SetActive(true);        // Right the cameras
        _puzzleCam.gameObject.SetActive(false);
        _terminalCam.gameObject.SetActive(false);

        
        Cursor.visible = false;                     // Right the cursor
        Cursor.lockState = CursorLockMode.Locked;

        onLockedInteractionTerminal?.Invoke(false); // Right the HUD

        _inUse = false;

        _terminalAnim.SetTrigger("TerminalStandby");    // Right the terminal
        _terminalAnim.SetTrigger("TerminalSleep");

        // terminal close SFX
        AudioManager.Instance.PlayTerminalClose();

        yield return new WaitForEndOfFrame();
        GameManager.Instance.PlayerEnabled = true;      // Free the player

        yield return new WaitForSeconds(2.25f);        // Only reenable interaction after the animation ends
        _col.enabled = true;
    }

}
