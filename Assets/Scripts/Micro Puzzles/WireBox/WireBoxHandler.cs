using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles checking for puzzle completion, and updates progression data upon completion.
/// </summary>
public class WireBoxHandler : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Name of this wire box puzzle state stored in game manager.")]
    private string _identifierName;

    [Header("References")]
    [SerializeField, Tooltip("Output node from which success is checked from")]
    NodeSelector _outputNode;
    [SerializeField, Tooltip("Objects to disable when puzzle is inactive (functional components).")]
    private GameObject[] _functionalObjects;

    private void Awake()
    {
        // TODO: replace with OFF by default always, only enabling itself on interaction (waiting for interaction system)
        if (GameManager.Instance.SceneData.FixedWireBoxes.Contains(_identifierName))
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
            GameManager.Instance.SceneData.FixedWireBoxes.Add(_identifierName);

            DisablePuzzle();

            // TODO: camera pan out and box close
        }
    }

    /// <summary>
    /// Disables all functional portions of the wire box puzzle.
    /// Prevents computations from taking place while the puzzle is inactive.
    /// </summary>
    private void DisablePuzzle()
    {
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
