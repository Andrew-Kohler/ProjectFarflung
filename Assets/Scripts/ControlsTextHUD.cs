using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ControlsTextHUD : MonoBehaviour
{
    [SerializeField, Tooltip("Used to update text")]
    private TextMeshProUGUI _text;
    [SerializeField, Tooltip("Scenario of use (0=default, 1=tab)")]
    private int _scenario = 0;
    [SerializeField, Tooltip("Used to read action bindings")]
    private InputActionReference _actions;

    private void OnEnable()
    {
        switch (_scenario)
        {
            case 0:
                string action0 = ReadAction();
                _text.text = action0.ToUpper();
                break;
            case 1:
                string action1 = ReadAction();
                _text.text = "PRESS " + action1.ToUpper() + " TO NAVIGATE INTERFACE";
                break;
            default:
                _text.text = "Wuh oh";
                break;
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
