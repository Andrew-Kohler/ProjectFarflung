using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public abstract class Interactable : MonoBehaviour
{
    // Placed on the interaction trigger of whatever the thing is

    [SerializeField] protected GameObject _vfx;
    [SerializeField] protected LookAtConstraint _eLookAt;
    protected Transform _camTransform;
    protected bool _isActiveCoroutine = false;

    protected IEnumerator _currentHideCoroutine;

    public delegate void OnLockedInteraction(bool start);
    protected void Start()
    {
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = Camera.main.transform;
        _camTransform = src.sourceTransform; // Use this call to also keep the camera transform for the door to guage distance against
        src.weight = 1;
        _eLookAt.AddSource(src);
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public virtual void ShowVFX()
    {
        if(_currentHideCoroutine != null)
            StopCoroutine(_currentHideCoroutine);
        _isActiveCoroutine = false;
        _vfx.SetActive(true);
        _vfx.GetComponent<Animator>().Play("Appear");
    }

    public virtual void HideVFX()
    {
        if (!_isActiveCoroutine)
        {
            _currentHideCoroutine = DoHideVFX();
            StartCoroutine(_currentHideCoroutine);
        }
            
    }

    private IEnumerator DoHideVFX()
    {
        _isActiveCoroutine = true;
        _vfx.GetComponent<Animator>().Play("Disappear"); // Standard name for VFX vanish anim
        yield return new WaitForSeconds(.16f);
        _vfx.SetActive(false);
        _isActiveCoroutine = false;
    }

    public abstract void InteractEffects();
}
