using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles functionality of clicking a wire to select it as the current wire to be used
/// </summary>
public class WireSelector : ClickableObject
{
    [SerializeField, Tooltip("Thickness of selected outline, according to Outline.cs.")]
    private float _outlineWidth;

    private Outline _outline;

    // Start is called before the first frame update
    void Awake()
    {
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
        _outline.OutlineWidth = _outlineWidth;
    }
}
