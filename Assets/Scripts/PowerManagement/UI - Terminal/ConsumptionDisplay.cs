using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles reading and displaying consumption and max consumption data for an individual zone region on the terminal interface.
/// </summary>
public class ConsumptionDisplay : MonoBehaviour
{
    [Header("Data Retrieval")]
    [SerializeField, Tooltip("Used to retrieve the power system and read data.")]
    private TerminalFloorNavigation _terminal;
    [SerializeField, Tooltip("Used to determine the index of the current zone without duplicate inspector fields.")]
    TerminalZoneToggle _zone;

    [Header("Data Display")]
    [SerializeField, Tooltip("Used to set display text.")]
    private TextMeshProUGUI _textDisplay;

    private int _currPower = 0;
    private PoweredZone _poweredZone;

    // Start is called before the first frame update
    void Start()
    {
        // retrieve power zone
        _poweredZone = _terminal.PowerSystem.PoweredZones[_zone.ZoneIndex];

        UpdateDisplay();

        _currPower = _poweredZone.GetCurrentConsumption();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle updates to the display - only when a change occurs
        int newPower = _poweredZone.GetCurrentConsumption();
        if (newPower != _currPower)
        {
            UpdateDisplay();
            _currPower = newPower;
        }
    }

    /// <summary>
    /// Updates text of display to match Current Consumption / Max Consumption for the corresponding region.
    /// </summary>
    private void UpdateDisplay()
    {
        _textDisplay.text = "" + _poweredZone.GetCurrentConsumption() + "\n" + _poweredZone.GetMaxConsumption();
    }
}
