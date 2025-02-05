using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    private bool _canInteract;
    private Interactable _obj;

    RaycastHit hit;
    Ray ray;


    // Player Interactor: Checks for if the player is in radius of an interactable

    private void OnEnable()
    {
        InputSystem.actions.FindAction("Interact").started += context => Interact(); 
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Interact").started -= context => Interact();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = new Ray(this.transform.position, transform.forward * 5);
        //Debug.DrawRay(this.transform.position, transform.forward * 5, Color.green);
        if (Physics.Raycast(ray, out hit, 5f, LayerMask.GetMask("Interactable")))
        {
            _canInteract = true;
            _obj = hit.collider.gameObject.GetComponent<Interactable>();
            _obj.ShowVFX();
        }
        else
        {
            _canInteract = false;
            if(_obj != null)
            {
                _obj.HideVFX();
                _obj = null;
            }
        }
    }

    private void Interact()
    {
        if (_canInteract)
        {
            _obj.GetComponent<Interactable>().InteractEffects();
        }
    }

}
