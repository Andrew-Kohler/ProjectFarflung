using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles proper processing of individual node clicks based on the bigger picture of selected wire and previous node states.
/// </summary>
public class NodeManager : MonoBehaviour
{
    [SerializeField, Tooltip("Used to access current selected wire state")]
    private WireManager _wireManager;

    private NodeSelector[] _nodes;
    private NodeSelector _firstNode = null; // null = none selected

    private void Awake()
    {
        _nodes = GetComponentsInChildren<NodeSelector>();

        // Precondition: must contain nodes
        if (_nodes.Length == 0)
            throw new System.Exception("Incorrect Node Configuration. Node Manager must contain AT LEAST one NodeSelector");
    }

    // Update is called once per frame
    void Update()
    {
        // Check for no valid wire to connect with
        // Handles de-selecting node if wire is de-selected
        if (_wireManager.GetSelectedWire() is null)
            DeselectFirstNode();
    }

    public void ProcessNodeClick(NodeSelector node)
    {
        // Don't even try if there is no selected wire
        if (_wireManager.GetSelectedWire() is null)
            return;

        // attaching first half of wire
        if (_firstNode is null)
        {
            node.SelectVisual(); // show outline
            _firstNode = node;
        }
        // cancel connection
        else if (_firstNode == node)
        {
            DeselectFirstNode();
        }
        // attaching second half of wire
        else if (true) // TODO: add check here for wire length
        {

        }
    }

    /// <summary>
    /// Used to cancel first node selection.
    /// Used on re-press and when wire type is de-selected.
    /// </summary>
    private void DeselectFirstNode()
    {
        if (_firstNode is not null)
            _firstNode.DeselectVisual();
        _firstNode = null;
    }
}
