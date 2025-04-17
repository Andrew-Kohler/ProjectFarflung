using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Handles navigation between different options tabs and back to the pause menu.
/// </summary>
public class OptionsNavigation : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Whether the current scene is the start menu - makes back button close EVERYTHING instead of return to pause.")]
    private bool _isMainMenu = false;

    [Header("Object References")]
    [SerializeField, Tooltip("Used to re-enable pause menu with back button.")]
    private GameObject _pauseMenuObject;
    [SerializeField, Tooltip("Used to enable/disable entire options menu.")]
    private GameObject _optionsMenuObject;
    [SerializeField, Tooltip("Used to enable/disable controls tab.")]
    private GameObject _controlsTabObject;
    [SerializeField, Tooltip("Used to enable/disable volume tab.")]
    private GameObject _volumeTabObject;
    [SerializeField, Tooltip("Used to enable/disable visuals tab.")]
    private GameObject _visualsTabObject;

    [Header("Toggles")]
    [SerializeField, Tooltip("Used to reset toggle configuration when closed.")]
    private Toggle _controlsToggle;
    [SerializeField, Tooltip("Used to reset toggle configuration when closed.")]
    private Toggle _volumeToggle;
    [SerializeField, Tooltip("Used to reset toggle configuration when closed.")]
    private Toggle _visualsToggle;

    // Start is called before the first frame update
    void Awake()
    {
        // controls always open by default
        // cannot simply call ToControls() cause then ClickUI SFX would play on start
        FunctionalToControls();
    }

    private void OnEnable()
    {
        // in main menu escape key does NOTHING
        if (!_isMainMenu)
        {
            // Ensure always starts on controls tab whenever re-opening options menu
            InputSystem.actions.FindAction("Escape").started += ContextToControls;
            InputSystem.actions.FindAction("Escape").Enable();
        }
    }

    private void OnDisable()
    {
        // in main menu escape key does NOTHING
        if (!_isMainMenu)
        {
            InputSystem.actions.FindAction("Escape").started -= ContextToControls;
            InputSystem.actions.FindAction("Escape").Disable();
        }
    }

    private void ContextToControls(InputAction.CallbackContext context)
    {
        FunctionalToControls();
    }

    #region Button Functions
    /// <summary>
    /// Enables controls tab. Disables all others.
    /// </summary>
    public void ToControls()
    {
        // click UI SFX (only play if not already on new tab)
        if (!_controlsTabObject.activeSelf)
            AudioManager.Instance.PlayClickUI();

        FunctionalToControls();
    }

    /// <summary>
    /// Handles functionality of swapping to controls WITHOUT SFX
    /// </summary>
    private void FunctionalToControls()
    {
        // return toggles to controls tab appropriately when closing out
        _controlsToggle.SetIsOnWithoutNotify(true);
        _volumeToggle.SetIsOnWithoutNotify(false);
        _visualsToggle.SetIsOnWithoutNotify(false);

        _controlsTabObject.SetActive(true);
        _volumeTabObject.SetActive(false);
        _visualsTabObject.SetActive(false);
    }

    /// <summary>
    /// Enables volume tab. Disables all others.
    /// </summary>
    public void ToVolume()
    {
        // click UI SFX (only play if not already on new tab)
        if (!_volumeTabObject.activeSelf)
            AudioManager.Instance.PlayClickUI();

        _controlsTabObject.SetActive(false);
        _volumeTabObject.SetActive(true);
        _visualsTabObject.SetActive(false);
    }

    /// <summary>
    /// Enables volume tab. Disables all others.
    /// </summary>
    public void ToVisuals()
    {
        // click UI SFX (only play if not already on new tab)
        if (!_visualsTabObject.activeSelf)
            AudioManager.Instance.PlayClickUI();

        _controlsTabObject.SetActive(false);
        _volumeTabObject.SetActive(false);
        _visualsTabObject.SetActive(true);
    }

    /// <summary>
    /// Closes options and opens pause menu.
    /// Returns now hidden options to default controls tab.
    /// </summary>
    public void BackToPause()
    {
        // click UI SFX
        AudioManager.Instance.PlayClickUI();

        if (_isMainMenu)
        {
            _pauseMenuObject.SetActive(false);
            _optionsMenuObject.SetActive(false);
        }
        else
        {
            _pauseMenuObject.SetActive(true);
            _optionsMenuObject.SetActive(false);
        }

        FunctionalToControls(); // return to default controls enabled state
    }
    #endregion
}
