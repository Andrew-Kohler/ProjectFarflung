using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles checking for puzzle completion, and updates progression data upon completion.
/// </summary>
public class WireBoxHandler : MonoBehaviour
{
    public static event Action WireBoxFixed;

    [Header("Configuration")]
    [Tooltip("Name of this wire box puzzle state stored in game manager.")]
    public string IdentifierName;

    [Header("References")]
    [SerializeField, Tooltip("Output node from which success is checked from")]
    NodeSelector _outputNode;
    [SerializeField, Tooltip("Objects to disable when puzzle is inactive (functional components).")]
    private GameObject[] _functionalObjects;
    [SerializeField, Tooltip("Objects to indicate whether puzzle is won. (emissive portions only).")]
    private GameObject[] _indicatorLights;
    [SerializeField, Tooltip("Materials for indicating if puzzle is won. 0 = not won, 1 = won")]
    private Material[] _indicatorMaterials;

    private void Awake()
    {
        // Precondition: must have non-empty identifier name
        if (IdentifierName.Equals(""))
            throw new Exception("Incorrect Wire Box Configuration: MUST have non-empty identifier name.");

        // TODO: replace with OFF by default always, only enabling itself on interaction (waiting for interaction system)
        if (GameManager.Instance.SceneData.FixedWireBoxes.Contains(IdentifierName))
        {
            DisablePuzzle();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                cont = false;
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

            DisablePuzzle();

            // ensure power elements update accordingly
            WireBoxFixed?.Invoke();

            // TODO: camera pan out and box close
        }
    }

    /// <summary>
    /// Disables all functional portions of the wire box puzzle.
    /// Prevents computations from taking place while the puzzle is inactive.
    /// </summary>
    private void DisablePuzzle()
    {
        //toggle lights to green to indicate solving
        _indicatorLights[0].GetComponent<Renderer>().material = _indicatorMaterials[1];
        _indicatorLights[1].GetComponent<Renderer>().material = _indicatorMaterials[1];

        foreach (GameObject obj in _functionalObjects)
            obj.SetActive(false);
        this.enabled = false;
    }

    /// <summary>
    /// Enables all functional portions of the wire box puzzle.
    /// Useful when entering puzzle (i.e. activating it).
    /// </summary>
    private void EnablePuzzle()
    {
        foreach (GameObject obj in _functionalObjects)
            obj.SetActive(true);
        this.enabled = true;
    }
}
