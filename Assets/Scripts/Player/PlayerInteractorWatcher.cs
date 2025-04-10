using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractorWatcher : MonoBehaviour
{
    private PlayerInteractor _interactor;   
    private void OnEnable()
    {
        TerminalInteractable.onLockedInteractionTerminal += ToggleInteractor;
        WireBoxInteractable.onLockedInteractionWirebox += ToggleInteractor;
    }

    private void OnDisable()
    {
        TerminalInteractable.onLockedInteractionTerminal -= ToggleInteractor;
        WireBoxInteractable.onLockedInteractionWirebox -= ToggleInteractor;
    }
    void Start()
    {
        _interactor = GetComponent<PlayerInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleInteractor(bool off)
    {
        if(_interactor != null)
        {
            if (off) _interactor.enabled = false;
            else _interactor.enabled = true;
        }
        
    }
}
