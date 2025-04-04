using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

/// <summary>
/// Handles assigning the player to an initial position based on a set of load transforms and an index set by the previous scene.
/// </summary>
public class PlayerPositionLoader : MonoBehaviour
{
    [SerializeField, Tooltip("Parent of transforms used to correctly place the player in this scene; if unassigned, this script will be ignored.")]
    private GameObject _loadSpotParent;
    [SerializeField, Tooltip("Used to determine if terminal to load to exists.")]
    private TerminalConfiguration[] _terminals; // list of references to avoid costly GetComponentInChildren calls
    [SerializeField, Tooltip("Angle of camera when loading to terminal - to ensure looking at terminal and not at the wall.")]
    private float _terminalCamAngle;
    [SerializeField, Tooltip("Needed to override starting pitch on camera.")]
    private FirstPersonController _controller;

    void Start()
    {
        // Resume based on terminal data
        if (GameManager.Instance.LoadPoint == -1)
        {
            // simply load first load spot if terminal index is set to -1
            // if there is no save terminal, that means no terminal has been opened since starting a new save - default to original position (first in Load pos list)
            if (GameManager.Instance.SceneData.SaveTerminal == -1)
            {
                if (!_loadSpotParent || _loadSpotParent.transform.childCount == 0)
                {
                    Debug.LogWarning("PlayerPositionLoader attempting to default to first position. " +
                        "This will not work because there is either no load spot parent assigned, or it contains no children transforms. " +
                        "The player will simply remain where they were placed in editor.");
                    return;
                }

                // default load spot
                Transform defaultLoadSpot = _loadSpotParent.transform.GetChild(0);
                transform.position = defaultLoadSpot.position;
                transform.rotation = defaultLoadSpot.rotation;
                return;
            }

            foreach (TerminalConfiguration terminal in _terminals)
            {
                if (terminal.ZoneIndex == GameManager.Instance.SceneData.SaveTerminal)
                {
                    transform.position = terminal.SpawnPos.position;
                    transform.rotation = terminal.SpawnPos.rotation;

                    // rotate camera follow transform to face terminal
                    Transform followTransform = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.
                        VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>().Follow;
                    followTransform.Rotate(Vector3.right * _terminalCamAngle);

                    // ensure player controller does not override angle
                    _controller.OverrideTargetPitch(_terminalCamAngle);

                    return;
                }
            }

            // if we got this far, no matching terminal was found - no loading occurs (throw a warning)
            Debug.LogWarning("PlayerPositionLoader could not find the appropriate terminal in this scene to load position based on save data." +
                " This should NOT occur through normal play but can occur through editor testing. The player will simply remain where they were placed in editor.");
        }
        // resume based on specific load point (i.e. door, ladder, elevator, etc.)
        else
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
            transform.rotation = loadSpots[GameManager.Instance.LoadPoint].transform.rotation;
        }
    }
}
