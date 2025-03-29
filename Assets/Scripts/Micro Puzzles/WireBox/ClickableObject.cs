using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles base functionality for all clickable objects, including hit detection and not click implementation
/// </summary>
public abstract class ClickableObject : MonoBehaviour
{
    private GameObject _target = null;

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
        if (gameObject == _target)
        {
            OnObjectClick();
        }
    }

    private void Update()
    {
        GameObject prevTarget = _target;

        Vector3 mousePos = InputSystem.actions.FindAction("MousePosition").ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out RaycastHit hit))
            _target = hit.collider.gameObject;
        else
            _target = null;

        // check for on hover and on unhover
        if (_target == gameObject && prevTarget != gameObject)
            OnObjectHover();
        else if (_target != gameObject && prevTarget == gameObject)
            OnObjectUnhover();
    }

    /// <summary>
    /// Functionality for when the object is actually clicked.
    /// </summary>
    public abstract void OnObjectClick();

    /// <summary>
    /// Functionality for when the object is hovered over but not yet clicked.
    /// </summary>
    public abstract void OnObjectHover();

    /// <summary>
    /// Functionality for when the object is no longer hovered but was last frame.
    /// </summary>
    public abstract void OnObjectUnhover();
}
