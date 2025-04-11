using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Handles functions for navigating up or down floors in the terminal interface.
/// Toggles floor groups and updates UI for floor navigation accordingly.
/// </summary>
public class TerminalFloorNavigation : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField, Tooltip("Used to access configured power system and zone index.")]
    private TerminalConfiguration _terminal;

    [Header("Buttons")]
    [SerializeField, Tooltip("Used to disable up when on top floor.")]
    private Button _upButton;
    [SerializeField, Tooltip("Used to disable down when on bottom floor.")]
    private Button _downButton;

    [Header("Text")]
    [SerializeField, Tooltip("Used to display current floor number.")]
    private TextMeshProUGUI _floorText;

    [Header("Floors")]
    [SerializeField, Tooltip("Used to enable/disable floor groupings appropriately.")]
    private GameObject[] _floors;

    private int _currUIFloor;

    private void Start()
    {
        // Precondition: must have 3 floors
        if (_floors.Length != 3)
            throw new System.Exception("There MUST be 3 floors for floor navigation or it is improperly configured.");

        // activate floor sections accordingly
        _floors[0].SetActive(false);
        _floors[1].SetActive(false);
        _floors[2].SetActive(false);
        // TERMINAL UI FLOOR NUMS: 0 = Floor 1, 1 = Floor 2, 2 = Floor 3
        // POWER SYSTEM FLOOR NUM: 0 = Hangar, 1 = Floor 1, 2 = Floor 2, 3 = Floor 3
        switch (_terminal.PowerSystem.FloorNum)
        {
            // floor 1
            case 1:
                _currUIFloor = 0; // floor 1
                _floors[_currUIFloor].SetActive(true);
                _downButton.interactable = false; // cannot go further down
                break;
            // floor 2 / Hangar
            case 0:
            case 2:
                _currUIFloor = 1; // floor 2
                _floors[_currUIFloor].SetActive(true);
                break;
            case 3:
                _currUIFloor = 2; // floor 3
                _floors[_currUIFloor].SetActive(true);
                _upButton.interactable = false;
                break;
        }

        // initial text
        _floorText.text = "" + (_currUIFloor + 1) + "F";
    }

    /// <summary>
    /// Called by up navigation button.
    /// </summary>
    public void UpFloor()
    {
        // Precondition: CANNOT go up from 2
        if (_currUIFloor >= 2)
            throw new System.Exception("Cannot navigate up 1 floor when already on top floor. Player should not be able to do this.");

        // terminal arrow SFX
        AudioManager.Instance.PlayTerminalArrow();

        // enable / disable floors
        _floors[_currUIFloor].SetActive(false);
        _currUIFloor++;
        _floors[_currUIFloor].SetActive(true);

        // enable / disable buttons
        _downButton.interactable = true; // no matter what
        if (_currUIFloor == 2)
            _upButton.interactable = false;

        // set text
        _floorText.text = "" + (_currUIFloor + 1) + "F";
    }

    /// <summary>
    /// Called by down navigation button.
    /// </summary>
    public void DownFloor()
    {
        // Precondition: CANNOT go up from 2
        if (_currUIFloor <= 0)
            throw new System.Exception("Cannot navigate down 1 floor when already on bottom floor. Player should not be able to do this.");

        // terminal arrow SFX
        AudioManager.Instance.PlayTerminalArrow();

        // enable / disable floors
        _floors[_currUIFloor].SetActive(false);
        _currUIFloor--;
        _floors[_currUIFloor].SetActive(true);

        // enable / disable buttons
        _upButton.interactable = true; // no matter what
        if (_currUIFloor == 0)
            _downButton.interactable = false;

        // set text
        _floorText.text = "" + (_currUIFloor + 1) + "F";
    }
}
