using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles checking for puzzle completion, and updates progression data upon completion.
/// </summary>
public class WireBoxHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Output node from which success is checked from")]
    NodeSelector _outputNode;

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
            // TODO: interface with game manager

            // TODO: camera pan out and box close
        }
    }
}
