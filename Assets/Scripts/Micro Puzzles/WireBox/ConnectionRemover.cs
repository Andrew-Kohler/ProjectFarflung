using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality for removing a placed connection
/// </summary>
public class ConnectionRemover : ClickableObject
{
    private Renderer _renderer;
    private GameObject _wireSelector;

    private void Awake()
    {
        // Precondition: child of proper container
        if (transform.parent is null || transform.parent.gameObject.name != "Wire Connection(Clone)")
            throw new System.Exception("Incorrect Connection Prefab, wire model (and ConnectionRemover) MUST be a child of Wire Connection empty.");

        // Precondition: renderer component
        if (!TryGetComponent(out _renderer))
            throw new System.Exception("Wire Connection MUST have Renderer component.");
    }

    /// <summary>
    /// Assigns relevant references so the connection can track associated wire segment AND material
    /// </summary>
    public void Initialize(WireSelector correspondingWire)
    {
        _wireSelector = correspondingWire.gameObject;

        // match wire to corresponding material
        if (!correspondingWire.TryGetComponent(out Renderer wireRenderer))
            throw new System.Exception("Incorrect Wire Selector Configuration. Must have a renderer component with corresponding material.");
        _renderer.material = wireRenderer.material;
    }

    public override void OnObjectClick()
    {
        // re-enable wire on wire board
        _wireSelector.SetActive(true);

        // destroy both the model AND its container
        Destroy(transform.parent.gameObject);
    }
}