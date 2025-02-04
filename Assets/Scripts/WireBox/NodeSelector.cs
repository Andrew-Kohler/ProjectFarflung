using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles clicking functionality and data configuration for an individual wire node.
/// </summary>
public class NodeSelector : ClickableObject
{
    [Header("Visual")]
    [SerializeField, Tooltip("Thickness of selected outline, according to Outline.cs.")]
    private float _outlineWidth;

    [HideInInspector]
    public NodeSelector Connection1 = null;
    [HideInInspector]
    public NodeSelector Connection2 = null;

    private NodeManager _nodeManager;
    private Outline _outline;

    void Awake()
    {
        // Precondition: must be contained by NodeManager
        if (transform.parent is null || !transform.parent.TryGetComponent(out _nodeManager))
            throw new System.Exception("Incorrect node configuration. A NodeSelector MUST be a child of a NodeManager.");

        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineWidth = 0; // no outline by default
    }

    // Update is called once per frame
    override protected void Update()
    {
        // ensure base click check is handled
        base.Update();
    }

    public override void OnObjectClick()
    {
        // cannot connect more than two connections on a single node - skip processing
        if (Connection1 is not null && Connection2 is not null)
            return;

        _nodeManager.ProcessNodeClick(this);
    }

    /// <summary>
    /// Removes visual outline.
    /// </summary>
    public void DeselectVisual()
    {
        _outline.OutlineWidth = 0;
    }

    /// <summary>
    /// Shows visual outline.
    /// </summary>
    public void SelectVisual()
    {
        _outline.OutlineWidth = _outlineWidth;
    }
}
