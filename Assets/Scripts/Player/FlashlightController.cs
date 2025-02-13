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
    [Header("Functionality Tuning")]
    [SerializeField, Tooltip("Maximum duration of flashlight from full charge (in seconds)")]
    private float _maxChargeDuration;

    [Header("Lights / References")]
    [SerializeField, Tooltip("Used to enable/disable actual left light element.")]
    private Light _leftLight;
    [SerializeField, Tooltip("Used to rotate left light.")]
    private GameObject _leftLightPivot;
    [SerializeField, Tooltip("Used to enable/disable actual right light element.")]
    private Light _rightLight;
    [SerializeField, Tooltip("Used to rotate right light.")]
    private GameObject _rightLightPivot;

    #region CONTROLS
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

    // Update is called once per frame
    void Update()
    {
        // decrease battery charge
        if (_isOn)
        {
            GameManager.BatteryCharge -= (1f/_maxChargeDuration) * Time.deltaTime;

            // running out of charge
            if (GameManager.BatteryCharge < 0)
            {
                GameManager.BatteryCharge = 0;
                _isOn = false;
                _leftLight.enabled = false;
                _rightLight.enabled = false;
            }
        }
    }
}
