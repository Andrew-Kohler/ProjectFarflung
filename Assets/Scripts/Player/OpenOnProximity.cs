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
    [SerializeField, Tooltip("Used to trigger SFX")]
    private AudioSource _audio;

    private Transform _player = null;

    private bool _isOpen = false;
    private bool _firstAnim = false;    // ensures animator plays out animation properly on start

    private void Start()
    {
        // fetch player transform on start - not ideal, but the alternate is adding a reference n every elevator/dumbwaiter
        _player = GameObject.Find("PlayerCapsule").transform;
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
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && _firstAnim)
                {
                    _anim.Play("Open", -1, 1f - _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

                    // reverse SFX
                    _audio.pitch = -_audio.pitch;
                }
                else
                {
                    _anim.Play("Open");

                    // SFX - prevent layering multiple sounds
                    _audio.Stop();
                    _audio.pitch = 1;
                    _audio.PlayOneShot(_audio.clip);
                }

                _isOpen = true;
                _firstAnim = true;
            }
        }
        // close logic
        else
        {
            // don't restart the close anim
            if (_isOpen)
            {
                // ensure smooth close even if open has not completed
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && _firstAnim)
                {
                    _anim.Play("Close", -1, 1f - _anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

                    // reverse SFX
                    _audio.pitch = -_audio.pitch;
                }
                else
                {
                    // SFX - prevent layering multiple sounds
                    _audio.Stop();
                    _audio.pitch = 1;
                    _audio.PlayOneShot(_audio.clip);

                    _anim.Play("Close");
                }

                _isOpen = false;
                _firstAnim = true;
            }
        }
    }
}
