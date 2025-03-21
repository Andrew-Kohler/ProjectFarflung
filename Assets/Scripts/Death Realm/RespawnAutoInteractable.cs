using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Handles auto interact effects of looking at respawn auto interactable, which triggers respawn sequence.
/// </summary>
public class RespawnAutoInteractable : MonoBehaviour
{
    [Header("Animation Sequence")]
    [SerializeField, Tooltip("The camera aimed into the visor")]
    private CinemachineVirtualCamera _visorCam;
    [SerializeField, Tooltip("Handles fade to black and vitals display animations")]
    private Animator _anim;

    public void InteractEffects()
    {
        StartCoroutine(DoInteractEffects());
    }

    public IEnumerator DoInteractEffects()
    {
        // player loses control
        GameManager.Instance.PlayerEnabled = false;

        // pan camera into visor
        CinemachineVirtualCamera mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _visorCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);

        // wait for camera to be for sure within the visor
        yield return new WaitForSeconds(2.0f);

        // activate respawn animator
        _anim.SetTrigger("Activate");

        // scene transition handled through animator event trigger... nothing here        
    }
}
