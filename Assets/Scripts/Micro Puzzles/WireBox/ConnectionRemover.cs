using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality for removing a placed connection
/// </summary>
public class ConnectionRemover : ClickableObject
{
    private WireSelector _wireSelector;
    private NodeSelector[] _connectedNodes; // used to properly unassign their references to each other on destroy

    private Outline _outline;
    private float _outlineWidth;

    private void Awake()
    {
        // Precondition: child of proper container
        if (transform.parent is null || transform.parent.gameObject.name != "Wire Connection(Clone)")
            throw new System.Exception("Incorrect Connection Prefab, wire model (and ConnectionRemover) MUST be a child of Wire Connection empty.");
    }

    /// <summary>
    /// Assigns relevant references so the connection can track associated wire segment AND material
    /// </summary>
    public void Initialize(WireSelector correspondingWire, NodeSelector node1, NodeSelector node2, float outlineWidth, Color hoverColor)
    {
        _wireSelector = correspondingWire;
        _connectedNodes = new NodeSelector[2];
        _connectedNodes[0] = node1;
        _connectedNodes[1] = node2;

        _outlineWidth = outlineWidth;
        // configure outline component
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineWidth = 0; // no outline by default
        _outline.OutlineColor = hoverColor; // never changes to any other color
    }

    public override void OnObjectClick()
    {
        // re-enable wire on wire board
        _wireSelector.gameObject.SetActive(true);
        _wireSelector.StuckTape.SetActive(true);
        _wireSelector.UnstuckTape.SetActive(false);

        // remove node connections for connection processing
        _connectedNodes[0].RemoveConnection(_connectedNodes[1]);
        _connectedNodes[1].RemoveConnection(_connectedNodes[0]);

        // wire remove SFX
        AudioManager.Instance.PlayRemoveZap();

        // destroy both the model AND its container
        Destroy(transform.parent.gameObject);
    }

    public override void OnObjectHover()
    {
        _outline.OutlineWidth = _outlineWidth;
    }

    public override void OnObjectUnhover()
    {
        _outline.OutlineWidth = 0;
    }
}