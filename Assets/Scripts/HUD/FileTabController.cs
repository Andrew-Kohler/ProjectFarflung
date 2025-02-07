using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FileTabController : MonoBehaviour
{

    private int currentIndex = 0;

    private InputAction _arrowAction;

    private void OnEnable()
    {
        InputSystem.actions.FindAction("Arrows").started += context => FileInteraction();
        // Put a method in here that refreshes the list
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Arrows").started -= context => FileInteraction();
    }
    void Start()
    {
        // Arrow keys
        _arrowAction = InputSystem.actions.FindAction("Arrows");
        _arrowAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Handles all arrow key inputs on the file tab.
    /// </summary>
    private void FileInteraction()
    {
        Vector2 arrowInput = _arrowAction.ReadValue<Vector2>();
        if (arrowInput.x > 0) // Moving the list to the right
        {

        }
        else if (arrowInput.x < 0) // Moving the list to the left
        {

        }

        else if (arrowInput.y > 0) // Closing a log
        {
           
        }
        else if (arrowInput.y < 0) // Opening a log
        {
           
        }
        
    }

    /// <summary>
    /// Refreshes the file list when the tab is enabled in case new logs were picked up.
    /// Called on enable.
    /// </summary>
    private void TabOpen()
    {

    }

    /// <summary>
    /// Makes sure no files are left 'open'.
    /// Called on disable.
    /// </summary>
    private void TabClose()
    {

    }
}
