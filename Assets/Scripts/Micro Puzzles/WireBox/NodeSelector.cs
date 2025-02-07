using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles clicking functionality and data configuration for an individual wire node.
/// </summary>
public class NodeSelector : ClickableObject
{
    [Header("Customization")]
    [Tooltip("Voltage difference imposed on a wire connection crossing the node.")]
    public int VoltageDifference;

    [Header("Visuals")]
    [SerializeField, Tooltip("Thickness of selected outline, according to Outline.cs.")]
    private float _outlineWidth;
    [SerializeField, Tooltip("Used to configure text to match voltage customization.")]
    private TextMeshProUGUI _voltageText;
    [SerializeField, Tooltip("Used to set the material of the node.")]
    private Renderer _renderer;
    [SerializeField, Tooltip("Materials for different charge types of node. 0 = negative, 1 = neutral, 2 = positive")]
    private Material[] _nodeMaterials;

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

        // voltage text configuration (never changes)
        _voltageText.text = (VoltageDifference >= 0 ? "+" : "-") + (int) Mathf.Abs(VoltageDifference) + "V";
        // node color by voltage
        if (VoltageDifference < 0)
            _renderer.material = _nodeMaterials[0];
        else if (VoltageDifference > 0)
            _renderer.material = _nodeMaterials[2];
        else
            _renderer.material = _nodeMaterials[1];
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
