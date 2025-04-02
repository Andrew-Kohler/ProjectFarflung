using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls enabling/disabling of flashlight with key presses.
/// Tracks battery level and rotation of lights to follow player movement.
/// </summary>
public class FlashlightController : MonoBehaviour
{
    [Header("Functionality Tuning")]
    [SerializeField, Tooltip("Maximum duration of flashlight from full charge (in seconds)")]
    private float _maxChargeDuration;
    [SerializeField, Tooltip("Intensity with which light lags behind camera rotation.")]
    private float _lightPivotSharpness;
    
    [Header("Stun Functionality")]
    [SerializeField, Tooltip("Duration of holding the key before the flashlight stun blast will occur.")]
    private float _stunHoldDuration;
    [SerializeField, Tooltip("Duration that the stun works for. Should be SHORTER than stun hold duration.")]
    private float _stunDuration;
    [SerializeField, Tooltip("range of light components during flash mode.")]
    private float _stunLightRange;
    [SerializeField, Tooltip("amount of battery consumed on stun use. Full battery is 1.")]
    private float _stunBatteryCost;

    [Header("Lights / References")]
    [SerializeField, Tooltip("Used to enable/disable actual left light element.")]
    private Light _light;
    [SerializeField, Tooltip("Used to rotate left light.")]
    private GameObject _lightPivot;
    [SerializeField, Tooltip("Used to enable/disable light stun trigger when light is on.")]
    private Collider _stunTrigger;
    [SerializeField, Tooltip("Used to get vertical flashlight angle.")]
    private GameObject _cameraRoot;

    private Quaternion _prevPivotRot;
    private float _defaultLightRange;

    private void Awake()
    {
        _defaultLightRange = _light.range;
    }

    #region CONTROLS
    private bool _isOn = false;

    private bool _isHeld = false;
    private float _heldTimer = 0f;

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
    /// Initial press of flashlight key.
    /// Plays click SFX for initial button press
    /// </summary>
    private void FlashlightClick(InputAction.CallbackContext context)
    {
        // start timer for stun blast
        _isHeld = true;
        _heldTimer = 0f;

        // TODO: SFX for button initial press
    }

    /// <summary>
    /// Release of flashlight key.
    /// Enables flashlight functionally on release of button press
    /// </summary>
    private void ToggleFlashlight(InputAction.CallbackContext context)
    {
        // do NOT turn light off on release if we just stun blasted
        if (_heldTimer > _stunHoldDuration)
            return;

        // indicates player is NOT holding down key for stun
        _isHeld = false;

        // turning flashlight off
        if (_isOn)
        {
            _isOn = false;
            _light.enabled = false;
        }
        // turning flashlight on
        // prevent one frame flicker of flashlight (due to new coroutine for flashlight shut off)
        else if (!_isOn && GameManager.FlashlightCharge > 0)
        {
            _isOn = true;
            _light.enabled = true;
        }

        // TODO: SFX for button release
    }
    #endregion

    private void Start()
    {
        _prevPivotRot = _lightPivot.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // decrease battery charge
        if (_isOn)
        {
            GameManager.FlashlightCharge -= (1f/_maxChargeDuration) * Time.deltaTime;

            // running out of charge
            if (GameManager.FlashlightCharge < 0)
            {
                GameManager.FlashlightCharge = 0;

                StartCoroutine(DoFlashlightForceOff());
            }
        }

        // Delayed rotation of lights

        // necessary so previous local can be fetched in terms of current forward
        _lightPivot.transform.rotation = _prevPivotRot;
        // goal is always 0, 0, 0 (forward) PLUS VERTICAL ANGLE
        Quaternion goal = Quaternion.Euler(_cameraRoot.transform.rotation.eulerAngles.x, 0, 0);
        // lerp between previous angle (in terms of current local frame)
        Quaternion newRot = Quaternion.Lerp(_lightPivot.transform.localRotation, goal, 1f - Mathf.Exp(-_lightPivotSharpness * Time.deltaTime));
        // update local rotations
        _lightPivot.transform.localRotation = newRot;
        // save pivot for next frame
        _prevPivotRot = _lightPivot.transform.rotation;

        // check for stun burst
        // must be on already for stun holding charge to work
        if (_isHeld && _isOn)
        {
            // activate burst
            if (_heldTimer > _stunHoldDuration)
            {
                _light.range = _stunLightRange;
                _stunTrigger.enabled = true;

                _isHeld = false;

                // consume charge
                GameManager.FlashlightCharge -= _stunBatteryCost;
                if (GameManager.FlashlightCharge < 0)
                    GameManager.FlashlightCharge = 0;

                StartCoroutine(DoReturnToNormal());
            }

            // get closer to activating burst next frame
            _heldTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Returns flashlight range to normal after delay.
    /// </summary>
    private IEnumerator DoReturnToNormal()
    {
        yield return new WaitForSeconds(_stunDuration);

        _light.range = _defaultLightRange;
        _stunTrigger.enabled = false;
    }

    /// <summary>
    /// Returns flashlight to off state due to out of battery.
    /// In case of a stun flash, it allows the flash to play out fully before actually disabling the light.
    /// </summary>
    private IEnumerator DoFlashlightForceOff()
    {
        yield return new WaitUntil(IsStunInactive);

        _isOn = false;
        _light.enabled = false;
    }

    private bool IsStunInactive()
    {
        return !_stunTrigger.enabled;
    }
}
