using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles displaying current consumption of entire power system AND capacity of grid.
/// Also handles grid shutdown when maximum capacity is exceeded.
/// </summary>
public class CapacityDisplay : MonoBehaviour
{
    [Header("Data Retrieval")]
    [SerializeField, Tooltip("Used to retrieve the power system and read data.")]
    private TerminalFloorNavigation _terminal;

    [Header("Data Display")]
    [SerializeField, Tooltip("Used to set display text.")]
    private TextMeshProUGUI _textDisplay;

    private int _currPower = 0;
    private PowerSystem _powerSystem;

    // Start is called before the first frame update
    void Start()
    {
        // retrieve power system
        _powerSystem = _terminal.PowerSystem;

        UpdateDisplay();

        _currPower = _powerSystem.GetCurrentConsumption();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle updates to the display - only when a change occurs
        int newPower = _powerSystem.GetCurrentConsumption();
        if (newPower != _currPower)
        {
            // check for grid shutdown
            if (newPower > _powerSystem.GetCapacity())
            {
                _powerSystem.ShutdownGrid();
                newPower = _powerSystem.GetCurrentConsumption(); // ensure stored power is updated to the new power after shutdown
            }

            // update display and curr power tracker
            UpdateDisplay();
            _currPower = newPower;
        }
    }

    /// <summary>
    /// Updates text of display to match Current Consumption / Capacity for the entire power grid.
    /// </summary>
    private void UpdateDisplay()
    {
        _textDisplay.text = "" + _powerSystem.GetCurrentConsumption() + "\n" + _powerSystem.GetCapacity();
    }
}
