using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles checking for puzzle completion, and updates progression data upon completion.
/// </summary>
public class WireBoxHandler : MonoBehaviour
{
    public static event Action WireBoxFixed;

    [Header("Configuration")]
    [Tooltip("Name of this wire box puzzle state stored in game manager.")]
    public string IdentifierName;
    [Tooltip("Light that must be on for the player to be permitted to interact with the wire box.")]
    public PoweredLight LightZone;

    [Header("References")]
    [SerializeField, Tooltip("Output node from which success is checked from")]
    NodeSelector _outputNode;
    [SerializeField, Tooltip("Objects to disable when puzzle is inactive (functional components).")]
    private GameObject[] _functionalObjects;
    [SerializeField, Tooltip("Interaction object")]
    WireBoxInteractable _interactable;
    [SerializeField, Tooltip("Light objects to indicate whether puzzle is won. (emissive portions only).")]
    private GameObject[] _indicatorLights;
    [SerializeField, Tooltip("Materials for indicating if puzzle is won. 0 = not won, 1 = won")]
    private Material[] _indicatorMaterials;
    [SerializeField, Tooltip("Used to configure text to match voltage total.")]
    private TextMeshProUGUI _currentVoltageText;

    private void Awake()
    {
        // Precondition: MUST be associated with a light to determine if interactor should be disabled
        if (!LightZone)
            throw new Exception("Wire Box MUST have a LightZone to correspond with whether the wire box interaction is permitted (cannot open box in the dark).");

        // Precondition: must have non-empty identifier name
        if (IdentifierName.Equals(""))
            throw new Exception("Incorrect Wire Box Configuration: MUST have non-empty identifier name.");

        // disabled until interacted with (interaction system accounts for completion to block re-interaction)
        DisablePuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        // skip detection of completion condition if already completed
        // this prevents identifier from being added to the list MANY times
        if (GameManager.Instance.SceneData.FixedWireBoxes.Contains(IdentifierName))
            return;

        // puzzle not completed if end connection has no nodes
        if (!_outputNode.HasAnyConnections())
            return;

        // check puzzle completion
        int chargeTotal = 0;
        NodeSelector prevNode = _outputNode;
        NodeSelector currNode = _outputNode.GetFirstConnection();

        // calculate charge total
        bool cont = true;
        while (cont)
        {
            chargeTotal += currNode.VoltageDifference;
            if (currNode.GetNextConnection(prevNode) is null)
            {
                cont = false;
                _currentVoltageText.text = (chargeTotal >= 0 ? "+" : "-") + (int)Mathf.Abs(chargeTotal) + "V";
                if (chargeTotal == 0)
                {
                    _currentVoltageText.color = Color.white;
                }
                else if (chargeTotal > 0)
                {
                    _currentVoltageText.color = Color.green;
                }
                else
                {
                    _currentVoltageText.color = Color.red;
                }
            }
            else
            {
                NodeSelector temp = currNode;
                currNode = currNode.GetNextConnection(prevNode);
                prevNode = temp;
            }
        }

        // Puzzle completion conditions
        // (1) connected to start node
        // (2) charge total equals expected output charge
        if (currNode.IsEndNode && chargeTotal == _outputNode.VoltageDifference)
        {
            // mark puzzle as complete
            GameManager.Instance.SceneData.FixedWireBoxes.Add(IdentifierName);

            //toggle lights to green to indicate solving
            _indicatorLights[0].GetComponent<Renderer>().material = _indicatorMaterials[1];
            _indicatorLights[1].GetComponent<Renderer>().material = _indicatorMaterials[1];

            DisablePuzzle();

            // ensure power elements update accordingly
            WireBoxFixed?.Invoke();

            // Give the player back control and disable further access to the puzzle
            _interactable.ReenablePlayer(true);
        }
    }

    /// <summary>
    /// Disables all functional portions of the wire box puzzle.
    /// Prevents computations from taking place while the puzzle is inactive.
    /// </summary>
    public void DisablePuzzle()
    {
        // Disable all puzzle objects, ONCE BOX CLOSES
        // the reason for disabling at all is because the mouse raycast checks on each clickable object are fairly costly and probably not good to keep going throughout the whole scene play
        StartCoroutine(DoDisableAfterDelay());
    }

    private IEnumerator DoDisableAfterDelay()
    {
        // give time for the box to close before objects are disabled
        yield return new WaitForSeconds(1f);

        // disables all the objects other than the box itself
        foreach (GameObject obj in _functionalObjects)
            obj.SetActive(false);

        //prevents use of any nodes/door wires left etc
        this.enabled = false;
    }

    /// <summary>
    /// Enables all functional portions of the wire box puzzle.
    /// Useful when entering puzzle (i.e. activating it).
    /// </summary>
    public void EnablePuzzle()
    {
        foreach (GameObject obj in _functionalObjects)
            obj.SetActive(true);
        this.enabled = true;

        // animation call can go here for opening puzzle??
    }
}
