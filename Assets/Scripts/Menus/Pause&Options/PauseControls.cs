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

    [Header("Menu Tabs")]
    [SerializeField, Tooltip("Used to enable/disable pause menu.")]
    private GameObject _pauseMenu;
    [SerializeField, Tooltip("Used to enable/disable options menu.")]
    private GameObject _optionsMenu;

    private bool _isPaused = false; // never paused at scene start

    #region Pause Toggling
    private void OnEnable()
    {
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

        // lock mouse back to center screen for first-person controls
        Cursor.lockState = CursorLockMode.Locked;

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
    /// Returns to start menu.
    /// </summary>
    public void Quit()
    {
        _transitionHandler.LoadScene(_startSceneName);
    }
    #endregion
}
