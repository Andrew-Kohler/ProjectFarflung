using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Minimum possible configurable sensitivity.")]
    private float _minSensitivity;
    [SerializeField, Tooltip("Maximum possible configurable sensitivity.")]
    private float _maxSensitivity;

    [SerializeField, Tooltip("Used to configure initial value of slider to match saved options.")]
    private Slider _slider;
    [SerializeField, Tooltip("Used to update visual text to match current slider/data value.")]
    private TextMeshProUGUI _displayText;

    private void Awake()
    {
        // initial configuration to match save data
        _slider.SetValueWithoutNotify(InverseRemapNonlinear(GameManager.Instance.OptionsData.Sensitivity));
        _displayText.text = RemapNonlinear(_slider.value).ToString("#0.00");
    }

    private void Update()
    {
        // ensure slider updates if values modified from outside source (i.e. reset to defaults button)
        if (GameManager.Instance.OptionsData.Sensitivity.ToString("#0.00") != RemapNonlinear(_slider.value).ToString("#0.00"))
        {
            _slider.SetValueWithoutNotify(InverseRemapNonlinear(GameManager.Instance.OptionsData.Sensitivity));
            _displayText.text = RemapNonlinear(_slider.value).ToString("#0.00");
        }
    }

    /// <summary>
    /// Called when the slider value is changed to update the saved values to match
    /// </summary>
    public void UpdateSavedVolume()
    {
        // update saved value and text
        GameManager.Instance.OptionsData.Sensitivity = RemapNonlinear(_slider.value);

        // Slider Click SFX
        AudioManager.Instance.PlaySliderClick();

        _displayText.text = RemapNonlinear(_slider.value).ToString("#0.00");
    }

    /// <summary>
    /// Returns non-linear remapping of value from [0,1] to [0.05, 5].
    /// </summary
    private float RemapNonlinear(float sliderVal)
    {
        // Precondition, input between 0 and 1
        if (sliderVal < 0 || sliderVal > 1)
            throw new System.Exception("Incorrect use of RemapNonlinear, must take an input between 0 and 1.");

        // square function on domain [0,1] weighs lower half more finely than upper half
        return (sliderVal * sliderVal * (_maxSensitivity - _minSensitivity)) + _minSensitivity;
    }

    /// <summary>
    /// Remaps brightness range to slider value.
    /// Used for initial configuration of slider.
    /// </summary>
    private float InverseRemapNonlinear(float brightnessVal)
    {
        // appropriate inverse square root function
        return Mathf.Sqrt((brightnessVal - _minSensitivity) / (_maxSensitivity - _minSensitivity));
    }

    /// <summary>
    /// Called by reset sensitivity button.
    /// </summary>
    public void ResetSensitivity()
    {
        // Click UI SFX
        AudioManager.Instance.PlayClickUI();

        GameManager.Instance.OptionsData.ResetSensitivity();
    }
}
