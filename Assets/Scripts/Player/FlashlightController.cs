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
    [SerializeField, Tooltip("Intensity with which light lags behind camera rotation.")]
    private float _lightPivotSharpness;

    [Header("Lights / References")]
    [SerializeField, Tooltip("Used to enable/disable actual left light element.")]
    private Light _leftLight;
    [SerializeField, Tooltip("Used to rotate left light.")]
    private GameObject _leftLightPivot;
    [SerializeField, Tooltip("Used to enable/disable actual right light element.")]
    private Light _rightLight;
    [SerializeField, Tooltip("Used to rotate right light.")]
    private GameObject _rightLightPivot;
    [SerializeField, Tooltip("Used to enable/disable light stun trigger when light is on.")]
    private Collider _stunTrigger;

    private Quaternion _prevPivotRot;

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
        _stunTrigger.enabled = _isOn;

        // TODO: SFX for button release
    }
    #endregion

    private void Start()
    {
        _prevPivotRot = _leftLightPivot.transform.rotation;
    }

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
                _stunTrigger.enabled = false;
            }
        }

        // Delayed rotation of lights

        // necessary so previous local can be fetched in terms of current forward
        _leftLightPivot.transform.rotation = _prevPivotRot;
        // goal is always 0, 0, 0 (forward)
        Quaternion goal = Quaternion.identity;
        // lerp between previous angle (in terms of current local frame)
        Quaternion newRot = Quaternion.Lerp(_leftLightPivot.transform.localRotation, goal, 1f - Mathf.Exp(-_lightPivotSharpness * Time.deltaTime));
        // update local rotations
        _leftLightPivot.transform.localRotation = newRot;
        _rightLightPivot.transform.localRotation = newRot;
        // save pivot for next frame
        _prevPivotRot = _leftLightPivot.transform.rotation;
    }
}
