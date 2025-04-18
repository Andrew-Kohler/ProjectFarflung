using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Plays opening animation upon being close enough to an elevator or dumbwaiter object.
/// </summary>
public class OpenOnProximity : MonoBehaviour
{
    [SerializeField, Tooltip("Distance within which the door will open.")]
    private float _openDistance;
    [SerializeField, Tooltip("Used to trigger the open animation.")]
    private Animator _anim;

    private Transform _player;

    private bool _isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        // fetch player transform on start through main camera - this avoids needing a direct reference on the objects
        _player = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>().Follow.parent;

        // default to staying in closed state
        _anim.Play("Close", -1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = _player.position;
        playerPos.y = 0; // ignore height
        Vector3 objPos = transform.position;
        objPos.y = 0;   // ignore height

        // open logic
        if (Vector3.Distance(playerPos, objPos) < _openDistance)
        {
            // don't restart the anim
            if (!_isOpen)
            {
                // ensure smooth open even if open has not completed
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                    _anim.Play("Open", -1, 1f - _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                else
                    _anim.Play("Open");

                _isOpen = true;
            }
        }
        // close logic
        else
        {
            // don't restart the close anim
            if (_isOpen)
            {
                // ensure smooth close even if open has not completed
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                    _anim.Play("Close", -1, 1f - _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                else
                    _anim.Play("Close");

                _isOpen = false;
            }
        }
    }
}
