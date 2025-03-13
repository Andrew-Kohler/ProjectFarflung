using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _raycastDistance = 3f;
    private bool _canInteract;
    private Interactable _obj;

    RaycastHit hit;
    Ray ray;


    // Player Interactor: Checks for if the player is in radius of an interactable

    #region Controls Bindings
    private void OnEnable()
    {
        InputSystem.actions.FindAction("Interact").started += DoInteract; 
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Interact").started -= DoInteract;
    }

    /// <summary>
    /// Simply calls Interact() function.
    /// Necessary to avoid memory leak.
    /// </summary>
    private void DoInteract(InputAction.CallbackContext context)
    {
        Interact();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = new Ray(this.transform.position, transform.forward * _raycastDistance);
        Debug.DrawRay(this.transform.position, transform.forward * _raycastDistance, Color.green);
        if (Physics.Raycast(ray, out hit, _raycastDistance, LayerMask.GetMask("Interactable", "Default")))
        {
            if (hit.collider.CompareTag("Interactable")){
                _canInteract = true;
                if (_obj == null)
                {
                    _obj = hit.collider.gameObject.GetComponent<Interactable>();
                    _obj.ShowVFX();
                }
            }
            else
            {
                _canInteract = false;
                if (_obj != null)
                {
                    _obj.HideVFX();
                    _obj = null;
                }
            }
            
        }
        else
        {
            _canInteract = false;
            if (_obj != null)
            {
                _obj.HideVFX();
                _obj = null;
            }
        }
    }

    private void Interact()
    {
        if (_canInteract && GameManager.Instance.PlayerEnabled)
        {
            _obj.GetComponent<Interactable>().InteractEffects();
        }
    }

}
