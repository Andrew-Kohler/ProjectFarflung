using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles proper processing of individual node clicks based on the bigger picture of selected wire and previous node states.
/// </summary>
public class NodeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Used to access current selected wire state")]
    private WireManager _wireManager;
    [SerializeField, Tooltip("Used to dynamically create, position, and scale wire connection")]
    private GameObject _wireConnectionPrefab;

    private InputAction _mousePosAction;
    private NodeSelector[] _nodes;
    private NodeSelector _firstNode = null; // null = none selected
    private GameObject _currConnection = null;

    private void Awake()
    {
        _nodes = GetComponentsInChildren<NodeSelector>();

        // Precondition: must contain nodes
        if (_nodes.Length == 0)
            throw new System.Exception("Incorrect Node Configuration. Node Manager must contain AT LEAST one NodeSelector");

        _mousePosAction = InputSystem.actions.FindAction("MousePosition");
    }

    // Update is called once per frame
    void Update()
    {
        // Check for no valid wire to connect with
        // Handles de-selecting node if wire is de-selected
        if (_wireManager.GetSelectedWire() is null)
            DeselectFirstNode();

        // render wire between node and mouse
        if (_firstNode is not null)
        {
            // create new connection
            if (_currConnection is null)
            {
                // parented to node manager
                _currConnection = Instantiate(_wireConnectionPrefab, transform);
            }

            // update connection
            Vector3 mousePos = _mousePosAction.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f, LayerMask.GetMask("WireBoxGrid")))
            {
                // show wire (mouse is on grid)
                _currConnection.SetActive(true);

                // fetch endpoints
                Vector3 hitPos = hit.point;
                hitPos.z = _currConnection.transform.position.z;
                Vector3 nodePos = _firstNode.transform.position;

                ShowWire(nodePos, hitPos);
            }
            else
            {
                // don't show wire (mouse is not on grid)
                _currConnection.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Renders wire between two provided vector3 points
    /// </summary>
    private void ShowWire(Vector3 originPos, Vector3 endPos)
    {
        // Max Length
        float maxLength = _wireManager.GetSelectedWire().Length;
        if (Vector3.Distance(endPos, originPos) > maxLength) // 10x scale factor from unity coords to length unit
        {
            // POSITION
            Vector3 newEndpoint = originPos + ((endPos - originPos).normalized * maxLength); // 10x scale factor from unity coords to length unit
            _currConnection.transform.position = (originPos + newEndpoint) / 2f;

            // SCALE
            Vector3 scale = _currConnection.transform.localScale;
            scale.x = _wireManager.GetSelectedWire().Length * 10f; // 10x scale factor from unit coords to length units
            _currConnection.transform.localScale = scale;
        }
        // Within Length Limit
        else
        {
            // POSITION
            _currConnection.transform.position = (endPos + originPos) / 2f;

            // SCALE
            Vector3 scale = _currConnection.transform.localScale;
            scale.x = Vector3.Distance(endPos, originPos) * 10f; // 10x scale factor from unity coords to grid units
            _currConnection.transform.localScale = scale;
        }

        // ROTATE
        Quaternion rot = Quaternion.LookRotation(originPos - endPos, Vector3.forward);
        rot *= Quaternion.Euler(0, 90, 0); // rotate 90 degrees (to account for improperly aligned forward)
        _currConnection.transform.rotation = rot;
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

            // ensure display wire is removed
            Destroy(_currConnection);
            _currConnection = null;
        }
        // attaching second half of wire - RANGE PERMITTING
        else if (Vector3.Distance(node.transform.position, _firstNode.transform.position) < _wireManager.GetSelectedWire().Length)
        {
            // snap wire between two nodes
            ShowWire(_firstNode.transform.position, node.transform.position);

            // sever control over these connections (the wire has been placed)
            DeselectFirstNode();

            // properly initialize completed connection
            ConnectionRemover newConnection = _currConnection.transform.GetChild(0).gameObject.AddComponent<ConnectionRemover>();
            newConnection.Initialize(_wireManager.GetSelectedWire());
            _currConnection = null;
            _wireManager.ConsumeCurrentWire();
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