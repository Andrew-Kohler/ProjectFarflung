using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality of clicking a wire to select it as the current wire to be used.
/// Also handles visual behavior of outline shader.
/// </summary>
public class WireSelector : ClickableObject
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Length of current wire segment.")]
    public float Length;

    [Header("Visual")]
    [SerializeField, Tooltip("Thickness of selected outline, according to Outline.cs.")]
    private float _outlineWidth;

    private WireManager _wireManager;
    private Outline _outline;
    private bool _isSelected = false;

    void Awake()
    {
        // Precondition: must be contained by WireManager
        if (transform.parent is null || !transform.parent.TryGetComponent(out _wireManager))
            throw new System.Exception("Incorrect wire configuration. A WireSelector MUST be a child of a WireManager.");

        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineWidth = 0; // no outline by default
    }

    // Update is called once per frame
    override protected void Update()
    {
        // ensure click check occurs
        base.Update();
    }

    public override void OnObjectClick()
    {
        // Deselection
        if (_isSelected)
            _wireManager.DeselectWire(this);
        // Selection
        else
            _wireManager.SelectNewWire(this);
    }

    /// <summary>
    /// Removes visual outline.
    /// </summary>
    public void DeselectVisual()
    {
        _outline.OutlineWidth = 0;
        _isSelected = false;
    }

    /// <summary>
    /// Shows visual outline.
    /// </summary>
    public void SelectVisual()
    {
        _outline.OutlineWidth = _outlineWidth;
        _isSelected = true;
    }
}
