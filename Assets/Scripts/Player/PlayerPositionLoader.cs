using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles assigning the player to an initial position based on a set of load transforms and an index set by the previous scene.
/// </summary>
public class PlayerPositionLoader : MonoBehaviour
{
    [SerializeField, Tooltip("Parent of transforms used to correctly place the player in this scene; if unassigned, this script will be ignored.")]
    private GameObject _loadSpotParent;

    void Start()
    {
        // If there is no load parent OR the load parent has no children, then simply do not move the player
        // this allows improperly configured scenes to still be used for testing
        // Note: 'is null' does not work on GameObjects due to how Unity handles game object null references - this weird way works instead
        if (!_loadSpotParent || _loadSpotParent.transform.childCount == 0)
        {
            Debug.LogWarning("PlayerPositionLoader will not work because there is either no load spot parent assigned, or it contains no children transforms. " +
                "The player will simply remain where they were placed in editor.");
            return;
        }

        // Get all the different points
        List<GameObject> loadSpots = new List<GameObject>();
        for (int i = 0; i < _loadSpotParent.transform.childCount; i++) 
            loadSpots.Add(_loadSpotParent.transform.GetChild(i).gameObject);

        // Precondition: must be index in bounds (within rangeis acceptable)
        if (GameManager.Instance.LoadPoint >= loadSpots.Count || GameManager.Instance.LoadPoint < 0)
            throw new System.Exception("Attempting to load Loadpoint index #" + GameManager.Instance.LoadPoint + ". But LoadPoints only contains " + loadSpots.Count + " load spots.");
        
        // move player to load spot - the actual functionality!
        transform.position = loadSpots[GameManager.Instance.LoadPoint].transform.position;
    }
}
