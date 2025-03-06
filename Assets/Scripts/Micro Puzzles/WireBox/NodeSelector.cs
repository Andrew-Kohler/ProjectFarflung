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

    [Header("Configuration")]
    [SerializeField, Tooltip("Whether the current node is an end node and only permits ONE connection.")]
    public bool IsEndNode;

    [Header("Visuals")]
    [SerializeField, Tooltip("Thickness of selected outline, according to Outline.cs.")]
    private float _outlineWidth;
    [SerializeField, Tooltip("Used to configure text to match voltage customization.")]
    private TextMeshProUGUI _voltageText;
    [SerializeField, Tooltip("Models for different charge types of node. 0 = negative, 1 = end node, 2 = positive")]
    private GameObject[] _nodeModels;

    private NodeSelector _connection1 = null;
    private NodeSelector _connection2 = null;

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
        // node color/model by voltage
        if (IsEndNode == true)
        {
            _nodeModels[0].SetActive(false);
            _nodeModels[1].SetActive(true);
            _nodeModels[2].SetActive(false);
        }
        else
        {
            if (VoltageDifference < 0)
            {
                _nodeModels[0].SetActive(true);
                _nodeModels[1].SetActive(false);
                _nodeModels[2].SetActive(false);
            }
            else if (VoltageDifference > 0)
            {
                _nodeModels[0].SetActive(false);
                _nodeModels[1].SetActive(false);
                _nodeModels[2].SetActive(true);
            }
            else
            {
                //remove this since neutral case isnt happening anymore?
                    _nodeModels[0].SetActive(false);
                    _nodeModels[1].SetActive(true);
                    _nodeModels[2].SetActive(false);
            }
        }
    }
       

    public override void OnObjectClick()
    {
        // cannot connect more than two connections on a single node - skip processing
        if (_connection1 is not null && _connection2 is not null)
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

    /// <summary>
    /// Adds input node as one the open connections (arbitrary).
    /// </summary>
    public void AssignConnection(NodeSelector node)
    {
        if (_connection1 is null)
        {
            _connection1 = node;
            return;
        }    
        else if (!IsEndNode && _connection2 is null)
        {
            _connection2 = node;
            return;
        }

        // no place to assign connection
        throw new System.Exception("Improper use of AssignConnection, there must be an open connection");
    }

    /// <summary>
    /// Removes connection that matches the provided node.
    /// </summary>
    public void RemoveConnection(NodeSelector node)
    {
        if (_connection1 is not null && _connection1 == node)
        {
            _connection1 = null;
            return;
        }
        else if (_connection2 is not null && _connection2 == node)
        {
            _connection2 = null;
            return;
        }

        throw new System.Exception("Improper use of RemoveConnection, can only provide a node that currently is a connection.");
    }

    /// <summary>
    /// Returns whether there are no more available slots on this node to attach another wire.
    /// </summary>
    public bool AreConnectionsFull()
    {
        return _connection1 is not null && (IsEndNode || _connection2 is not null);
    }

    /// <summary>
    /// Returns whether there are any connections attached to this node.
    /// </summary>
    public bool HasAnyConnections()
    {
        return _connection1 is not null || _connection2 is not null;
    }

    /// <summary>
    /// Returns the next connection that is NOT the provided one.
    /// Useful for one-way traversal of a node chain.
    /// </summary>
    public NodeSelector GetNextConnection(NodeSelector node)
    {
        if (_connection1 == node)
        {
            return _connection2 is null ? null : _connection2;
        }
        else if (_connection2 == node)
        {
            return _connection1 is null ? null : _connection1;
        }

        throw new System.Exception("Incorrect usage of GetNextConnection. The provided node MUST be one of the connections.");
    }

    /// <summary>
    /// Returns the first connection on this node.
    /// </summary>
    public NodeSelector GetFirstConnection()
    {
        if (_connection1 is null)
            throw new System.Exception("Incorrect usage of GetFirstCOnnection. The first connection must exist to access it.");

        return _connection1;
    }
}
