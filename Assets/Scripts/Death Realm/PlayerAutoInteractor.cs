using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles interaction WITHOUT pressing E to be used on Death Realm respawn terminal.
/// </summary>
public class PlayerAutoInteractor : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum distance from which the look trigger can activate.")] 
    private float _raycastDistance = 3f;
    
    private RespawnAutoInteractable _obj = null;
    RaycastHit hit;
    Ray ray;

    // Update is called once per frame
    void Update()
    {
        ray = new Ray(this.transform.position, transform.forward * _raycastDistance);
        Debug.DrawRay(this.transform.position, transform.forward * _raycastDistance, Color.green);
        if (Physics.Raycast(ray, out hit, _raycastDistance, LayerMask.GetMask("Interactable", "Default")))
        {
            // activate effects upon looking at the object
            if (hit.collider.CompareTag("Interactable"))
            {
                _obj = hit.collider.gameObject.GetComponent<RespawnAutoInteractable>();
                _obj.InteractEffects();
            }
            else
            {
                // still waiting to interact
                _obj = null;
            }
        }
        else
        {
            // still waiting to interact
            _obj = null;
        }
    }
}
