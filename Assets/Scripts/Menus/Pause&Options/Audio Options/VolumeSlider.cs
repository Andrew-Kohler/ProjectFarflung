using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles initial configuration and data modification interfacing with the game manager for volume options data.
/// Game Manager stores values 0-1 (float), slider stores values 0-100 (int).
/// </summary>
public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType
    {
        Main,
        Music,
        SFX,
        AudioLog
    }

    [SerializeField, Tooltip("Type of volume the current slider controls.")]
    private VolumeType _type;
    [SerializeField, Tooltip("Used to configure initial value of slider to match saved options.")]
    private Slider _slider;
    [SerializeField, Tooltip("Used to update visual text to match current slider/data value.")]
    private TextMeshProUGUI _displayText;

    private void Awake()
    {
        // match saved values on scene start
        switch (_type)
        {
            case VolumeType.Main:
                _slider.SetValueWithoutNotify(Mathf.RoundToInt(GameManager.Instance.OptionsData.MainVolume * 100));
                break;
            case VolumeType.Music:
                _slider.SetValueWithoutNotify(Mathf.RoundToInt(GameManager.Instance.OptionsData.MusicVolume * 100));
                break;
            case VolumeType.SFX:
                _slider.SetValueWithoutNotify(Mathf.RoundToInt(GameManager.Instance.OptionsData.SFXVolume * 100));
                break;
            case VolumeType.AudioLog:
                _slider.SetValueWithoutNotify(Mathf.RoundToInt(GameManager.Instance.OptionsData.LogVolume * 100));
                break;
        }

        // same formula for all types
        _displayText.text = _slider.value.ToString("##0.") + "%";
    }

    private void Update()
    {
        // ensure slider updates if values modified from outside source (i.e. reset to defaults button)
        int currVal = 0;
        switch (_type)
        {
            case VolumeType.Main:
                currVal = Mathf.RoundToInt(GameManager.Instance.OptionsData.MainVolume * 100);
                break;
            case VolumeType.Music:
                currVal = Mathf.RoundToInt(GameManager.Instance.OptionsData.MusicVolume * 100);
                break;
            case VolumeType.SFX:
                currVal = Mathf.RoundToInt(GameManager.Instance.OptionsData.SFXVolume * 100);
                break;
            case VolumeType.AudioLog:
                currVal = Mathf.RoundToInt(GameManager.Instance.OptionsData.LogVolume * 100);
                break;
            default:
                throw new System.Exception("Something is wrong with a volume slider, how is it none of the available volume types??");
        }

        // same formula for all types, if a change occurred
        if (currVal != Mathf.RoundToInt(_slider.value))
        {
            _slider.SetValueWithoutNotify(currVal);
            _displayText.text = _slider.value.ToString("##0.") + "%";
        }
        
    }

    /// <summary>
    /// Called when the slider value is changed to update the saved values to match
    /// </summary>
    public void UpdateSavedVolume()
    {
        switch (_type)
        {
            case VolumeType.Main:
                GameManager.Instance.OptionsData.MainVolume = _slider.value / 100f;
                break;
            case VolumeType.Music:
                GameManager.Instance.OptionsData.MusicVolume = _slider.value / 100f;
                break;
            case VolumeType.SFX:
                GameManager.Instance.OptionsData.SFXVolume = _slider.value / 100f;
                break;
            case VolumeType.AudioLog:
                GameManager.Instance.OptionsData.LogVolume = _slider.value / 100f;
                break;
        }

        // same formula for all types
        _displayText.text = _slider.value.ToString("##0.") + "%";
    }
}
