using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles assigning the player to an initial position based on a set of load transforms and an index set by the previous scene.
/// </summary>
public class PlayerPositionLoader : MonoBehaviour
{
    [SerializeField, Tooltip("Parent of transforms used to correctly place the player in this scene")]
    private GameObject _loadSpotParent;
    private List<GameObject> _loadSpots;

    private Transform _playerTransform;

    void Start()
    {
        _playerTransform = GetComponent<Transform>();
        _loadSpots = new List<GameObject>();

        // Precondition: assigned load parent
        if (_loadSpotParent is null)
            throw new System.Exception("Player does not have load spots assigned");
        // Precondition: at least one load transform contained
        if (_loadSpotParent.transform.childCount == 0)
            throw new System.Exception("Load spot parent has no spots to load the player to");

        // Get all the different points
        for (int i = 0; i < _loadSpotParent.transform.childCount; i++) 
            _loadSpots.Add(_loadSpotParent.transform.GetChild(i).gameObject);

        // Precondition: must be index in bounds
        if (GameManager.Instance.LoadPoint >= _loadSpots.Count)
            throw new System.Exception("Attempting to load Loadpoint index #" + GameManager.Instance.LoadPoint + ". But LoadPoints only contains " + _loadSpots.Count + " load spots.");
        
        // move player to load spot - the actual functionality!
        _playerTransform.position = _loadSpots[GameManager.Instance.LoadPoint].transform.position;
    }
}
