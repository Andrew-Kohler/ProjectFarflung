using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles interfacing between bobbing UI toggle and saved game manager data.
/// </summary>
public class BobbingToggle : MonoBehaviour
{
    [SerializeField, Tooltip("Used to configure initial state of toggle to match saved value")]
    private Toggle _bobbingToggle;
    [SerializeField, Tooltip("Used to update text display to match stored state.")]
    private TextMeshProUGUI _displayText;

    private void Awake()
    {
        // match toggle initial value to saved value
        _bobbingToggle.SetIsOnWithoutNotify(GameManager.Instance.OptionsData.CameraBobbing);
        _displayText.text = GameManager.Instance.OptionsData.CameraBobbing ? "on" : "off";
    }

    /// <summary>
    /// Called when toggle value changes to update saved value
    /// </summary>
    public void UpdateToggleValue()
    {
        GameManager.Instance.OptionsData.CameraBobbing = _bobbingToggle.isOn;
        _displayText.text = GameManager.Instance.OptionsData.CameraBobbing ? "on" : "off";
    }
}
