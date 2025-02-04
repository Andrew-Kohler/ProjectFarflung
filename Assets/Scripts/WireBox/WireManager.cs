using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour
{
    private WireSelector[] _wires;
    private WireSelector _currWire = null;  // null = none selected

    private void Awake()
    {
        _wires = GetComponentsInChildren<WireSelector>();

        // Precondition: must contain wires
        if (_wires.Length == 0)
            throw new System.Exception("Incorrect Wire Configuration. Wire Manager must contain AT LEAST one WireSelector");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets all wires to deselected state.
    /// </summary>
    public void SelectNewWire(WireSelector newWire)
    {
        // first deselect all so that there is never two selected at once
        foreach (WireSelector wire in _wires)
            wire.DeselectVisual();

        newWire.SelectVisual();
        _currWire = newWire;
    }

    /// <summary>
    /// Disables visuals of wire and unassigns it in the manager.
    /// </summary>
    public void DeselectWire(WireSelector wire)
    {
        wire.DeselectVisual();
        _currWire = null;
    }
}
