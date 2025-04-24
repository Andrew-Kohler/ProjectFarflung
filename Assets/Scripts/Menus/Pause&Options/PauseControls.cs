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
    [Header("Configuration")]
    [SerializeField, Tooltip("Whether this is being used in the StartMenu - i.e. pause controls are disable.")]
    private bool _isMainMenu = false;

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

    // Used to ensure that controls text updates when the pause menu is exited
    // also used to reduce music/ambient volumes while paused
    public delegate void OnPauseOpen();
    public static event OnPauseOpen onPauseOpen;
    public delegate void OnPauseClose();
    public static event OnPauseClose onPauseClose;

    #region Pause Toggling
    private void OnEnable()
    {
        // remove pause controls component in main menu since no pausing should be permitted
        if (_isMainMenu)
        {
            Destroy(this);
            return; // avoid still assigning the below keybindings
        }

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
        // Block pausing control during load-in of new scene
        // if it was allowed, then the UI of pause menu would be unclickable
        if (!_transitionHandler.IsDoneEnter())
            return;

        // Resume
        if (_isPaused)
        {
            Resume();
        }
        // Pause
        else
        {
            // click UI SFX
            AudioManager.Instance.PlayClickUI();

            _pauseMenu.SetActive(true);

            // free control over the mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0;
            _actionMap.Disable();

            _isPaused = true;

            onPauseOpen?.Invoke();
        }
    }
    #endregion

    #region Pause Menu Buttons
    /// <summary>
    /// Closes pause interface and resumes normal gameplay.
    /// </summary>
    public void Resume()
    {
        // click UI SFX
        AudioManager.Instance.PlayClickUI();

        onPauseClose?.Invoke();
        _pauseMenu.SetActive(false);
        _optionsMenu.SetActive(false); // ensure options also closes if that was opened (i.e. closed from Escape press)

        // lock mouse back to center screen for first-person controls
        if (GameManager.Instance.PlayerEnabled || !GameManager.Instance.SceneData.IntroCutsceneWatched)  // Enabled check currently used as a shorthand for if a player is in a locked interaction (terminal, wirebox)
            Cursor.lockState = CursorLockMode.Locked;

        // resume controls and time scale
        Time.timeScale = 1;
        _actionMap.Enable();

        _isPaused = false;
    }

    /// <summary>
    /// closes pause interface, opens options interface.
    /// </summary>
    public void ToOptions()
    {
        _pauseMenu.SetActive(false);
        _optionsMenu.SetActive(true);

        // click UI SFX
        AudioManager.Instance.PlayClickUI();
    }

    /// <summary>
    /// Shows confirmation popup for quitting.
    /// </summary>
    public void TryQuit()
    {
        _quitConfirmationPopup.SetActive(true);

        // click UI SFX
        AudioManager.Instance.PlayClickUI();
    }

    /// <summary>
    /// Cancels confirmation popup for quitting.
    /// </summary>
    public void CancelQuit()
    {
        _quitConfirmationPopup.SetActive(false);

        // click UI SFX
        AudioManager.Instance.PlayClickUI();
    }

    /// <summary>
    /// Returns to start menu.
    /// </summary>
    public void ConfirmQuit()
    {
        // click UI SFX
        AudioManager.Instance.PlayClickUI();

        // ensure transition is able to occur
        Time.timeScale = 1;
        // ensure player cannot re-pause during scene transition
        InputSystem.actions.FindAction("Escape").Disable();

        _transitionHandler.LoadScene(_startSceneName);
    }
    #endregion
}
