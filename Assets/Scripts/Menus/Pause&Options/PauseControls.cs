using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles functionality for pausing controls (via Escape key).
/// Also contains pause menu button functionality.
/// </summary>
public class PauseControls : MonoBehaviour
{
    [Header("Quit Button")]
    [SerializeField, Tooltip("Scene name to return to upon pressing quit")]
    private string _startSceneName;
    [SerializeField, Tooltip("used to actually call scene transitions")]
    private SceneTransitionHandler _transitionHandler;

    [Header("Object References")]
    [SerializeField, Tooltip("Used to enable/disable pause menu.")]
    private GameObject _pauseMenu;
    [SerializeField, Tooltip("Used to enable/disable options menu.")]
    private GameObject _optionsMenu;
    [SerializeField, Tooltip("Used to enable/disable quit confirmation popup.")]
    private GameObject _quitConfirmationPopup;

    private bool _isPaused = false; // never paused at scene start

    private InputActionMap _actionMap;

    #region Pause Toggling
    private void OnEnable()
    {
        _actionMap = InputSystem.actions.FindActionMap("Player");

        InputSystem.actions.FindAction("Escape").started += TogglePause;
        InputSystem.actions.FindAction("Escape").Enable();
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Escape").started -= TogglePause;
        InputSystem.actions.FindAction("Escape").Disable();
    }

    /// <summary>
    /// Handles both resume and pause functionality.
    /// </summary>
    private void TogglePause(InputAction.CallbackContext context)
    {
        // Resume
        if (_isPaused)
        {
            Resume();
        }
        // Pause
        else
        {
            _pauseMenu.SetActive(true);

            // free control over the mouse
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;
            _actionMap.Disable();

            // TODO: pause controls and time scale

            _isPaused = true;
        }
    }
    #endregion

    #region Pause Menu Buttons
    /// <summary>
    /// Closes pause interface and resumes normal gameplay.
    /// </summary>
    public void Resume()
    {
        _pauseMenu.SetActive(false);
        _optionsMenu.SetActive(false); // ensure options also closes if that was opened (i.e. closed from Escape press)

        // lock mouse back to center screen for first-person controls
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;
        _actionMap.Enable();
        
        // TODO: resume controls and time scale

        _isPaused = false;
    }

    /// <summary>
    /// closes pause interface, opens options interface.
    /// </summary>
    public void ToOptions()
    {
        _pauseMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Shows confirmation popup for quitting.
    /// </summary>
    public void TryQuit()
    {
        _quitConfirmationPopup.SetActive(true);
    }

    /// <summary>
    /// Cancels confirmation popup for quitting.
    /// </summary>
    public void CancelQuit()
    {
        _quitConfirmationPopup.SetActive(false);
    }

    /// <summary>
    /// Returns to start menu.
    /// </summary>
    public void ConfirmQuit()
    {
        // ensure transition is able to occur
        Time.timeScale = 1;
        // ensure player cannot re-pause during scene transition
        InputSystem.actions.FindAction("Escape").Disable();

        _transitionHandler.LoadScene(_startSceneName);
    }
    #endregion
}
