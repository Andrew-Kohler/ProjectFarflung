using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (_loadSpotParent is null)
        {
            throw new System.Exception("Player does not have load spots assigned");
        }
        if (_loadSpotParent.transform.childCount == 0)
        {
            throw new System.Exception("Load spot parent has no spots to load the player to");
        }

        for (int i = 0; i < _loadSpotParent.transform.childCount; i++) // Get all the different points
            _loadSpots.Add(_loadSpotParent.transform.GetChild(i).gameObject);

        _playerTransform.position = _loadSpots[GameManager.Instance.LoadPoint].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
