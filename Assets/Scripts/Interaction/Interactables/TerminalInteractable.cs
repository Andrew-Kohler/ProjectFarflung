using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TerminalInteractable : Interactable
{
    [SerializeField, Tooltip("The terminal")] 
    private TerminalConfiguration _terminal;
    [SerializeField, Tooltip("The camera aimed at the terminal interface")]
    private CinemachineVirtualCamera _terminalCam;
    [SerializeField, Tooltip("The camera aimed at the light puzzle")]
    private CinemachineVirtualCamera _puzzleCam;

    private CinemachineVirtualCamera _mainCam;
    private bool _isBeingUsed; // If the terminal is being used (to determine when the exit keybind is valid)

    // Delegate events
    public delegate void PlayerActiveStateChange(bool state);
    public static event PlayerActiveStateChange playerActiveStateChange;
    new void Start()
    {
        base.Start();
        _mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        
    }
    //TODO
    // Let the cursor actually move
    // Turn Off the player
    public override void InteractEffects()
    {
        // Turn the player's movement off, show the cursor, and switch the main camera off
        playerActiveStateChange?.Invoke(false);
        _mainCam.gameObject.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex]) // If this terminal is unlocked
        {
            _terminalCam.gameObject.SetActive(true);
        }
        else // If it isn't
        {
            _puzzleCam.gameObject.SetActive(true);
        }
    }

}
