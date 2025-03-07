using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bypasses the Outline script in a sneaky way so that the base of the in/out nodes don't receive an outline.
/// </summary>
public class BodyToggleSync : MonoBehaviour
{
    [Header("InNode Objects")]
    [SerializeField, Tooltip("Object that should be highlighted and used for collider detection.")]
    GameObject _inOutNode;
    [SerializeField, Tooltip("Object that should not be highlighted or used for collider detection, but need to be activated for visuals.")]
    GameObject _inOutBody;

    void Start()
    {
        //if node is a in/out node also turn on the body
        if (_inOutNode.activeSelf == true)
            _inOutBody.SetActive(true);
        else
            _inOutBody.SetActive(false);
    }
}
