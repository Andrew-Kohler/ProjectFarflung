using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles Puzzle Customization for toggling series of lights in some configured pattern.
/// </summary>
public class LightPuzzleHandler : MonoBehaviour
{
    [Header("Puzzle Customization")]
    [SerializeField, Tooltip("Starting configurations of button before puzzle start.")]
    private bool[] _startConfiguration;
    [SerializeField, Tooltip("SEVEN digit Left side toggle pattern; counting to the left, false = do nothing, true = flip.")]
    private bool[] _leftPattern;
    [SerializeField, Tooltip("SEVEN digit Right side toggle pattern; counting to the right, false = do nothing, true = flip.")]
    private bool[] _rightPattern;

    [Header("References")]
    [SerializeField, Tooltip("Toggles to be flipped in patterns based on presses.")]
    private Toggle[] _toggles;
    [SerializeField, Tooltip("Used to access terminal index.")]
    private TerminalConfiguration _terminal;

    private void Awake()
    {
        // Precondition: appropriate number of toggles & toggle configuration
        if (_toggles.Length != 8 || _startConfiguration.Length != 8)
            throw new System.Exception("Invalid Light Puzzle Setup: there must be 8 toggles in current implementation");

        // Precondition: appropriate length patterns
        if (_leftPattern.Length != 7 || _rightPattern.Length != 7)
            throw new System.Exception("Invalid Light Puzzle Configuration: left & right patterns must EACH be SEVEN bools.");

        // configure toggles to start configuration
        if (GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex])
        {
            // already completed, all lights on
            for (int i = 0; i < _toggles.Length; i++)
                _toggles[i].SetIsOnWithoutNotify(true);
        }
        else
        {
            // not yet completed, go to puzzle start config
            for (int i = 0; i < _startConfiguration.Length; i++)
                _toggles[i].SetIsOnWithoutNotify(_startConfiguration[i]);
        }
    }

    /// <summary>
    /// Toggles buttons in a pattern centered around the button with input index.
    /// It is assumed that the pressed button toggles itself on press and is not needed to be flipped.
    /// </summary>
    public void TogglePattern(int buttonIndex)
    {
        // blocks functionality of buttons once puzzle has been completed
        if (GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex])
        {
            // undo the press of the current toggle
            _toggles[buttonIndex].isOn = true;
            return;
        }

        // light button press SFX
        AudioManager.Instance.PlayLightButtonPress();

        for (int i = 0; i < _toggles.Length; i++)
        {
            // apply left pattern
            if (i < buttonIndex)
            {
                // flip toggle based on pattern
                int leftPatternIndex = (_toggles.Length - 1) - (buttonIndex - i); // list range minus left distance from button index
                if (_leftPattern[leftPatternIndex])
                    _toggles[i].SetIsOnWithoutNotify(!_toggles[i].isOn);
            }
            // apply right pattern
            else if (i > buttonIndex)
            {
                int rightPatternIndex = i - buttonIndex - 1; // distance between toggles (minus 1 to start at 0)
                if (_rightPattern[rightPatternIndex])
                    _toggles[i].SetIsOnWithoutNotify(!_toggles[i].isOn);
            }
        }

        // check for successful puzzle completion
        bool isDone = true;
        foreach (Toggle toggle in _toggles)
        {
            if (!toggle.isOn)
            {
                isDone = false;
                break;
            }
        }
        // puzzle completion
        if (isDone)
        {
            // unlock terminal
            GameManager.Instance.SceneData.TerminalUnlocks[_terminal.ZoneIndex] = true;
            
            // TODO: transition camera from puzzle to screen now (using same lerp function to be made??)
        }
    }
}
