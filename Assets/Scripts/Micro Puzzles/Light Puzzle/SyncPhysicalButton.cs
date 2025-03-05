using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Synchronizes the physical appearance of the button to match the UI toggle
/// </summary>
public class SyncPhysicalButton : MonoBehaviour
{
    [SerializeField, Tooltip("Renderer that will have its material swapped.")]
    private MeshRenderer _button;
    [SerializeField, Tooltip("Materials button on/off state; 0 = off; 1 = on.")]
    private Material[] _materials;
    [SerializeField, Tooltip("Corresponding toggle that this button should match.")]
    private Toggle _toggle;
    [SerializeField, Tooltip("Animation that will play when material swaps.")]
    private Animator _animator;
    public string[] _triggers;

    private bool _currState;

    private void Awake()
    {
        // Precondition: only 2 materials
        if (_materials.Length != 2)
            throw new System.Exception("Physical Button MUST have exactly two linked materials. One for off, and one for on.");
    }

    // Start is called before the first frame update
    void Start()
    {
        // must be in start because initial toggle states are configured in Awake() of LightPuzzleHandler
        UpdateMaterial();

        //Get the animator that will control the press animantions
        _animator = GetComponentInChildren<Animator>();
        _currState = _toggle.isOn;
    }

    // Update is called once per frame
    void Update()
    {
        // check for updating material
        if (_toggle.isOn != _currState)
        {
            UpdateMaterial();
            _currState = _toggle.isOn;
        }
    }

    /// <summary>
    /// Updates material of the button to match the isOn state of the linked toggle.
    /// </summary>
    private void UpdateMaterial()
    {
        _button.material = _materials[_toggle.isOn ? 1 : 0];

        //make sure there is an animator assigned, updates button animation state based on material set
        if(_animator != null) 
        _animator.SetTrigger(_triggers[_toggle.isOn ? 1 : 0]);
    }
}
