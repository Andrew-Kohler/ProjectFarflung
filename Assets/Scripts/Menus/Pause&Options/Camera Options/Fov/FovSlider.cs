using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles configuration and game manager interfacing for the FoV slider in the Camera Tab of the options menu.
/// </summary>
public class FovSlider : MonoBehaviour
{
    [SerializeField, Tooltip("Used to configure initial value of slider to match saved options.")]
    private Slider _slider;
    [SerializeField, Tooltip("Used to update visual text to match current slider/data value.")]
    private TextMeshProUGUI _displayText;

    private void Awake()
    {
        // initial configuration to match save data
        _slider.SetValueWithoutNotify(GameManager.Instance.OptionsData.FoV);
        _displayText.text = _slider.value.ToString("00.");
    }

    private void Update()
    {
        // ensure slider updates if values modified from outside source (i.e. reset to defaults button)
        if (GameManager.Instance.OptionsData.FoV != Mathf.RoundToInt(_slider.value))
        {
            _slider.SetValueWithoutNotify(GameManager.Instance.OptionsData.FoV);
            _displayText.text = _slider.value.ToString("00.");
        }
    }

    /// <summary>
    /// Called when the slider value is changed to update the saved values to match
    /// </summary>
    public void UpdateSavedVolume()
    {
        // update saved value and text
        GameManager.Instance.OptionsData.FoV = Mathf.RoundToInt(_slider.value);

        // Slider Click SFX - only when a visual change occurs
        if (_displayText.text != _slider.value.ToString("00."))
            AudioManager.Instance.PlaySliderClick();

        _displayText.text = _slider.value.ToString("00.");
    }

    /// <summary>
    /// Called by reset FoV button.
    /// </summary>
    public void ResetFoV()
    {
        // Click UI SFX
        AudioManager.Instance.PlayClickUI();

        GameManager.Instance.OptionsData.ResetFoV();
    }
}
