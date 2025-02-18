using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public abstract class Interactable : MonoBehaviour
{
    // Placed on the interaction trigger of whatever the thing is

    [SerializeField] protected GameObject _vfx;
    [SerializeField] protected LookAtConstraint _eLookAt;
    protected void Start()
    {
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = Camera.main.transform;
        src.weight = 1;
        _eLookAt.AddSource(src);
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public void ShowVFX()
    {
        _vfx.SetActive(true);
    }

    public void HideVFX()
    {
        StartCoroutine(DoHideVFX());
    }

    private IEnumerator DoHideVFX()
    {
        _vfx.GetComponent<Animator>().Play("Disappear"); // Standard name for VFX vanish anim
        yield return new WaitForSeconds(.16f);
        _vfx.SetActive(false);
    }

    public abstract void InteractEffects();
}
