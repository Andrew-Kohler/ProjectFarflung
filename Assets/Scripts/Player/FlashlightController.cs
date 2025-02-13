using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls enabling/disabling of flashlights with key presses.
/// Tracks battery level and rotation of lights to follow player movement.
/// </summary>
public class FlashlightController : MonoBehaviour
{
    #region CONTROLS
    [Header("Controls")]
    [SerializeField, Tooltip("Used to enable/disable actual left light element.")]
    private Light _leftLight;
    [SerializeField, Tooltip("Used to enable/disable actual right light element.")]
    private Light _rightLight;

    private bool _isOn = false;

    private void OnEnable()
    {
        InputSystem.actions.FindAction("ToggleFlashlight").started += FlashlightClick;
        InputSystem.actions.FindAction("ToggleFlashlight").canceled += ToggleFlashlight;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("ToggleFlashlight").started -= FlashlightClick;
        InputSystem.actions.FindAction("ToggleFlashlight").canceled -= ToggleFlashlight;
    }

    /// <summary>
    /// Plays click SFX for initial button press
    /// </summary>
    private void FlashlightClick(InputAction.CallbackContext context)
    {
        // TODO: SFX for button initial press
    }

    /// <summary>
    /// Enables flashlight functionally on release of button press
    /// </summary>
    private void ToggleFlashlight(InputAction.CallbackContext context)
    {
        // functionally toggle light states
        _isOn = !_isOn;
        _leftLight.enabled = _isOn;
        _rightLight.enabled = _isOn;

        // TODO: SFX for button release
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
