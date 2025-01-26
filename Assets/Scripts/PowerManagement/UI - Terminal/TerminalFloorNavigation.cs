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
    [Header("Power System")]
    [Tooltip("Scene reference to the power system for retrieving data to display.")]
    public PowerSystem PowerSystem;

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

    private int _currFloor = 0;

    private void Start()
    {
        if (_floors.Length != 3)
            throw new System.Exception("There MUST be 3 floors for floor navigation or it is improperly configured.");

        // set button states accordingly
        if (_currFloor == 0)
            _downButton.interactable = false;
        else if (_currFloor == 2)
            _upButton.interactable = false;

        // activate floor sections accordingly
        _floors[0].SetActive(false);
        _floors[1].SetActive(false);
        _floors[2].SetActive(false);
        _floors[_currFloor].SetActive(true);

        // initial text
        _floorText.text = "" + (_currFloor + 1) + "F";
    }

    /// <summary>
    /// Called by up navigation button.
    /// </summary>
    public void UpFloor()
    {
        // Precondition: CANNOT go up from 2
        if (_currFloor >= 2)
            throw new System.Exception("Cannot navigate up 1 floor when already on top floor. Player should not be able to do this.");

        // enable / disable floors
        _floors[_currFloor].SetActive(false);
        _currFloor++;
        _floors[_currFloor].SetActive(true);

        // enable / disable buttons
        _downButton.interactable = true; // no matter what
        if (_currFloor == 2)
            _upButton.interactable = false;

        // set text
        _floorText.text = "" + (_currFloor + 1) + "F";
    }

    /// <summary>
    /// Called by down navigation button.
    /// </summary>
    public void DownFloor()
    {
        // Precondition: CANNOT go up from 2
        if (_currFloor <= 0)
            throw new System.Exception("Cannot navigate down 1 floor when already on bottom floor. Player should not be able to do this.");

        // enable / disable floors
        _floors[_currFloor].SetActive(false);
        _currFloor--;
        _floors[_currFloor].SetActive(true);

        // enable / disable buttons
        _upButton.interactable = true; // no matter what
        if (_currFloor == 0)
            _downButton.interactable = false;

        // set text
        _floorText.text = "" + (_currFloor + 1) + "F";
    }
}
