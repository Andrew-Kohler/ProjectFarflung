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

    [Header("Model Configuration")]
    [SerializeField, Tooltip("Stuck tape object.")]
    public GameObject StuckTape;
    [SerializeField, Tooltip("Un-Stuck tape object.")]
    public GameObject UnstuckTape;
    [SerializeField, Tooltip("Wire object for outline controls.")]
    public GameObject WireModel;

    private WireManager _wireManager;
    private Outline _outline;
    private bool _isSelected = false;

    void Awake()
    {
        // Precondition: must be contained by WireManager
        if (transform.parent is null || !transform.parent.TryGetComponent(out _wireManager))
            throw new System.Exception("Incorrect wire configuration. A WireSelector MUST be a child of a WireManager.");

        _outline = WireModel.AddComponent<Outline>();
        _outline.OutlineWidth = 0; // no outline by default
    }

    public override void OnObjectClick()
    {
        // Deselection
        if (_isSelected)
            _wireManager.DeselectWire(this);
        // Selection
        else
            _wireManager.SelectNewWire(this);


        // no matter what, this click should remove any first node connection made on a node of the board
        _wireManager.NodeManager.DeselectFirstNode();
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
