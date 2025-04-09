using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles range remapping, functionally interacting with the game manager, and visually displaying brightness value
/// </summary>
public class BrightnessSlider : MonoBehaviour
{
    [Header("Brightness Bounds")]
    [SerializeField, Tooltip("Minimum possible configurable brightness.")]
    private float _minBrightness;
    [SerializeField, Tooltip("Maximum possible configurable brightness.")]
    private float _maxBrightness;

    [Header("References")]
    [SerializeField, Tooltip("Used to read/set value of slider.")]
    private Slider _slider;
    [SerializeField, Tooltip("Used to update display text.")]
    private TextMeshProUGUI _displayText;

    private void Awake()
    {
        // configure initial slider pos  
        float initVal = InverseRemapNonlinear(GameManager.Instance.OptionsData.Brightness);
        _slider.SetValueWithoutNotify(initVal);

        // text initialization
        _displayText.text = GameManager.Instance.OptionsData.Brightness.ToString("#0.00");
    }

    private void Update()
    {
        // ensure slider updates if values modified from outside source (i.e. reset to defaults button)
        if (GameManager.Instance.OptionsData.Brightness.ToString("#0.00") != RemapNonlinear(_slider.value).ToString("#0.00"))
        {
            _slider.SetValueWithoutNotify(InverseRemapNonlinear(GameManager.Instance.OptionsData.Brightness));
            _displayText.text = RemapNonlinear(_slider.value).ToString("#0.00");
        }
    }

    /// <summary>
    /// Called by slider change event. Interfaces with game manager based on remapped value.
    /// </summary>
    public void UpdateBrightness()
    {
        // read value / remap
        float newBrightness = RemapNonlinear(_slider.value);

        // update game manager
        GameManager.Instance.OptionsData.Brightness = newBrightness;

        // Slider Click SFX - only when a visible change actually occurs
        if (_displayText.text != newBrightness.ToString("#0.00"))
            AudioManager.Instance.PlaySliderClick();

        // format display to two decimal places
        _displayText.text = newBrightness.ToString("#0.00");
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
        return (sliderVal * sliderVal * (_maxBrightness - _minBrightness)) + _minBrightness;
    }

    /// <summary>
    /// Remaps brightness range to slider value.
    /// Used for initial configuration of slider.
    /// </summary>
    private float InverseRemapNonlinear(float brightnessVal)
    {
        // appropriate inverse square root function
        return Mathf.Sqrt((brightnessVal - _minBrightness) / (_maxBrightness - _minBrightness));
    }

    /// <summary>
    /// Called by the reset brightness button.
    /// </summary>
    public void ResetBrightness()
    {
        // Click UI SFX
        AudioManager.Instance.PlayClickUI();

        GameManager.Instance.OptionsData.ResetBrightness();
    }
}
