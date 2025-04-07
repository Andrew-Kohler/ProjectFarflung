using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles proper processing of individual node clicks based on the bigger picture of selected wire and previous node states.
/// </summary>
public class NodeManager : MonoBehaviour
{
    [Header("Obstructions / Adjustments")]
    [SerializeField, Tooltip("Radius of obstruction overlap capsule checks.")]
    private float _obstructionCheckRadius;
    [SerializeField, Tooltip("Inwards offset between nodes of obstruction check to prevent wire tips from touching within a node and registering as obstruction.")]
    private float _obstructionInwardOffset;
    [SerializeField, Tooltip("Max length reducing offset for wire preview to better align with actual snapping distance (since proper functional snap distance is center, but visually should be edge for clarity).")]
    private float _maxLengthVisualOffset;

    [Header("References")]
    [SerializeField, Tooltip("Used to access current selected wire state")]
    private WireManager _wireManager;
    [SerializeField, Tooltip("Used to dynamically create, position, and scale wire connection")]
    private GameObject _wireConnectionPrefab;

    [Header("Connection Remover Visuals")]
    [SerializeField, Tooltip("Thickness of selected outline, according to Outline.cs on connection removers (placed wires).")]
    private float _outlineWidth;
    [SerializeField, Tooltip("Color of hover outline on connection remover (placed wires).")]
    private Color _hoverColor;

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

                // match wire to corresponding MATERIAL
                Renderer wireRenderer = _currConnection.GetComponentInChildren<Renderer>();
                Renderer selectedRenderer = _wireManager.GetSelectedWire().WireRenderer;
                if (wireRenderer is null || selectedRenderer is null)
                    throw new System.Exception("BOTH wire removers and wire selectors MUST have a renderer component in their children.");
                wireRenderer.material = selectedRenderer.material;
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
        float maxLength = _wireManager.GetSelectedWire().Length - _maxLengthVisualOffset;
        if (Vector3.Distance(endPos, originPos) > maxLength) // 10x scale factor from unity coords to length unit
        {
            // POSITION
            Vector3 newEndpoint = originPos + ((endPos - originPos).normalized * maxLength); // 10x scale factor from unity coords to length unit
            _currConnection.transform.position = (originPos + newEndpoint) / 2f;

            // SCALE
            Vector3 scale = _currConnection.transform.localScale;
            scale.x = maxLength * 20f; // 10x scale factor from unit coords to length units
            _currConnection.transform.localScale = scale;
        }
        // Within Length Limit
        else
        {
            // POSITION
            _currConnection.transform.position = (endPos + originPos) / 2f;

            // SCALE
            Vector3 scale = _currConnection.transform.localScale;
            scale.x = Vector3.Distance(endPos, originPos) * 20f; // 10x scale factor from unity coords to grid units
            _currConnection.transform.localScale = scale;
        }

        // ROTATE
        Quaternion rot = Quaternion.LookRotation(originPos - endPos, Vector3.forward);
        rot *= Quaternion.Euler(0, 90, 0); // rotate 90 degrees (to account for improperly aligned forward)
        _currConnection.transform.rotation = rot;
    }

    public void ProcessNodeClick(NodeSelector clickedNode)
    {
        // Don't even try if there is no selected wire
        if (_wireManager.GetSelectedWire() is null)
            return;

        // cannot do ANYTHING to a node that already has two connections
        if (clickedNode.AreConnectionsFull())
            return;

        // NEW CONNECTION
        // attaching first half of wire
        if (_firstNode is null)
        {
            clickedNode.SelectVisual(); // show outline
            _firstNode = clickedNode;
        }
        // CANCEL CONNECTION
        else if (_firstNode == clickedNode)
        {
            // ensure display wire is removed
            Destroy(_currConnection);

            DeselectFirstNode();
        }
        // COMPLETE CONNECTION - range and obstruction permitting
        else if (Vector3.Distance(clickedNode.transform.position, _firstNode.transform.position) < _wireManager.GetSelectedWire().Length)
        {
            // determine if there are obstructions
            // CANNOT be placed if there are node/wire obstructions
            Collider[] potentialObstructions = new Collider[64]; // should be excessive, but will ensure no important collisions are missed
            CapsuleCollider collider = _currConnection.GetComponentInChildren<CapsuleCollider>();
            Vector3 pointTop = _firstNode.transform.position;
            Vector3 pointBot = clickedNode.transform.position;
            pointTop = pointTop + (pointBot - pointTop).normalized * _obstructionInwardOffset;
            pointBot = pointBot + (pointTop - pointBot).normalized * _obstructionInwardOffset;
            int numColls = Physics.OverlapCapsuleNonAlloc(pointTop, pointBot, _obstructionCheckRadius, potentialObstructions);
            for (int i = 0; i < numColls; i++)
            {
                // (1) obstructed by ANY other connection (we implicitly know any ConnectionRemover is not the current once since it does not have a ConnectionRemover yet)
                // (2) obstructed by ANY node that is not either endpoint
                if (potentialObstructions[i].TryGetComponent(out ConnectionRemover connectionColl)
                    || (potentialObstructions[i].TryGetComponent(out NodeSelector nodeColl) && nodeColl != _firstNode && nodeColl != clickedNode))
                {
                    return;
                }
            }

            // if we got this far, there is a successful connection!!!

            // snap wire between two nodes
            ShowWire(_firstNode.transform.position, clickedNode.transform.position);
            // properly initialize completed connection
            ConnectionRemover newConnection = _currConnection.transform.GetChild(0).gameObject.AddComponent<ConnectionRemover>();
            newConnection.Initialize(_wireManager.GetSelectedWire(), _firstNode, clickedNode, _outlineWidth, _hoverColor);

            // assign wire connections (for charge calculations) - two way reference
            _firstNode.AssignConnection(clickedNode);
            clickedNode.AssignConnection(_firstNode);

            // sever control over these connections (the wire has been placed)
            clickedNode.DeselectVisual(); // ensure hover outline is removed on wire placement
            DeselectFirstNode();

            // consume the current selected wire from the wire rack
            _wireManager.ConsumeCurrentWire();
        }
    }

    /// <summary>
    /// Used to cancel first node selection.
    /// Used on re-press and when wire type is de-selected.
    /// Also called when a new wire type is picked.
    /// </summary>
    public void DeselectFirstNode()
    {
        if (_firstNode is not null)
            _firstNode.DeselectVisual();
        _firstNode = null;

        // requires new first click to make new connection
        _currConnection = null;
    }

    // Used to complete cleanup when a player backs out of a box puzzle
    public void DestroyCurrentWire()
    {
        if(_currConnection is not null)
            Destroy(_currConnection);
        _currConnection = null;
    }

    /// <summary>
    /// Returns whether any wire selector is currently chosen
    /// </summary>
    public bool IsAnyWireSelected()
    {
        return _wireManager.GetSelectedWire() is not null;
    }
}