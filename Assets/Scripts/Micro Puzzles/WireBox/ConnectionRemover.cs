using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality for removing a placed connection
/// </summary>
public class ConnectionRemover : ClickableObject
{
    private bool _isConnected = false;

    private void Awake()
    {
        // Precondition: child of proper container
        if (transform.parent is null || transform.parent.gameObject.name != "Wire Connection(Clone)")
            throw new System.Exception("Incorrect Connection Prefab, wire model (and ConnectionRemover) MUST be a child of Wire Connection empty.");
    }

    public override void OnObjectClick()
    {
        // only process clicks once the connection has been made
        if (!_isConnected)
            return;

        // destroy both the model AND its container
        Destroy(transform.parent.gameObject);

        // TODO: re-populate wire on wire board on left
    }

    public void Connect()
    {
        _isConnected = true;
    }
}
