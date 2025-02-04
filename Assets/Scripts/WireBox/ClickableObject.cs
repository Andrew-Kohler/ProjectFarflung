using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles base functionality for all clickable objects, including hit detection and not click implementation
/// </summary>
public abstract class ClickableObject : MonoBehaviour
{
    // Update is called once per frame
    protected virtual void Update()
    {
        // TODO: update this to new input system once Andrew's Player HUD PR is merged in
        if (Input.GetMouseButtonDown(0) && gameObject == GetClickedObject())
        {
            OnObjectClick();
        }
    }

    /// <summary>
    /// Returns the object clicked by the mouse
    /// </summary>
    /// <returns></returns>
    private GameObject GetClickedObject()
    {
        GameObject target = null;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO: shift to new input system
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
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
