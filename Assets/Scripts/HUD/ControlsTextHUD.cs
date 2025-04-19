using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ControlsTextHUD : MonoBehaviour
{
    [SerializeField, Tooltip("Used to update text")]
    private TextMeshProUGUI _text;
    [SerializeField, Tooltip("Used to update text in worldspace")]
    private TextMeshPro _textWorld;
    [SerializeField, Tooltip("Whether this script is on a Canvas or World element")]
    private bool _isCanvas = true;
    [SerializeField, Tooltip("Scenario of use (0=default, 1=tab, 2=text scroll, 3=audio FFWD, 4=view log, 5=close log)")]
    private int _scenario = 0;
    [SerializeField, Tooltip("Used to read action bindings")]
    private InputActionReference _actions;

    private void OnEnable()
    {
        UpdateText();
        PauseControls.onPauseClose += UpdateText;
    }

    private void OnDisable()
    {
        PauseControls.onPauseClose -= UpdateText;
    }

    private void UpdateText()
    {
        string action = ReadAction();
        if (_isCanvas)
        {
            switch (_scenario)
            {
                case 0:
                    _text.text = action.ToUpper();
                    break;
                case 1:
                    _text.text = "PRESS " + action.ToUpper() + " TO NAVIGATE INTERFACE";
                    break;
                case 2:
                    _text.text = "PRESS " + action.ToUpper() + " TO SCROLL";
                    break;
                case 3:
                    _text.text = "HOLD " + action.ToUpper() + " FOR 1.5X PLAYBACK";
                    break;
                case 4:
                    _text.text = "PRESS " + action.ToUpper() + " TO VIEW";
                    break;
                case 5:
                    _text.text = "PRESS " + action.ToUpper() + " TO CLOSE";
                    break;
                case 6:
                    _text.text = "SUIT DISPLAY MINIMIZED * PRESS " + action.ToUpper() + " TO RESTORE DISPLAY";
                    break;
                case 7:
                    _text.text = "Press " + action.ToUpper() + " to toggle";
                    break;
                case 8:
                    _text.text = "DANGER: Hold " + action.ToUpper() + " to overclock";
                    break;
                default:
                    _text.text = "Wuh oh";
                    break;
            }
        }
        else
        {
            switch (_scenario)
            {
                case 0:
                    _textWorld.text = action.ToUpper();
                    break;
                default:
                    _text.text = "Wuh oh";
                    break;
            }
        }
        
    }

    /// <summary>
    /// Returns corresponding binding associated with action.
    /// Prioritizes first binding, and returns "Not Bound" if no bindiing found.
    /// </summary>
    private string ReadAction()
    {
        string binding1 = InputSystem.actions.FindAction(_actions.name).GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        string binding2 = InputSystem.actions.FindAction(_actions.name).GetBindingDisplayString(1, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        if (binding1 != "")
            return binding1;
        else if (binding2 != "")
            return binding2;
        else
            return "Unbound";
    }
}
