using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles base functionality for all clickable objects, including hit detection and not click implementation
/// </summary>
public abstract class ClickableObject : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        InputSystem.actions.FindAction("MousePress").started += CheckForClick;
    }

    protected virtual void OnDisable()
    {
        InputSystem.actions.FindAction("MousePress").started -= CheckForClick;
    }

    /// <summary>
    /// Fetches object that was clicked and compares it to game object
    /// </summary>
    private void CheckForClick(InputAction.CallbackContext context)
    {
        if (gameObject == GetClickedObject())
        {
            OnObjectClick();
        }
    }

    /// <summary>
    /// Returns the object clicked by the mouse
    /// </summary>
    private GameObject GetClickedObject()
    {
        GameObject target = null;
        Vector3 mousePos = InputSystem.actions.FindAction("MousePosition").ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out RaycastHit hit))
        {
            target = hit.collider.gameObject;
        }
        return target;
    }

    /// <summary>
    /// Functionality for when the object is actually clicked.
    /// </summary>
    public abstract void OnObjectClick();
}
